# Break-glass: recovering a locked-out Tutor admin

Use this when the only person who can manage accounts cannot sign in to the hosted site
(`https://mindattic-tutor.azurewebsites.net`). Passwords are stored only as Argon2id hashes with a pepper,
so nobody can read a password back. Every step below replaces a credential instead. Try the steps in order
and stop at the first one that works.

Accounts and sign-in live in the Azure SQL database `TutorAuth` (the server is in
`Tutor.Blazor/appsettings.json`, `ConnectionStrings:TutorAuth`). Signing in to it uses Microsoft Entra ID,
so these steps need an Entra account with access to that database, such as the owner's. Run SQL from the
Azure portal's query editor for the database, or `sqlcmd -G` with the owner's Entra login.

## 1. Another admin is available

That admin opens `/users` and resets the password, or re-enables the account. They can also tick
"must change password". Nothing else is needed.

## 2. The admin has the password but lost the authenticator

Sign in, and on the two-factor step (`/mfa`) tick "Use a recovery code instead" and enter one of the
recovery codes saved during setup. Each code works once. Then enrol a new authenticator at
`/account/mfa/setup`.

## 3. Locked out by too many failed attempts

Lockout is a timed backoff, so waiting clears it. To clear it at once, remove the throttle rows. This
removes the backoff for every account and address, so do it only during a recovery:

```sql
DELETE FROM AuthLoginThrottles;
```

## 4. Authenticator and recovery codes are both lost

Turn two-factor off for that one account. The admin then signs in with the password alone. If
`auth.Mfa.RequireForAdmin` is on, the next sign-in sends them to `/account/mfa/setup` to enrol again.

```sql
DECLARE @u uniqueidentifier = (SELECT Id FROM AuthUsers WHERE NormalizedUserName = UPPER('<username>'));
DELETE FROM AuthRecoveryCodes WHERE UserId = @u;
DELETE FROM AuthUserMfa WHERE UserId = @u;
UPDATE AuthUsers SET MfaEnabled = 0, SecurityStamp = LOWER(REPLACE(CONVERT(varchar(36), NEWID()), '-', ''))
WHERE Id = @u;   -- the new stamp ends every existing session for that account
```

## 5. The password is lost and no other admin or reset email is available

The hosted site has no SMTP yet, so "Forgot password?" sends nothing there. Instead, give the account a
temporary password in the legacy format the library still accepts. It is rehashed to Argon2id at the
next sign-in, and the user must choose a new password straight away.

1. On a trusted machine, make a long random temporary password and compute its legacy hash. Do not save
   the password to a file or paste it into a ticket.

   ```powershell
   $temp = Read-Host 'Temporary password'   # 16+ characters, random
   [Convert]::ToBase64String([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($temp)))
   ```

2. Put the hash on the account and force a password change:

   ```sql
   UPDATE AuthUsers
   SET PasswordHash = '<base64 hash from step 1>', LegacyHashScheme = 'sha256', PasswordPepperKeyId = NULL,
       MustChangePassword = 1, IsActive = 1,
       SecurityStamp = LOWER(REPLACE(CONVERT(varchar(36), NEWID()), '-', ''))
   WHERE NormalizedUserName = UPPER('<username>');
   ```

3. Sign in with the temporary password. The site sends you to `/account/change-password`, where you set
   the real password. Do this at once: until you sign in, the unsalted temporary hash is the weak point.
4. If two-factor is also lost, do step 4 as well.

## 6. Turning the admin two-factor requirement off for a while

If enrolment itself is broken, an operator can turn the requirement off in Tutor's settings.
Locally, that is MindAttic Vault's Tutor tab, setting `auth` > `Mfa` > `RequireForAdmin`. On the host,
it is the `requireForAdmin` value under `auth` > `mfa` in `C:\home\data\MindAtticLocal\Tutor\settings.json`
(edit it with the App Service's Kudu console). Do not set it as an App Service setting instead: any
`MindAttic__Settings__Tutor__*` setting makes Tutor read all its settings from App Service configuration
and ignore that file. Restart the site after the change, and turn the requirement back on once the admin
has enrolled.

## 7. Nothing else works: the empty-table bootstrap

`AuthBootstrapper` seeds an `admin` account with the Vault secret `Security:bootstrap-token`, but only
when the `AuthUsers` table is empty. Emptying the table deletes every account, so use this only on a
fresh database. Add the token to Key Vault and reference it from an App Service setting named like the
existing `MindAttic__Vault__Security__pepperv1` one, restart, sign in as `admin`, change the password, then
remove the setting and rotate the secret.

## After any break-glass step

- Check the sign-in audit table (`AuthAuditLog`) for the recovery and for anything unexpected before it.
- If the lockout came from an attack, rotate the affected passwords and look at the sign-in rate limits
  in Tutor's settings (Usage limits group).
- Write down what happened and which step was used.
