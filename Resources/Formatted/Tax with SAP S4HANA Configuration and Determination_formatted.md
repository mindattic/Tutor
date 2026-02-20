---
title: Tax with SAP S4HANA Configuration and Determination
source: Tax with SAP S4HANA Configuration and Determination.txt
formatted: 2026-02-20T06:02:07.5813871Z
---

## SAP PRESS Overview

**SAP PRESS** is a joint initiative of **SAP** and **Rheinwerk Publishing**. The **know-how offered by SAP specialists** combined with the expertise of Rheinwerk Publishing provides readers with expert books in the field. 

**SAP PRESS** features **first-hand information**, **expert advice**, and provides useful skills for professional decision-making. It offers a variety of books on **technical** and **business-related topics** for the **SAP user**.

For further information, visit: [http://www.sap-press.com](http://www.sap-press.com)

---

## Featured Books on SAP S/4HANA

- **Jawad Akhtar**  
  *Business Partners in SAP S/4HANA: The Comprehensive Guide to Customer-Vendor Integration*  
  2022, 353 pages, hardcover and e-book  
  [www.sap-press.com/5468](http://www.sap-press.com/5468)

- **Aylin Korkmaz**  
  *Financial Reporting with SAP S/4HANA*  
  2022, 707 pages, hardcover and e-book  
  [www.sap-press.com/5416](http://www.sap-press.com/5416)

- **Stoil Jotev**  
  *Configuring SAP S/4HANA Finance (2nd Edition)*  
  2021, 738 pages, hardcover and e-book  
  [www.sap-press.com/5361](http://www.sap-press.com/5361)

- **Christian van Helfteren**  
  *Configuring Sales in SAP S/4HANA (2nd Edition)*  
  2022, 905 pages, hardcover and e-book  
  [www.sap-press.com/5401](http://www.sap-press.com/5401)

- **Jawad Akhtar, Martin Murray**  
  *Materials Management with SAP S/4HANA: Business Processes and Configuration (2nd Edition)*  
  2020, 939 pages, hardcover and e-book  
  [www.sap-press.com/5132](http://www.sap-press.com/5132)

- **Michael Fuhr, Dirk Heyne, Nadine Teichelmann, Jan Walter**  
  *Tax with SAP S/4HANA®: Configuration and Determination*

---

## Preface

Certain events are annual calendar staples—**birthdays**, **anniversaries**, and **holidays**—year in and year out. Despite good intentions, preparation is often last-minute. For example, I find myself searching frantically for the perfect birthday gift and paying extra for express shipping.

Another annual guarantee is **taxes**. In the US, **Tax Day** rolls around every April, marking the deadline for income tax returns to the federal government. This event is one I’m always prepared for since last-minute tax filing causes serious headaches.

---

In business, **taxes** are more than an event on the calendar. Managing tax data means configuring **indirect** and **direct taxes**, fulfilling **reporting requirements**, and performing monitoring activities. The tax function touches data throughout your **SAP S/4HANA system**—which is what this book covers.

This book provides essential information for **managing tax with SAP S/4HANA**, ensuring your business is prepared for **year-round tax compliance.**

---

Your feedback on *Tax with SAP S/4HANA: Configuration and Determination* is invaluable. Please contact me with any praise or criticism. Thank you for choosing **SAP PRESS**!

**Megan Fuerst**  
Editor, SAP PRESS  
meganf@rheinwerk-publishing.com  
[www.sap-press.com](http://www.sap-press.com)

---

## Notes on Usage

This e-book is protected by **copyright**. By purchasing, you agree to the copyright terms, allowing **personal use** only. You may print and copy for personal use but sharing or distributing electronically or physically to others is **not permitted**, nor is making it available online or on company networks.

Refer to the **Legal Notes** section for detailed conditions.

---

This copy contains a **digital watermark** identifying the user:  
Copy No. rwq3-ifa2-d8x7-hb6p  
Personal use of **Yun Kyoung Oh**  
ohleo199@gmail.com

---

## Imprint

This e-book publication involved:  

- Editor: **Megan Fuerst**  
- Acquisitions Editor: **Emily Nicholls**  
- Copyeditor: **Julie McNamee**  
- Cover Design: **Graham Geary**  
- Photo Credit: iStockphoto: 1200200147/© **kontekbrothers**  
- Layout Design: **Vera Brauner**  
- E-Book Production: Graham Geary  
- Typesetting E-Book: **SatzPro, Germany**

We appreciate your feedback and invite you to read the **Service Pages** for contact information.

---

The Library of Congress cataloged the printed edition as follows:

- **Authors:** Michael Fuhr, Dirk Heyne, Nadine Teichelmann, Jan Walter  
- **Title:** Tax with SAP S/4HANA: Configuration and Determination  
- 1st Edition, Boston (MA): Rheinwerk Publishing, 2022  
- Includes index  
- Identifiers:  
  - LCCN: 2022024031  
  - ISBN: 9781493222452 (hardcover)  
  - ISBN: 9781493222469 (ebook)  
- Subjects: SAP HANA (Electronic resource), Taxpayer compliance, Tax administration and procedure  
- Classification: LCC HJ2319 .F84 2022 | DDC 336.2/91--dc23/eng/20220708  
- LC record: [https://lccn.loc.gov/2022024031](https://lccn.loc.gov/2022024031)

---

## ISBNs

- **978-1-4932-2245-2** (print)  
- **978-1-4932-2246-9** (e-book)  
- **978-1-4932-2247-6** (print and e-book)

© 2022 by **Rheinwerk Publishing, Inc., Boston (MA)**  
1st edition 2022

---

## Contents

- **Preface** ....................................................................................................................................................... 13  

### 1 Introduction to Taxes with SAP S/4HANA .......................................................................................... 17

- 1.1 The Tax Function .................................................................................................................... 17  
  - 1.1.1 SAP S/4HANA as a Unique Occasion for Tax Functions ....................................... 17  
  - 1.1.2 Challenges and Requirements ................................................................................. 18  

- 1.2 The Tax Operating Model and SAP S/4HANA ................................................................. 22  
  - 1.2.1 IT Infrastructure and Architecture ............................................................................ 23  
  - 1.2.2 Tax Data Management ............................................................................................... 24  
  - 1.2.3 Tax Processes ............................................................................................................... 26  
  - 1.2.4 Tax Talents .................................................................................................................... 27  
  - 1.2.5 Governance and Organization ................................................................................... 28  
  - 1.2.6 Tax Business Case ..................................................................................................... 29  

- 1.3 Taxes with SAP S/4HANA ...................................................................................................... 30  
  - 1.3.1 What’s New? ............................................................................................................... 30  
  - 1.3.2 SAP Solutions for Global Tax Management ........................................................... 36  

- 1.4 Getting Started with SAP S/4HANA .................................................................................... 47  
  - 1.4.1 Deployment Alternatives ......................................................................................... 47  
  - 1.4.2 Migration Strategies ................................................................................................. 48  
  - 1.4.3 SAP Activate Project Methodology ........................................................................ 51  

- 1.5 End-to-End Processes ........................................................................................................... 55  
  - 1.5.1 Tax Processes ............................................................................................................... 55  
  - 1.5.2 Business Processes ................................................................................................... 58  

- 1.6 Summary ................................................................................................................................... 60

### 2 Indirect and Direct Tax Requirements .............................................................................................. 61

- 2.1 Indirect Tax ............................................................................................................................... 61  
  - 2.1.1 Global Indirect Tax Systems .................................................................................. 61  
  - 2.1.2 Taxable Events .......................................................................................................... 68  
  - 2.1.3 Tax Exemptions ........................................................................................................ 70  
  - 2.1.4 Grouping .................................................................................................................... 72  
  - 2.1.5 Transaction Types .................................................................................................... 74  
  - 2.1.6 Standard Reporting Requirements ........................................................................ 76  
  - 2.1.7 Digital Reporting Requirements ............................................................................ 80  

- 2.2 Direct Taxes ............................................................................................................................ 86  
  - 2.2.1 Base Erosion and Profit Shifting ............................................................................ 86  
  - 2.2.2 Base Erosion and Profit Shifting 2.0 ................................................................. 88  
  - 2.2.3 Standard Audit File for Tax .................................................................................... 91  
  - 2.2.4 Financial Reporting Standards ............................................................................. 93  
  - 2.2.5 Tax-Relevant Reporting Requirements ............................................................. 93  

- 2.3 Summary ................................................................................................................................... 95

### 3 Basic Settings in SAP S/4HANA ........................................................................................................... 97

- 3.1 Financial Accounting ............................................................................................................. 97  
  - 3.1.1 Tax Codes ................................................................................................................... 97  
  - 3.1.2 Tax Accounts ............................................................................................................. 116  
  - 3.1.3 Plants Abroad ........................................................................................................... 119  
  - 3.1.4 Exchange Rates ....................................................................................................... 126  

- 3.2 Sales and Distribution and Materials Management ..................................................... 133  
  - 3.2.1 Tax Classifications .................................................................................................. 133  
  - 3.2.2 Tax Identification Numbers ................................................................................ 145  
  - 3.2.3 Tax Determination with Pricing ............................................................................ 156  

- 3.3 Integration of SAP Modules in Tax Determination ................................................... 160  
  - 3.3.1 Materials Management and Financial Accounting .......................................... 160  
  - 3.3.2 Sales and Distribution and Financial Accounting ............................................ 163  

- 3.4 Smart Forms ............................................................................................................................. 170

- 3.5 Summary ...................................................................................................................................

## 4 Indirect Tax Determination in SAP S/4HANA

### 4.1 Condition Logic in SAP ...................................................................................................... 175

#### 4.1.1 Price Calculation Procedure ................................................................................ 176

#### 4.1.2 Condition Types ...................................................................................................... 181

#### 4.1.3 Access Sequence ..................................................................................................... 184

#### 4.1.4 Condition Tables ..................................................................................................... 187

#### 4.1.5 Access Requirements ............................................................................................ 197

### 4.2 Indirect Tax Determination in Purchasing ................................................................. 198

#### 4.2.1 Electronic Data Interchange versus Purchase Info Records ..................... 198

#### 4.2.2 Purchase Order ........................................................................................................ 200

#### 4.2.3 Invoice Posting ........................................................................................................ 201

#### 4.2.4 Customization Scenarios ..................................................................................... 202

#### 4.2.5 Solutions for Procurement .................................................................................. 204

### 4.3 Indirect Tax Determination in Sales .............................................................................. 208

#### 4.3.1 Sales Order ................................................................................................................ 208

#### 4.3.2 Invoice ........................................................................................................................ 212

#### 4.3.3 Customization Scenarios ..................................................................................... 215

### 4.4 Special Indirect Tax Transactions ................................................................................... 217

#### 4.4.1 Chain Transactions/Drop Shipments .............................................................. 217

#### 4.4.2 Internal Stock Transfers ....................................................................................... 227

#### 4.4.3 Special Payment Processes .................................................................................. 235

### 4.5 Summary ................................................................................................................................... 249

---

## 5 Indirect Tax Engines and Add-Ons

### 5.1 SAP Condition Logic .............................................................................................................. 251

### 5.2 Add-Ons ..................................................................................................................................... 253

### 5.3 Tax Service with SAP ............................................................................................................ 253

### 5.4 External Tax Engines ............................................................................................................ 256

### 5.5 Decision Support for Vendor Selection ........................................................................ 258

### 5.6 Summary ................................................................................................................................... 260

---

## 6 Custom Coding for Indirect Taxes

### 6.1 ABAP Basics for Indirect Tax ............................................................................................. 262

#### 6.1.1 User Exits, Customer Exits, and Business Add-Ins ....................................... 262

#### 6.1.2 Objects ....................................................................................................................... 263

#### 6.1.3 Includes, Functions, Subroutines, and Programs ......................................... 264

#### 6.1.4 ABAP Statements .................................................................................................... 265

### 6.2 User Exits for Tax Determination in Sales .................................................................. 266

#### 6.2.1 Sales Orders .............................................................................................................. 267

#### 6.2.2 Invoices ...................................................................................................................... 274

#### 6.2.3 Additional Enhancements ................................................................................... 276

### 6.3 Customer Exits for Tax Determination in Purchasing ........................................... 280

#### 6.3.1 Identifying Customer Exits .................................................................................. 281

#### 6.3.2 EXIT_SAPLMEKO_001 ........................................................................................... 284

#### 6.3.3 EXIT_SAPLMEKO_002 ........................................................................................... 287

#### 6.3.4 Additional Enhancements ................................................................................... 288

### 6.4 Summary ................................................................................................................................... 289

---

## 7 Direct Taxes in SAP S/4HANA

### 7.1 Direct Tax Basics .................................................................................................................... 291

#### 7.1.1 End-to-End Process for Direct Taxes ................................................................ 292

#### 7.1.2 Direct Tax Organizational Structures and Master Data ............................ 294

### 7.2 Organizational Structures ................................................................................................. 295

#### 7.2.1 Ledger ......................................................................................................................... 295

#### 7.2.2 Company Code ........................................................................................................ 310

#### 7.2.3 Chart of Accounts ................................................................................................... 312

#### 7.2.4 Withholding Tax ..................................................................................................... 316

### 7.3 Master Data ............................................................................................................................. 317

#### 7.3.1 Business Partner ..................................................................................................... 318

#### 7.3.2 General Ledger Account ....................................................................................... 319

#### 7.3.3 Asset ........................................................................................................................... 321

### 7.4 Tax Tagging .............................................................................................................................. 322

### 7.5 Direct Tax Data Analytics and Monitoring ................................................................. 326

#### 7.5.1 SAP Tax Compliance .............................................................................................. 328

#### 7.5.2 Embedded Analytics .............................................................................................. 329

#### 7.5.3 Comparative Review ............................................................................................. 331

### 7.6 Direct Tax Filing and Reporting ....................................................................................... 332

#### 7.6.1 SAP Profitability and Performance Management ....................................... 333

#### 7.6.2 SAP Document and Reporting Compliance ................................................... 337

### 7.7 Direct Tax Planning with SAP Analytics Cloud ......................................................... 340

#### 7.7.1 Direct Tax Planning Scenarios ............................................................................ 341

#### 7.7.2 Map Fiscal Application Scenarios ...................................................................... 342

#### 7.7.3 Implement Tax Scenarios .................................................................................... 344

### 7.8 Summary ................................................................................................................................... 347

---

## 8 Managing Transfer Prices in SAP S/4HANA

### 8.1 Operational Transfer Pricing ............................................................................................ 349

#### 8.1.1 Tax Transfer Prices ................................................................................................. 350

#### 8.1.2 Management Transfer Prices ............................................................................. 350

#### 8.1.3 Operational Transfer Prices ................................................................................ 350

#### 8.1.4 Transfer Pricing Process ....................................................................................... 351

#### 8.1.5 Challenges and Requirements ........................................................................... 355

### 8.2 Transfer Price Calculation .................................................................................................. 357

#### 8.2.1 Cost-Plus Method in SAP S/4HANA .................................................................. 358

#### 8.2.2 Resale-Minus Method in SAP S/4HANA .......................................................... 361

#### 8.2.3 SAP Profitability and Performance Management ....................................... 363

### 8.3 Transfer Price Determination ........................................................................................... 364

### 8.4 Transfer Price Monitoring and Adjustments ............................................................. 364

#### 8.4.1 Extend Universal Journal ..................................................................................... 365

#### 8.4.2 Derivation and Validation ................................................................................... 368

#### 8.4.3 Indirect Cost Allocation Limitations ................................................................. 369

#### 8.4.4 SAP Profitability and Performance Management ....................................... 369

### 8.5 Transfer Price Reporting and Simulation .................................................................... 371

### 8.6 Transfer Price Testing and Compliance ....................................................................... 373

#### 8.6.1 Transaction Matrix ................................................................................................ 374

#### 8.6.2 Country-by-Country Reporting .......................................................................... 375

### 8.7 Summary ................................................................................................................................... 377

---

## 9 Tax Reporting in SAP S/4HANA

### 9.1 Worldwide Tax Reporting Requirements ................................................................... 379

#### 9.1.1 The Five Fingers of Tax Administration .......................................................... 380

#### 9.1.2 Maturity of Tax Reporting Requirements ...................................................... 381

#### 9.1.3 Tax Record Evidence Requirements ................................................................. 383

#### 9.1.4 Global Tax Reporting Compliance Models ..................................................... 384

### 9.2 Standardized Tax Reporting in SAP ............................................................................... 385

#### 9.2.1 Standard Report for Value-Added Tax Returns ............................................

## Contents

- **9.2.2** Standard Reports for EC Sales List .................................................................... 394
- **9.3** SAP Document and Reporting Compliance for SAP S/4HANA .......................... 398
  - **9.3.1** Setting Up Compliance Reporting .................................................................... 398
  - **9.3.2** Setting Up EC Sales Reporting ........................................................................... 407
  - **9.3.3** Electronic Document Processing ....................................................................... 408
- **9.4** Summary .......................................................................................................................... 412
- **10** Tax Monitoring in SAP S/4HANA .............................................................................. 413
  - **10.1** Why Monitor Tax? .................................................................................................. 413
    - **10.1.1** Compliance Risks .......................................................................................... 414
    - **10.1.2** Efficiency Improvements ............................................................................. 415
    - **10.1.3** Cash Flow Optimization ................................................................................ 416
  - **10.2** Tax Monitoring Requirements ............................................................................... 417
  - **10.3** Tax Control Framework ........................................................................................ 419
  - **10.4** SAP Solutions for Indirect Tax Monitoring .......................................................... 424
    - **10.4.1** SAP S/4HANA Embedded Analytics .......................................................... 424
    - **10.4.2** SAP Analytics Cloud .................................................................................... 425
    - **10.4.3** SAP BW/4HANA ........................................................................................... 426
    - **10.4.4** SAP Tax Compliance .................................................................................. 426
  - **10.5** Monitoring with SAP Tax Compliance ................................................................. 429
  - **10.6** Summary .................................................................................................................. 438

- **Appendices** .......................................................................................................................... 439
  - **A** Europe ....................................................................................................................... 441
  - **B** India ........................................................................................................................... 461
  - **C** North America ............................................................................................................. 465
  - **D** The Authors ............................................................................................................... 473

- **Index** ...................................................................................................................................... 475

- **Service Pages** ......................................................................................................................... I

- **Legal Notes** ............................................................................................................................ I


## Preface

Welcome to this comprehensive guide to **tax in SAP S/4HANA**. In these pages, you’ll find information on **indirect and direct tax**, including **fundamental concepts**, step-by-step instructions, and best practices.

### Objective of This Book

We wrote this book to support you with a practical guideline to prepare, realize, and succeed with your **SAP S/4HANA implementation project** and during your ongoing deployment of **SAP S/4HANA** from the tax perspective. Our intention was to minimize theoretical content to the necessary extent to have a certain comfort about the current challenges and opportunities that can be addressed during **SAP S/4HANA implementations** or deployments.

The major part of this book comprises **practical solution proposals** to be applied to your projects. During development, our experiences, best practices, and customized client solutions formed part of the content.

Solution proposals aren’t intended to be copied without proper IT process/project steps (**analysis, design, realization, testing, go-live**), but we hope you'll be inspired to pursue your own solutions by the proposals and experiences captured here.

### Important Note

We cannot take responsibility for the correct deployment or application to any other **ERP system** than our **SAP S/4HANA test system**, which was the basis for this book.

As a consequence of the high demand for **tax talents** and the emerging need for **digital tax solutions**, tax communities seek higher influence on **tax legislation, administration, reporting, determination**, and **compliance management**.

Cross-company and cross-industry teams are vital resources for **thought leadership** and the development of **value-added solutions**. We are happy to see more tax communities initiating important discussions and actions toward the new normal of tax.

The **strategic positioning** in the tax sector is a critical focus area for corporations and professional services firms to keep pace with ongoing developments.

### Personal Copy for Yun Kyoung Oh, ohleo199@gmail.com

### Continued Purpose of the Book

With this book, we want to contribute to a growing community of **tax professionals** focused on successfully running **value-added tax (VAT) technology solutions** in SAP S/4HANA landscapes.

We aim for you to concentrate on your business, so we wrote the book in a practical style with many examples and guided solution proposals.

Finally, we hope many readers are inspired to start or continue a career in **tax technology** and join our community of practitioners in tax functions, advisory practices, and tax solution development.

For those already on this path, this book can help refine your skills.

We can’t wait to see the future of the community and creators of next-generation tax topics and solutions.

### Readers of This Book

All interested in **tax technology** and curious about solutions for **typical tax challenges** in SAP S/4HANA will find inspiration, information, examples, and motivation.

- **Tax functions**, tax practitioners, and **SAP S/4HANA project managers** will find reasonable approaches for their projects.
- **IT colleagues, tax process experts**, and personnel in **accounting, controlling, and finance** will benefit for tax-specific topics.
- **Solution providers** can use this book’s many use cases to include in their offerings.
- **Newcomers** in tax (tech) teams can use this book as a compendium for personal growth and career development.

Please share your reading experience and help us improve. We welcome all feedback!

## Structure of This Book

This book is divided into the following 10 chapters:

### Chapter 1: Introduction to Taxes with SAP S/4HANA

- Introduces the **tax function** in SAP S/4HANA
- Describes the **tax operating model**
- Explores SAP solutions for **global tax management**
- Provides high-level info on **deployment, migration**, and **SAP Activate methodology**
- Looks at relevant **end-to-end processes** for tax

### Chapter 2: Indirect and Direct Tax Requirements

- Introduces **indirect** and **direct tax** in a general context
- Covers **indirect tax concepts** like tax systems, events, exemptions, and reporting requirements
- Discusses core **direct tax concepts**: **Base Erosion and Profit Shifting (BEPS)**, **Standard Audit File for Tax (SAF-T)**, and reporting

### Chapter 3: Basic Settings in SAP S/4HANA

- Guides you to set up tax-relevant settings in **financial accounting, sales and distribution, and materials management**
- Explains integration of SAP modules in **tax determination**
- Reviews **smart forms** functionality

### Chapter 4: Indirect Tax Determination in SAP S/4HANA

- Step-by-step on indirect tax determination in SAP S/4HANA
- Starts with **condition logic setup**
- Explains indirect tax determination in **purchasing and sales**
- Discusses special **indirect tax transactions**

### Chapter 5: Indirect Tax Engines and Add-Ons

- Brief overview of other **tax engines** and **add-ons** available for indirect tax
- Introduces **SAP's tax service** and **external tax engines**
- Covers **decision support considerations**

### Chapter 6: Custom Coding for Indirect Taxes

- Explains **custom coding** options for indirect taxes
- Introduces fundamental **ABAP concepts**
- Explores **user exits** for tax determination in sales and purchasing

### Chapter 7: Direct Taxes in SAP S/4HANA

- Steps through **direct taxes**
- Covers setup of **organizational structures** and **master data**
- Discusses **direct tax analytics and monitoring**
- Covers **tax filing**, reporting, and **tax planning** with **SAP Analytics Cloud**

### Chapter 8: Managing Transfer Prices in SAP S/4HANA

- Explains **transfer pricing** in the tax context
- Discusses **operational transfer pricing**
- Reviews functions for calculating and determining transfer prices
- Discusses **monitoring, reporting, and testing** options

### Chapter 9: Tax Reporting in SAP S/4HANA

- Discusses **worldwide tax reporting** requirements for direct and indirect taxes
- Explains standardized tax reporting using **key SAP reports**
- Walks through setting up **SAP Document and Reporting Compliance**
- Discusses **electronic document processing**

### Chapter 10: Tax Monitoring in SAP S/4HANA

- Introduces **tax monitoring requirements** and the **tax control framework**
- Explores indirect tax monitoring solutions like:
  - **Embedded analytics**
  - **SAP Analytics Cloud**
  - **SAP BW/4HANA**
  - **SAP Tax Compliance**
- Discusses monitoring with **SAP Tax Compliance** in detail

## Acknowledgments

All authors and contributors are experienced **tax technology advisors** and part of fantastic teams. This big **“Thank you”** goes out to all team members whose input was invaluable.

We had the pleasure to work with many clients, whose challenging projects form the basis of this book. Without these common projects, this book would not exist.

Writing this book reminded us why we chose our profession and why it fulfills us daily with satisfaction to keep on rocking.

## Chapter 1  
# Introduction to Taxes with SAP S/4HANA

To begin our journey into **tax** with **SAP S/4HANA**, we’ll explain how **direct and indirect taxes** work in a global context, as well as how these requirements have been met with **SAP solutions**. This first chapter is about current challenges of **tax functions** and their implications to the **tax target operating model**.

We describe in a nutshell what a **tax target operating model** can look like and discuss in detail the importance of **tax processes** and **tax data management**. Afterward we focus on essential **SAP S/4HANA topics** with a link to taxes, that is, what's new about SAP S/4HANA for tax, and which deployment alternatives and implementation methods are available. Finally, we walk through the **SAP Activate methodology** for tax functions.  

---

## 1.1 The Tax Function

In this section, we’ll provide some initial context around the **tax function**. We’ll see how SAP presents new opportunities in the tax space, and we’ll consider relevant challenges and requirements.  

### 1.1.1 SAP S/4HANA as a Unique Occasion for Tax Functions

Let’s begin with a brief **history of SAP**, as shown in Figure 1.1. SAP introduced **SAP ERP** in **2004** as an upgraded version of the already existing **enterprise resource planning (ERP)** software called **SAP R/3**. Eleven years later, **SAP S/4HANA** was announced in **2015** by SAP, and some of the first adoptions of SAP S/4HANA started in 2015.  

Assuming an ERP software lifecycle takes roughly **10 to 15 years** on average in an enterprise, **ERP software implementation projects** are unique opportunities once in a decade for **tax functions** to integrate tax requirements and connect with most of the tax-relevant stakeholders in the company.  

Typically, business processes managed with the help of the ERP system and outside the ERP system are also under review in such implementation projects and can be subject to further improvements from a **tax perspective** as well. Therefore, we recommend making use of this opportunity from a tax perspective, ensuring there's a **tax seat at the table** of the SAP S/4HANA implementation project and having a budget for internal and external costs related to the tax workstream.  

In Section 1.4.3, we’ll cover the detailed tax agenda during the **SAP Activate method** in an SAP S/4HANA implementation project.  

### SAP Software History (Figure 1.1)

- **1972:** SAP R/2  
- **1992:** Client/Server  
- **2004:** SAP ERP on SAP HANA with SAP Fiori UX  
- **2011:** Suite on SAP HANA  
- **2013:** SAP HANA In-Memory  
- **2015:** SAP S/4HANA Digital Core  
- **2018:** SAP S/4HANA Intelligent ERP with SAP Business Technology Platform (SAP BTP; Previously SAP Cloud Platform)  
- Also includes SAP ERP 6.0, mysap.com  

---

### 1.1.2 Challenges and Requirements

**Tax functions** are currently subject to change driven by **digitization** and more complex tax requirements than ever before. The **fourth industrial revolution** (also known as the **digital revolution** or **Industry 4.0**) affects the tax function by making tax-relevant **processes** and **data digital** and available in real time, at the same time.  

The areas and triggers of the change can be structured in three different parts, which we’ll discuss in the following sections:  

- **Digital tax administration**  
- **Value creation**  
- **Risk management**  

---

### Digital Tax Administration  

One trigger is the **digital tax administration** that is currently driving the digitalization of **tax functions**. Tax administrations across the globe have intensified their demand for data to gain **transparency** and **tax revenue**. Especially **indirect taxes** become more and more popular and an emerging source of revenues for governments.  

**Indirect taxes** used to be the main source of income for jurisdictions. A deviation between the amount of indirect taxes to be collected and the amount actually collected is called a **value-added tax (VAT) gap**.  

One of the main reasons for the demand of higher transparency of tax administrations is to close the **VAT gap**. For example, in the **European Union (EU)**, there is a yearly VAT gap of **134 billion euros**. This amount equals to more than **10%** of the total income of all EU member states (see the study "**VAT Gap in the EU. Report 2021**").  

As a consequence, **tax reporting obligations** and **mandatory e-invoicing** are completely disrupting reporting processes of taxpayers on a global scale. Within the next 5 to 10 years, we’ll see an exponential increase of digital reporting requirements that results in (nearly) global coverage.  

That means there will be fragmented global obligations to be met simultaneously that are running in the IT systems and/or processes of the taxpayers. Due to the growing demand in terms of transparency, the level of digitalization of the tax authorities enormously changed during the past decade.  

Previously, tax administrations relied on nondigital documents to prove the periodical and aggregated tax reporting and statutory accounts. With taxpayers facing more and more digital recordings and digital disruptions, tax authorities also make use of digital measures and therefore force tax functions to keep pace with this development.  

Given current discussions of mandatory e-invoicing on a global scale to improve transparency and fight indirect tax fraud, the role of the tax authorities turned from acting **reactively** to being a driver of the **digitization of taxpayers**.  

Let’s consider a few examples of how tax is being digitized to produce more data and more transparency:  

- **Mandatory e-invoicing**  
  - Systems across Europe with invoice exchange by tax administration (e.g., in Italy since 2019, in France starting July 2024).  
- **Real-time reporting**  
  - Continuous tax reporting in a near real-time frequency (e.g., the **Suministro Inmediato de Información (SII)** in Spain since 2017).  
- **Standard Audit File for Tax (SAF-T)**  
  - Periodic filing of a SAF-T in many European countries (e.g., Poland since 2018, Romania in 2023).  

These requirements lead to an increased amount of data to be filed and to a higher filing frequency and data interrogation. As a consequence, taxpayers need to prepare and monitor the requested data.  

Due to the number of single transactions, **automation** is the only way to manage the demands of the tax authorities and to avoid routine penalties. There is also upside potential as a higher degree of automation throughout the economy leads to more efficiencies.  

The tax reporting process itself becomes, to a large extent, an IT process with a real-time impact on **business processes** and **financial processes** (e.g., treasury). For example, mandatory e-invoicing leads to electronic customer invoices that need to be cleared by the tax authority (given a clearance e-invoicing model applies) before transfer to the customer.  

**Tax-driven invoice requirements** are thereby essential for a running business process to receive cash from the customer and satisfy customer experiences. At the same time, companies can reduce the environmental impact by no longer printing paper invoices.  

---

### Value Creation  

The next trigger is the changing role of the **tax function** within an organization. The role of the tax function today is a **strategic business partner** role that is connected to its stakeholders within and outside the organization.  

In addition to external challenges, internal topics are also driving change within tax functions. Many tax teams are hit by a shortfall of resources and budget restrictions. The time that is available for the allocation to tasks of tax team members is currently subject to change, as shown in Figure 1.2. The percentages are average values that may differ between organizations.  

---

### Tax as Strategic Business Partner (Figure 1.2)

- **Traditional Tax Function Time Allocation:**  
  - **40%** Reporting  
  - **40%** Transaction processing  
  - **10%** Control  
  - **10%** Decision support  
  - **0%** IT Enablement  

- **Value-Driven Tax Function Time Allocation:**  
  - **25%** Reporting  
  - **15%** Transaction processing  
  - **10%** Control  
  - **30%** Decision support  
  - **20%** IT Enablement  

The focus of tax management in organizations should be **value driven**, but there are several challenges the tax function encounters that take up time:  

- Generally, the majority of time (up to **80%**) in a tax function’s day is typically used for **tax reporting** and **transaction processing**.  
- Depending on the number of reporting entities, tax compliance tasks, spreadsheet-based analysis with manual data collection, and limited automation for tax reporting and filing of tax returns can be time consuming.  
- Even user-friendly tax reporting software is only as good as the **ERP/legacy system source data integrity** and quality allows.  
- During transaction processing tasks, the tax function supports the core business of an organization. Without visibility and access to **tax data** with integrated software solutions, communication is based on emails, phone calls, or even handwritten messages.  
- Very often, the tax function is no expert in fixing IT-indicated root causes. Especially with regard to ERP systems, the tax function is used to having only limited access and capacity to engage in IT processes that are necessary for **tax data management**.  

The more time used for reporting and transaction processing, the less time is available for a conceptual change and an enablement of proper tax data management and processing. This can be a frustrating situation.  

Carefully planned and successfully realized tax-integrated **SAP S/4HANA** projects can create momentum to heavily improve the portion of **value-driven decision support** for the business (e.g., **10%–30%**) and the stake of tax in **IT enablement** (up to **20%**).  

Further project results can lead to the following improvements:  

- **One single data source**  
  - One single data source for tax-relevant data available and extractable for self-service reporting.  
- **Automation of compliance and reporting**  
  - Fully automated process for real-time tax reporting and periodic compliance and reporting.  
- **Tax data recording and monitoring**  
  - Enable proper tax data recording, automated tax decisions, and tax data monitoring.  

These improvements are value drivers for the entire organization.  

---

### Risk Management  

Tax **risk management** is one of the major concerns of the tax department. Especially in an environment of business disruption, logistical uncertainties, and a global pandemic, the tax function is in an unprecedented time and in a totally changed working environment.  

Adequate and effective **tax compliance management systems** and **tax control frameworks** can help the tax function identify, valuate, and manage **tax risks**. Especially in Germany and in more and more countries in Western Europe, **tax compliance management systems** are implemented in businesses to improve the procedural situation in case there are identified exceptions from the tax law.  

Tax-integrated **SAP S/4HANA** projects can support the building and improvement of operationalized **tax compliance management systems** to avoid tax-indicated risks such as:  

- Incorrect invoices  
- Overreporting/underreporting of tax  
- Interest and penalty exposure  
- Customs issues that lead to breaks in the supply chain  

## 1.2 The Tax Operating Model and SAP S/4HANA

**Tax functions** must keep pace with developments and adjust their processes, systems, and ways of working. A **tax operating model** is an approach to establish a common vision and strategy for the tax function. Improving isolated parts, such as a single tax type (e.g., **transfer pricing**) technology or process, can result in partial improvements and fragmented success compared to an overall strategy.

An **SAP S/4HANA implementation project** is an appropriate opportunity for tax functions to kick-start or reframe the tax operating model, as all important parts can be designed, adjusted, and supported during the project. 

Figure 1.3 illustrates a possible **tax operating model**, summarizing a holistic approach to manage the most important areas of a target operating model of a tax function.

The tax operating model consists of several major areas:

- **IT infrastructure and architecture** (including tax data management)  
- **Tax processes**  
- **Tax talents**  
- **Governance and organization**

We will also explore the **value drivers** for an overall tax business case as a framework for the tax operating model.

---

### 1.2.1 IT Infrastructure and Architecture

The foundation of a tax operation is an appropriate **IT infrastructure and architecture** for tax purposes. This consists of:

- Systems  
- Database(s)  
- Tools  
- Data  

For some organizations, **SAP S/4HANA** is the single system IT infrastructure. It offers:

- An **ERP environment**  
- An **SAP HANA in-memory database**  
- Tools (e.g., SAP Document and Reporting Compliance)  
- All tax-relevant data  

Most businesses, however, have more complexity, including external legacy systems and interfaces. 

Figure 1.4 shows a possible structure of a data model in the course of a **Central Finance approach**.

The IT landscape workflow in this example:

- **Tax-relevant data** is interfaced and streamlined from recording in legacy systems.  
- Data aggregation happens at the **Central Finance instance**.  
- Data moves to a **central tax data warehouse**.  
- The **application layer** uses the data for tax applications (e.g., tax reporting).

Data must be standardized to enable aggregation and interoperability. For example, **financial accounting data** recorded in two different source systems (SAP ERP and SAP S/4HANA) with different charts of accounts requires:

- Technical mapping between ERP versions  
- Account mapping for correct accounting rules  

After standardization, the data can be further processed in the **central tax data warehouse** for tax reporting. There is a two-way data flow between these layers, allowing new data or changes to be reflected in source systems.

---

### 1.2.2 Tax Data Management

Figure 1.4's structure shows the importance of **tax data management** to ensure data quality meets tax and finance requirements efficiently. 

**Tax data management** involves defining organizational data elements, master data, and transactional data per applicable tax rules arising from business transactions.

#### Figure 1.5 Tax Data Types

- **Organizational Elements**  
  - Define company structure from an application viewpoint  
  - Framework of SAP system (e.g., controlling areas, company code)  
  - Data remains unchanged over time  

- **Master Data**  
  - Information repeatedly used across components  
  - Examples: vendor, cost center, cost elements, activity types  

- **Transactional Data**  
  - Short-term, process-related data assigned to master data  
  - Individual postings  

#### Data Elements Include

- Receipt capture fields (e.g., invoice date, posting date)  
- Company code, business partner, trading partner  
- Transaction types, chart of accounts (tax accounts)  
- Equity, financial assets, tax receivables, tax liabilities  

**Operational tax data management** enables proper processing and use of data in SAP S/4HANA following these steps:

1. SAP data fields from business processes (sales, procurement, logistics, finance) are put into a **tax-relevant context**.
2. SAP data transforms into **tax-relevant information** through tax interpretation.
3. Combining tax-relevant info results in a **tax-relevant transaction** (e.g., sale of goods between independent parties).
4. Tax-relevant transactions categorize automatically into a **transaction type** (e.g., indirect taxes like VAT, direct tax nondeductible expense).
5. **Tax base amounts, tax amounts, exception categories**, and system settings potentials are derived.
6. **Tax requirements** can be defined and managed based on this approach.  

This enables **strategic tax management** (from requirements to tax data) and **operational tax management** (from tax data to requirements).

#### Tax Data Structures in SAP

- SAP tax data follows a data model including **master data** and **transactional data**.
- Use **Transaction SE16** to extract and analyze SAP tables containing tax data.
- The **Data Retention Tool (DART) catalog** in SAP S/4HANA is useful for identifying relevant tables and data fields.
- Tax auditors use DART during audits to access granular tax data.

---

### 1.2.3 Tax Processes

During SAP S/4HANA implementations, tax requirements are collected according to **end-to-end business processes**. Tax functions must understand and define requirements at the **data element level** for all relevant tax types.

Typical end-to-end processes affecting various tax types are shown in Table 1.1.

It is essential to collect all tax requirements along **business** and **tax reporting processes**. During implementations:

- Tax must define or adjust business processes from a tax perspective.  
- Define **user stories** from an operational tax function perspective.  

The **SAP Activate methodology** requires integrating tax requirements during implementation. Tax functions must participate in cross-workstream workshops for requirement gathering, well-prepared with a **data-based approach** supported by tax analysis software and relevant source data.

Scalable solutions exist for:

- Structuring and analyzing tax data  
- Analyzing sales, distribution, materials management, and financial accounting data  
- Identifying tax-relevant transactions and mapping them to tax codes (indirect taxes) or accounts (direct taxes, WHT)  

Historical data supports validation, clarification of exceptions, and more efficient workshops.

#### Table 1.1 End-to-End Processes and Tax Relevance

| End-to-End Process  | Indirect Tax | Direct Tax | Transfer Pricing | Withholding Tax (WHT) |
|--------------------|--------------|------------|------------------|-----------------------|
| Record-to-report    | X            | X          | X                | X                     |
| Order-to-cash      | X            | X          |                  |                       |
| Procure-to-pay     | X            | X          | X                | X                     |
| Plan-to-ship       | X            |            |                  |                       |

---

### RISE with SAP

**RISE with SAP** is a subscription-based methodology guiding customers to **SAP S/4HANA** and **SAP S/4HANA Cloud**.

Key features include:

- Predefined structures and processes for implementation projects  
- Integrated deployment after go-live  
- Use of **tax type lifecycles** to ensure all tax requirements are collected properly along business processes

## 1.2.3 Indirect Tax Lifecycle

**Figure 1.6** shows a typical **tax type lifecycle** from the **indirect tax perspective** that covers a strategic dimension such as **operating model effectiveness**, **indirect tax determination**, **indirect tax monitoring**, **indirect tax reporting**, and **indirect tax audit/controversy**. This structure can be applied to all tax types and business processes that are part of the **tax operating model**.

---

## 1.2.4 Tax Talents

A **tax operating model** requires the right **competencies** in the tax function. Establishing appropriate **working conditions** is key to attracting and retaining **tax talents**. Based on the **company culture**, the tax function needs to define and establish an environment that **motivates** and **challenges** team members.

The tax function must create a place for each talent to bring unique **competencies** and develop a career according to expectations of both the team member and the company. While working conditions are very individual and not covered in this book, the generally required **competencies** and **profiles** include:

- **Tax technology hybrids**  
Experienced hybrids that combine **tax backgrounds**, **technology experience** (e.g., data analytics, SAP user knowledge), and **project management skills**.

- **Tax advisors**  
Specialists such as **transfer pricing experts**, **transaction specialists**, and **real estate specialists**.

- **IT experts**  
Experts with strong **coding skills**, database skills in **SQL**, and **SAP HANA** knowledge.

- **Process experts**  
Experts with operational expertise, for example, in **logistics**.

- **Tax compliance officers**  
Manage the **tax control framework**, including workflows, mitigation tasks, and improvements.

- **Leaders**  
Possess **leadership competencies** to support integration and transformation of **tax technology topics** as well as team building and communication with tax stakeholders.

Given the challenging market for these profiles, having the right people is a **success factor**. Along with intelligent tax IT software and hardware solutions, a **good team** within the tax function and stakeholders is essential.

---

## 1.2.5 Governance and Organization

The area of **governance and organization** as part of the tax operating model covers guidance on managing tasks regarding **SAP S/4HANA implementation projects** by the responsible tax function leadership. Main governance topics include:

- Clear and focused articulation of **tax needs** aligned with overall goals such as **maximum value contribution** to the business.

- Checking for **optimization potential** before the SAP S/4HANA project to ensure the best starting point for transformation.

- Reviewing and updating the **tax control framework**, including documentation and controls.

- Identifying and allocating **tax resources** into the SAP S/4HANA project for enhanced knowledge, skills, and project impact.

Organizational questions include:

- How will SAP S/4HANA implementation impact the **finance operating model?**  
- What are the changing **roles and responsibilities** between finance and tax?  
- What skills are needed to integrate SAP S/4HANA processes and functionalities into tax functions?  
- What are the **off- or near-shore opportunities** for tax activities, such as tax reporting processes?  

---

## 1.2.6 Tax Business Case

The **tax business case** defines the underlying principles for a **tax operating model**. Its goal is to establish a **value-driven environment** for all parts of the tax operating model.

**Figure 1.7** shows three key areas of the business case:

- **Risk management**  
- **Cash potentials**  
- **Efficiencies**

Depending on organizational maturity, there are various upside potentials for a **value-driven business case**.

### Table 1.2 Business Calculation Example

| Aspect                | Investment Side Costs                   | Savings Side (Value per Year)                                         |
|-----------------------|---------------------------------------|----------------------------------------------------------------------|
| One-time costs        | - Software license costs (700k)        | - Compliance cost savings on 55,000 invoices (450k)                  |
|                       | - Implementation including data checks and workflow design (300k) | - Efficiency gains from saved time on mitigation tasks (150k)        |
| Recurring costs       | - Yearly software license (70k)        | - Cash opportunities via earlier VAT deduction and foreign VAT refund (80k) |
|                       | - Yearly license for data check repository (50k) |                                                                      |

The **amortization duration** for the one-time amount is between **one and two years**. Recurring costs are offset by yearly savings, although savings may flatten due to **learning effects**. This example shows how to calculate the **value contribution** of the **tax function**.

---

## 1.3 Taxes with SAP S/4HANA

This section introduces new functionalities in **SAP S/4HANA for tax**, covering key features like the **Universal Journal** and SAP solutions for **global tax management**.

---

### 1.3.1 What’s New?

Most innovations in SAP S/4HANA have **general impacts on taxes**, with some **tax-oriented functionalities** requested by users.

#### Simplification List

The **SAP S/4HANA simplification list** is a reference document detailing SAP S/4HANA advancements of familiar SAP transactions. Some transactions now have **SAP Fiori apps**, while others are replaced or suspended.

- Available at: [http://s-prs.co/v549500](http://s-prs.co/v549500)  
- Covers multiple SAP S/4HANA releases and is continuously maintained.  
- Includes components, master data, additional info, and sector solutions if relevant.  

The list provides new or changed functionalities similar to **SAP Notes**, helping in SAP S/4HANA project planning especially for tax data management setup.

**Example:**  
SAP S/4HANA **Intrastat functionality** shifted international trade processes to **SAP Global Trade Services (SAP GTS)**, an additional separately licensed module, with selected foreign trade functions provided in SAP S/4HANA core. The simplification list documents which Intrastat transactions were removed, critical to implementation projects.

#### SAP Fiori User Interface

**SAP Fiori 3** offers a new, user-friendly interface enhancing SAP S/4HANA usability:

- Provides solutions for integrated **internal and external data processing** and evaluation.
- Tax functionalities accessible via **SAP Fiori tiles**.
- Supports mobile device usage and **role-based functions**, facilitating remote work and home office.
- Examples include the **Run Compliance Reports** app (advanced compliance reporting via SAP Document and Reporting Compliance).
- Users can create custom SAP Fiori apps and tiles via the **SAP Fiori launchpad content manager**, which allows exploration and adaptation of business catalogs.
- The **SAP Fiori launchpad application manager** manages application and technical catalogs.
- The **Manage Launchpad Spaces and Pages** app enables page layout structuring with drag and drop.
- **SAP CoPilot** offers multilingual voice input and task management support.

**Figure 1.8** illustrates SAP Fiori tiles.

#### Universal Journal

The **SAP HANA in-memory database technology** and revised **finance architecture** in SAP S/4HANA enable the **Universal Journal**. This combines data from previously separate sources (**financial accounting** and **management accounting**) into a single data table.

- Individual **line items** form the basis for the balance sheet instead of separate balances.
- The Universal Journal stores all postings from **internal** and **external accounting** in one SAP table.
- Accounting and controlling data are combined in table **ACDOCA**, which stands for **Accounting Document Actuals**.
- This table amalgamates document information from **bookkeeping**, **asset accounting**, **cost accounting**, and the **Material Ledger**.
- **Cost element** and **financial accounting accounts** are synchronized, meaning each controlling posting generates a financial accounting document.

**Figure 1.9** shows the Universal Journal structure.

## Harmonization and Enrichment of Data in SAP S/4HANA

Due to the **harmonization** and **enrichment** of data, there is no need to reconcile **financial accounting** with the **subledgers**:

- **Financial accounting (general ledger)**
- **Profitability analysis**
- **Controlling**
- **Asset accounting**
- **Material Ledger**

The original tables remain as **database views** (e.g., tables **BSEG** or **COEP**), though they are not primarily used for database storage. Standard reports and transactions used previously in **SAP ERP** continue to be applicable. This is important for **tax reporting** and **data provisioning**, particularly data from table **BSET** or **BSEG**.

The tax data for the accounting document remains updated in table **BSET**. Besides table **ACDOCA**, table **BKPF** (header data) also remains in place. Additional fields (customer-specific) can be implemented for special tax purposes to enhance reporting capabilities.

Because trial balances have been omitted, it is possible to navigate from the balance sheet down to the individual items and view detailed information for **tax validations**.

---

## Material Ledger

Information related to **materials management** is stored in the **Material Ledger**. Material movements are managed in table **MATDOC**. The header data from the material document **MKPF** and line item data from table **MSEG** were transferred to table **MATDOC**.

You can choose between different Material Ledger types and currency types. Key capabilities of the Material Ledger include:

- Use of **parallel currencies** (up to three for stock mapping)
- Running **parallel evaluations** (e.g., for transfer pricing)
- Optionally using **actual cost** for externally procured and internally created materials (instead of moving average price or standard price)

Overall, the Material Ledger increases the **transparency** of all material movements and the company’s stock.

---

## Business Partner Concept in SAP S/4HANA

A major innovation in SAP S/4HANA is the **business partner concept**, introduced at the financial accounting level. This concept enables **master data unification**, resulting in:

- Fewer **data redundancies**
- Less **maintenance effort**
- Higher **data quality**

Every organization has different relationships, including:

- **Business-to-business (B2B)**
- **Business-to-consumer (B2C)**
- **Business-to-government (B2G)**
- **Business-to-tax (B2T)**

A business partner may be both a **customer** and a **supplier**. The business partner concept replaces parts of the vendor and customer master records maintained separately previously.

A unique **business partner number** is assigned irrespective of classification (customer/vendor). The **central business partner** is the leading master data object, working with **customers** and **accounts payable** in the background. Relevant SAP table names usually start with **BP**.

**Synchronization** of business partner data occurs only in one direction: from business partners to customer/supplier master data.

### Tax Data Management in Business Partner

- **General data** can be used across several business partner roles.
- Multiple **addresses** for a business partner can be maintained.
- Central maintenance benefits different SAP components by assigning different roles.

In the **Billing view**, the customer's tax classification can be set up. For instance, the **Output Tax table** may show a customer liable to taxes identified by a tax classification code (e.g., code 1 for Germany).

---

## Transaction BP

- **Transaction BP** is used to maintain all business partner data.
- It replaces former transactions such as **FK03** (vendor display) and others related to customers, debtors, suppliers, and creditors.
- New business partners can be created as **persons**, **organizations**, or **groups**.

---

## 1.3.2 SAP Solutions for Global Tax Management

**SAP solutions for global tax management** help companies handle **indirect tax compliance**, efficiency, and cost control based on SAP technology. They include:

- **Tax determination**
- **Calculation**
- **Reporting**
- **Compliance**

SAP S/4HANA implementation projects are ideal for integrating tax solutions with core business processes to meet evolving global tax requirements.

### Tax Capabilities

Tax functions ensure **tax compliance** and support **tax planning** with a value-driven approach emphasizing:

- **Cost efficiency**
- **Cash optimization**

SAP supports these functions by enabling:

- Automated and compliant tax reporting for **direct** and **indirect taxes**
- Rule-based **data quality** and **tax risk management**
- Automated **tax depreciation** for direct tax purposes
- Improved **intercompany reconciliation** for group indirect taxes
- Real-time **insights into tax-relevant data**

### SAP S/4HANA Tax Capabilities

- **Automated and compliant tax reporting**
- **Proactive transfer price management**
- **Rule-based data quality and tax risk management system**
- Real-time segmented **P&L** and **Balance Sheet reporting**
- Enhanced **withholding tax calculations**
- Automated **customs controls and calculations**
- **Digital tax administration integration**
- **Predictive tax planning**

---

## SAP Solutions for Global Tax Management Overview

SAP solutions are grouped into:

- **Core functionalities** (digital core)
- **Apps** based on SAP technology (e.g., SAP Tax Compliance, SAP Document and Reporting Compliance)
- **External solutions** with varying levels of integration

### Digital Core

Includes master data and tax determination based on **condition logic** in sales & distribution, materials management, and financial accounting.

### SAP Apps

High integration into SAP core but not part of the core itself.

### External Solutions

Wide range of integration levels, from no integration (using SAP data only) to fully integrated add-ins or bolt-ins. Integration can use API connections, remote function calls (RFCs), or web services.

---

## Tax Determination in SAP S/4HANA

Tax determination is supported by:

- **Internal built-in tax calculation**
- **External tax calculation engines** connected to SAP

These solutions address different jurisdictional requirements.

---

## External Solutions for SAP

- For both, **SAP native configuration**, including **SAP master data**, needs to be set up accordingly.
- SAP’s internal built-in tax calculations use the **condition technique** based on **customer and material master data** and other parameters.

## Solutions Built to Integrate with SAP

### Transformation

- Transform **tax operations** through SAP and SAP-enabled applications.

### Core SAP

- Configuration of **SAP Digital Core Solutions** within SAP.

### Digital Core SAP Apps

- Configuration of SAP applications and solutions on SAP technology.

### Managed Services

- Solutions and applications leveraging **SAP Business Technology Platform**.

---

## 1.3 Taxes with SAP S/4HANA

- Tax determination automation applies to **customer orders/invoices** or **purchase orders** based on:
  - **Customer/material master data**
  - **Condition technique**
- For **vendor invoices**, automation is based on:
  - Combination of **workflow solutions** and latest technologies
  - Extensions in **SAP partner solutions** (e.g., OpenText vendor invoice management [VIM])
  
- SAP’s built-in tax determination parameters are located **in SAP or linked directly to SAP**.
- Full integration with transactional processes ensures a **single source of truth**.

---

### External Tax Calculation

- Applies specifically to the **United States, Puerto Rico, Canada, and Brazil**.
- For jurisdictions with **high volatility** and **complex tax rules**, SAP supports integration with external partners.
- External partner solutions cover **tax rate changes** and **maintenance**.
- Integration details:
  - **SAP S/4HANA Cloud** supports **API integration** (Canada under investigation).
  - **On-premise SAP S/4HANA**, SAP HANA Enterprise Cloud, and SAP S/4HANA Cloud private edition support the **RFC interface**.
  - For Brazil, integration is via **SAP Integration Suite** in SAP S/4HANA Cloud; on-premise supports only internal determination.
  
- For more details, see **SAP Note 1497956**.

---

### Time-Dependent Tax Codes

- The **tax code** is the most important **indirect tax data** in SAP, with a technical name consisting of **two digits**.
- SAP S/4HANA Cloud introduced **time dependency** for tax codes with two new columns: **Valid From** and **Valid To**.
  
![Validity Periods of Time-Dependent Taxes](figure1.15.png)

---

### SAP S/4HANA Availability for Time Dependency

- Available only for **SAP S/4HANA Cloud** as of release 2202.
- Available in **35 countries**, including Austria, Belgium, China, Czech Republic, Denmark, Finland, France, Germany, Hungary, India, Indonesia, Ireland, Italy, Japan, Luxembourg, Malaysia, Mexico, Netherlands, New Zealand, Norway, Philippines, Poland, Romania, Saudi Arabia, Singapore, Slovakia, South Africa, South Korea, Spain, Sweden, Switzerland, Taiwan, Thailand, United Arab Emirates, United Kingdom.
- SAP is testing for **on-premise availability** (see **SAP Note 3071737**).

- Validity periods link to:
  - The **tax code**
  - **VAT return field mapping**

- Example: During COVID-19, Germany decreased the standard VAT from **19% to 16%** for six months, affecting the VAT return mapping.

---

### Configuration of Time-Dependent Taxes

![Configuration of Time-Dependent Taxes](figure1.16.png)

- Previously, tax rate changes required creating new tax codes and maintaining many tables.
- With time-dependent tax calculation, tax rate changes are set via **validity periods**, automatically applied in calculations.
- Benefits:
  - Saves tax codes
  - Reduces maintenance efforts in **financial accounting**, **sales and distribution**, and **materials management**
  - Prevents inconsistencies between sales and finance documents

---

### Activation of Time-Dependent Taxes

Depends on whether the **country/region solution** in SAP S/4HANA Cloud was activated:

- **Not previously activated**: Time-dependent taxes activate with the country/region.
- **Previously activated**: Existing custom and transactional data requires migration.

---

### Customer Responsibility

- SAP customers create and maintain **tax codes** where time-dependent tax calculation is active.
- Tax codes and rates are **standard SAP reference content**.
- Users must comply with **legal requirements** and update tax codes accordingly.

- Activation must occur in both **quality and production systems** to enable transports.
- Changes in quality systems are transported automatically to production.
- Deletion of tax codes is **not supported** by SAP.
- Retroactive tax rate maintenance deletes later tax rates, which must be recreated chronologically.
- A system check usually prevents such modifications (can be disabled upon request).

- Refer to **SAP Notes 2809073, 2818970, 2819322, and 2937776** for additional details.

- Foreign tax code maintenance is **not possible** with time-dependent tax activated.
- Example: A **Dutch company code** cannot add foreign tax codes for VAT registrations.

---

## Registration for Indirect Taxation Abroad (RITA)

- Enables tax registration in a **different country/region** than the company code.
- Useful when no company code exists for foreign presence.
- Registers foreign taxable transactions for **indirect tax reporting and payments**.

### SAP S/4HANA Availability

- Available **only in SAP S/4HANA Cloud test environment** (see **SAP Note 2907372**).
- Enabled for Austria, Belgium, Finland, France, Germany, Ireland, Italy, Netherlands, Spain.
  
- RITA **cannot** be used where a foreign company code exists for a **fixed establishment abroad**.
- The **condition type GRWR** (statistical value) is used in pricing procedures to calculate statistical indirect tax values abroad.
- Reminder: Replace **routine number 008** with **routine number 091** in the pricing procedure’s Requirement field.
- Applies also to self-created pricing procedures.
- For more, see **Chapter 4** on condition types, routines, and pricing procedures.

---

## Online Validation Service

- Uses the **VAT Information Exchange System (VIES)** to automate and integrate VAT ID validation in SAP ERP and SAP S/4HANA.
- Supports **European indirect tax compliance** by validating VAT IDs for EU member states.
  
![VIES VAT Number Validation Homepage](figure1.17.png)

- Based on **VAT Directive 112/2006** and updated by **Council Directive (EU) 2018/1910**.
- Local validations possible (e.g., Polish tax payer e-register).
- Extensibility options exist for validating other business partner attributes.
- Integrates with **business partner maintenance** and the **sales/billing process**.

### Availability

- Available for all SAP deployment options:
  - SAP S/4HANA
  - SAP S/4HANA Cloud
  - Central Finance
  - SAP ERP

- SAP is working on further integration with other applications.

---

## SAP Tax Compliance

- An **end-to-end tax monitoring solution** covering data across the entire tax lifecycle inside and outside SAP.
- Features include:
  - **Risk-adjusted control scenarios**
  - **Workflow-based mitigation task lists**
  - **Machine learning components** to reduce manual effort
- Helps reduce effort for **tax exception handling**.
- Generates high value for tax functions as part of the organization.

## SAP Tax Compliance Overview

**SAP Tax Compliance** allows you to replicate data centrally on an **SAP HANA database** or use the ERP data directly (or hybrid) to centralize and automate **tax checks**, perform remediation, and improve the quality of tax data. As part of a **tax control framework**, SAP Tax Compliance automates tax controls for streamlined compliance and provides the following:

- Firm-wide centralized checking rules (not limited to tax checks)
- Ability to apply controls to **100% of relevant transactions** due to high performance based on **in-memory technology** and depending on the risk profile of checks
- One central platform with full integration into processes by replication of SAP external data sources
- Automation supported by **machine learning** to reduce manual intervention
- Continuous monitoring of exceptional tax postings
- Use of SAP workflow for alert notification and orchestration
- Audit-proof remediation trail
- Simulations of scenarios possible
- Repository of issues to improve processes and minimize noncompliance risk

**SAP Tax Compliance** uses navigation targets (via **SAP Fiori**) to jump directly from exceptional documents into the **SAP S/4HANA** system to execute analysis.

---

## 1.3 Taxes with SAP S/4HANA

An example of the **Release Supplier Invoice task** in SAP Tax Compliance is shown in Figure 1.18.

Additionally, **SAP Tax Compliance** can be linked to **SAP Document and Reporting Compliance**, allowing integration of exceptions directly in the reporting process and enabling streamlined processing of tax monitoring and reporting along the tax lifecycle.

The key value added for the tax function and broader organization is to enable an **automated internal tax control/compliance framework (no spot checks)**. This connection must be carefully adapted by organizations to maximize efficiencies.

We’ll discuss SAP Tax Compliance further in **Chapter 10**.

---

## SAP Document and Reporting Compliance

With **SAP Document and Reporting Compliance**, tax functions can manage **e-invoicing**, **real-time reporting**, and **statutory reporting** on a global scale. The solution combines features originally available via **SAP Document Compliance** and SAP solutions for advanced reporting and compliance.

**SAP Document and Reporting Compliance** integrates the tax reporting process in **SAP S/4HANA** to manage the end-to-end process from:

- Data extraction
- Data aggregation
- Monitoring
- Exception handling
- Filing
- Payment

Figure 1.19 shows an example monitoring screen displaying **Advanced Compliance Reports**.

### Coverage

- The part formerly known as **SAP solutions for advanced compliance reporting** covers periodic/statutory reporting requirements (e.g., **VAT return**, **European Commission (EC) Sales List**, **WHT report**) and tax reporting requirements (e.g., **SAF-T**).
- The part formerly known as **SAP Document Compliance (or eDocument)** covers **continuous transaction controls (CTC)** (e.g., **X-Rechnung**, **FatturaPA**) for several jurisdictions.

### Key Features

- Enables standardization, efficiency, and automation
- Provides transparency on compliance status and fulfills audit trail requirements
- Reduces ongoing compliance costs
- Supports the transition from periodic statutory reporting to **CTC**
- Ensures consistency between real-time business document submissions and statutory reports
- Facilitates reconciliation between internal records and records available on tax authorities’ platforms

For further details about **SAP Document and Reporting Compliance**, refer to **Chapter 9, Section 9.3**.

---

## 1.4 Getting Started with SAP S/4HANA

Now that tax in the context of **SAP S/4HANA** has been discussed, it’s time to consider your first steps toward your **migration/implementation project**.

This section discusses:

- Deployment options
- Migration approaches
- **SAP Activate** methodology for your implementation project

The tax function typically has limited influence over migration and deployment decisions. Therefore, an overall **tax (technology) strategy** is very important and needs to be included in discussions.

---

### Importance of Tax Involvement

- Migration strategies and deployment alternatives should enable the organization to be **tax compliant** considering the available capacities and technologies of the tax function.
- Tax consumes a large portion of data in operational and finance processes and must be involved in the **SAP S/4HANA implementation**.
- The migration can help realize **data-driven tax management** and improve data availability and quality.
- You should define whether and to what extent additional tools like **SAP Tax Compliance** or **SAP Document and Reporting Compliance** will be implemented and used.
- The **bill of materials (BOM)** can be leveraged for tax solutions and should be discussed early from the tax side.
- The **tax operating model** from Section 1.2 is relevant here. The clearer the vision, the better SAP S/4HANA can be used to realize and fund it.

---

### 1.4.1 Deployment Alternatives

**SAP S/4HANA** can be deployed in:

- The **cloud**
- **On-premise**
- Hybrid (parts on-premise and parts in the cloud)

#### SAP S/4HANA On-premise

- Chosen by companies with deep supply chain processes (logistics), production, or manufacturing sites.
- Offers a comprehensive range of functions with customizing and enhancement options.
- Most current deployments are on-premise.

#### SAP S/4HANA Cloud

- A **software as a service (SaaS)** offering.
- SAP’s strategy is to migrate more clients to the cloud.

Cloud deployment types:

- **Private (single-tenant) edition** — combines on-premise solution scope with SaaS benefits, full function access, and full customizing access. Does **not** allow modifications to SAP source code.
- **Public (multitenant) edition** — lowest flexibility, fully predefined and standardized processes, offers highest scalability.

#### Availability Considerations

Due to the **SAP roadmap** of tax-relevant solutions such as **RITA**, you should carefully verify that the available functionality at go-live covers legal requirements, as semi-/automated workarounds are difficult or impossible in **SAP S/4HANA Cloud**.

---

### 1.4.2 Migration Strategies

Figure 1.20 represents alternative SAP S/4HANA implementation options:

| Category                 | Initial Situation                              | Scenario of Implementation                                           |
|--------------------------|-----------------------------------------------|---------------------------------------------------------------------|
| New Implementation Greenfield | SAP ERP, SAP Business Suite, AnyDB, Non-SAP Systems | Starting from scratch, migrate existing data, reengineering, process simplification |
| Selective Data Transition Hybrid | SAP ERP, SAP Business Suite, AnyDB, Non-SAP Systems | Example: Different ERP systems; selective greenfield/brownfield       |
| System Conversion Brownfield | SAP ERP, SAP Business Suite                  | Existing business processes moved to new platform, including customizations and data via Software Update Manager (SUM) |

---

### New Implementation vs. System Conversion

- **New Implementation (Greenfield):**  
  Starts from scratch based on **SAP S/4HANA best practices** and industry solutions. The system is completely reimplemented and processes are extensively remodeled. Preferred for non-upgradeable or very inhomogeneous ERP systems or when digital transformation is required (e.g., M&A, digitization initiatives).

- **System Conversion (Brownfield):**  
  Transfers existing processes from SAP ERP to SAP S/4HANA while retaining the current setup as much as possible. Requires adoption to new **SAP HANA data structures** and functionalities. Suitable for organizations with simple and stable functionalities or prior transformation projects.

- **Mixed Approaches:**  
  Common in practice, combining benefits of both greenfield and brownfield for optimal investment value and future-proof setups.

- **Selective Data Transition:**  
  For organizations with multiple ERP systems, using a leading system upgraded as a pilot/template project, then merging or rolling other systems in later.

---

### Central Finance

For highly fragmented and complex ERP landscapes, **Central Finance** offers an alternative:

- Centralizes heterogeneous and distributed IT and SAP system landscapes (**SAP and non-SAP**) linked to one **SAP S/4HANA system**.
- Used mainly for centralized finance data and global financial information reporting.

#### Central Finance Landscape Example (Figure 1.21):

- Legacy systems such as **Oracle, SAP ERP, JD Edwards** connected to **SAP S/4HANA Finance**.
- Enables harmonized reporting consolidation, analysis, and tax/legal integration (e.g., SAP BPC, SAP Analytics Cloud, tax systems).

#### Key Points

- Finance transformations often use the Central Finance approach.
- Logistic or production flows are less relevant as finance data is centralized.
- A critical project milestone is when company parts run on a leading Central Finance instance with independent postings and payments only in Central Finance.

## Tax Requirements in Central Finance and SAP S/4HANA Migration

If there is no more **synchronization** with the source systems, the **tax requirements** of the leading **Central Finance** instance need to be reflected in the same way as for **brownfield** or **greenfield implementations** to have a **tax-compliant setup**. 

As **Central Finance** is often the first step of an **SAP S/4HANA migration**, the system can later be fully migrated to the **SAP S/4HANA** platform.

---

## Template Approach

The **1,000 biggest SAP customers** are multinational organizations that moved or will move to **SAP S/4HANA** with a multiple country scope and multiple systems, including **legacy systems**. 

Many have already moved or planned the move to **SAP S/4HANA**. Introducing at least one central **SAP S/4HANA system** for multiple countries and subsidiaries poses challenges, such as slim project timelines, intensive sprints, and uneven distribution of **SAP S/4HANA** know-how between organizations, solution integrators, and tax (technology) consultants.

Therefore, a **template approach** can support a successful project by providing central definitions, design principles, processes, and harmonized master data for the template and rollouts. Only deviations need reflection during rollout.

Key considerations:

- Define one or more **template countries** to localize available tax requirements.
- The template should cover most **tax-relevant processes** and **transaction categories** to retain significant template character.
- Use a **template transaction matrix** containing comparable transaction categories and relevant tax parameters.
- Maintain a **central tax code repository** based on a unified approach for compliant, consistent, and efficient tax code setup.
- This methodology applies to **all tax types** throughout the template and rollouts.
- A **global tax template** defines generally applicable requirements across countries for each tax type.
- The **local tax template** extends the range of functions based on legal requirements of national companies.

---

## 1.4 Getting Started with SAP S/4HANA

### 1.4.3 SAP Activate Project Methodology

The **SAP Activate implementation methodology** provides project guidance with tools, templates, and processes to support customers and consultants. It accommodates all SAP modules and project complexities, from single pilot projects to large complex transformations.

This section highlights **tax requirements** covered by the **SAP Activate methodology** across project phases, as illustrated in **Figure 1.22**.

---

### SAP Activate Methodology Phases

- **Discover**
- **Prepare**
- **Explore**
- **Realize**
- **Deploy**
- **Run**

---

### Discover Phase

**SAP Activate-driven SAP S/4HANA implementations** start with the **Discover** phase. Here, the **tax function** should create or have a strategy for their **digital transformation** aligned with the company’s **SAP S/4HANA transformation** strategy (see Section 1.2).

The outcome is leveraged to build a **business case** demonstrating the value-added of digital, automated, and data-driven **tax management** within an up-to-date tax operating model.

Key points:

- The business case quantifies **saved compliance costs**, **efficiency gains**, and **cash potentials**.
- These savings are compared to implementation costs.
- Positive net value supports the **tax business case**.

---

### Prepare Phase

During the **Prepare** phase:

- Gather requirements for jurisdictions in scope, covering applicable transaction scenarios.
- Draft process flows to structure tax-relevant processes.
- Conduct a preliminary **fit-to-standard analysis**.
- Prepare workshop material for design workshops.
- Conduct an initial workshop for key design decisions if complexity is high.

Important decisions during this phase include:

- Support on additional products (e.g., **external tax engine**).
- Involvement of **external partners** (third-party tax solution providers).
- Involvement of **implementation partners**.
- Development of a project plan for the **Explore** phase.

---

### Explore Phase

The **Explore** phase involves:

- Conducting **design workshops** that produce a **business process design** for tax, incorporating solutions for tax requirements.
- Running **fit-gap sessions** to identify gaps for the future model.
- Hosting **cross-workstream workshops** to address dependencies with other workstreams, such as logistics or order management.
- Documenting all findings and solutions in a **tax business process document (BPD)**.
- Aligning with external solution providers continuously.

---

### Realize Phase

During the **Realize** phase, the tax lead typically remains within the tax department.

Relevant tasks include:

- Development of **system adjustments** (reports, interfaces, conversions, enhancements, forms, workflows - RICEFWs):
  - Functional specifications
  - SAP coding
  - Unit testing
  - Quality review
  - Presentations to implementation teams or architects

- **Testing**:
  - Execute functional tests and document results
  - Incorporate program code into system enhancement documentation
  - Adapt transport system from development to test environment

- Implementation of **tax determination logic**:
  - Setup of primary **VAT determination parameters**
  - Setup of **condition types**, **access sequences**, and **condition records**
  - Linking tax determination logic components
  - Adjusting migration concept

- **Master data management**:
  - Setup of **tax codes**
  - Validation of **invoice forms**
  - Coordination with other workstreams and master data
  - Adjust migration concept

- **Reporting and monitoring**:
  - Configuration of **tax monitoring**
  - Configuration of **tax reporting**

- Documentation and quality of build activities

- Alignment with **Project Management Office (PMO)** across workstreams

---

#### Testing Activities

To prepare for **go-live**, testing is critical to ensure the tax setup functions correctly. Key testing steps:

1. Draft **testing matrices** for countries in scope.
2. Support testing cycles for exceptions.
3. Train users on using testing matrices and meetings.
4. Create test cases (end-to-end, user acceptance) in **SAP S/4HANA**.
5. Deploy testing matrices (including SAP S/4HANA testing report and data extraction).
6. Populate testing matrices.
7. Review test results.
8. Address exceptions with workarounds.
9. Obtain approval from the tax team for test results.

---

### Deploy Phase

The **Deploy** phase prepares the system for production, including:

- **Dress rehearsal**
- **Cutover activities**
- The **go-live** day transitions the project to operational mode.

Although confidence is high after months of hard work and testing, the go-live day is filled with curiosity about how the tax setup will perform.

After go-live, the **hypercare** period begins, which:

- Lasts a short time to transition the project into the productive phase.
- Allows changes to enter standard processes (e.g., tax rate changes).
- Requires a clear time schedule ensuring all hypercare topics are resolved before the **Run** phase.

---

#### Hypercare Observations

- Tax-related topics during hypercare often arise from open testing items without material compliance risks.
- For instance, tax testing of a transaction category may cover one country fully but not a second with identical setup. This approach saves time but needs resolution during hypercare to ensure completeness and compliance.

---

### Run Phase

In the **Run** phase, the implementation cycle ends and **maintenance** begins.

Key points:

- The organization adopts the **operational tax control framework** with defined responsibilities.
- Tax-related solutions and applications are supported by earlier **training sessions**.
- Measurement activities start, monitoring results through **tax monitoring solutions** to ensure compliance.
- Deviations trigger **mitigation workflows** to support learning and continuous improvement of the tax setup.

---

## 1.5 End-to-End Processes

**SAP S/4HANA transformations** are structured according to the **business processes**, that is, **procure-to-pay**, **order-to-cash**, and **record-to-report**. **End-to-end scenarios** describe all process steps that are subject to practical application in the ERP and IT landscape. However, depending on **industry-specific characteristics**, modified scenarios are also possible. 

The **tax function** needs to understand all **tax-relevant steps** within the **end-to-end scenarios** and the business processes behind them, and then needs to integrate their requirements accordingly. To structure the tax input to the business processes, the tax function defines the requirements according to **tax end-to-end scenarios** across the relevant tax types as a prerequisite for an SAP S/4HANA project integration and to support **PMO activities** with regard to tax. 

Before we explain the tax implications and tasks in the end-to-end business processes, we need to describe the typical phases of an **end-to-end tax process**.

### 1.5.1 Tax Processes

The **end-to-end tax process** can be divided into four phases across tax types: 

- **data recording and tax determination**
- **data analysis and monitoring**
- **compliance and statutory reporting**
- **tax planning**

We’ll discuss each in the following sections, starting with the processes for **indirect tax** and then taking a look at **direct tax**.

### Tax Data Recording and Tax Determination

The most granular tax element is **tax-relevant data**. The tax requirements define what kind of data is necessary to determine the right tax consequence. In general, tax decisions are processed based on **data** and **tax rules**.

Let’s take the example of an **intra-community purchase of goods** of a **German company code** received in Germany. The requirements for this case were captured in a **tax determination matrix**, as shown in Table 1.3. The correct result was calculated based on a rule that combines tax-relevant data from different process steps.

It’s thus important to maintain **correct master data** in order to receive and read **digital invoice information**, to combine and collect the relevant data elements in the **tax data model**, and to have a rule in place that is up to date according to the latest **tax requirements**.

| Data Elements                | Available Data/Documents in SAP S/4HANA             | Rule Set for Tax Determination                                                 |
|-----------------------------|----------------------------------------------------|--------------------------------------------------------------------------------|
| Transaction category:        | Purchase order                                     | Determine the transaction category: direct purchase (two parties involved)     |
| accounts payable direct purchase | Supplier master data                         | Determine the ship-from country by taking the first two digits of the VAT ID of the supplier on the invoice: BE |
| Tax departure country: NL    | Material master data                               | Determine the ship-to country from the receiving plant assigned to the German company code in the company code master data: DE |
| Tax destination country: DE  | Goods receipt                                     | Material tax indicator: standard VAT rate 19%                                  |
| Tax rate: 19% (input VAT and output VAT) | Supplier invoice                              | Rule result: Intracommunity purchase of goods in DE 19%                        |
| Supplier name: Supplier 1    | Tax code                                          |                                                                                |
| Supplier address: Supplier Street 1, Supplier City, Netherlands | Payment                                           |                                                                                |
| Supplier domestic VAT ID: NL123456789 | Financial posting                               |                                                                                |
| Supplier foreign VAT ID: BE123456789 |                                                    |                                                                                |
| Ship-from country: Belgium   |                                                    |                                                                                |
| Ship-to country: Germany     |                                                    |                                                                                |
| Plant of goods receipt: Germany |                                                 |                                                                                |

**Table 1.3 Tax Determination Matrix Example**

### Tax Monitoring and Data Quality

The **tax monitoring process** will ensure that the **tax-relevant data** is **complete and correct** and will prepare proper **tax reporting**. In our example, we could apply several data checks based on the collected data:

- Check the **foreign VAT ID** of the Dutch supplier by comparing the **master data** with **transaction data**.
- Compare the calculated tax amount based on **invoice data** and the **tax rate** from the tax code settings with the posted tax amount.
- Check if the supplier has used **multiple tax codes** during this month. 

These monitoring actions aren’t just detective controls to identify anomalies during the reporting process; they also help to prevent systematic exceptions due to **master data incompleteness** and **rule set mistakes**.

During monitoring, you can make optimal use of the possibilities of the **data model** in SAP S/4HANA (e.g., **core data services [CDS] views**). Important solutions that are used in this area are **SAP Tax Compliance** and **SAP S/4HANA embedded analytics**, which are explained in more detail in Chapter 10.

### Tax Reporting

The next phase is **tax reporting**, which is important to fulfill the **tax filing obligations** during the right timing and in the right quality to avoid negative impacts on the duration and quality of the tax reporting process.

In our example, we have to ensure the right mapping between the **tax code** and the **VAT return field** in the first step. In the next step, the **tax reporting process** needs to fulfill the legal requirements of the **digital processing of a VAT return**. Because no paper form is allowed, you must ensure that you transfer the data in the right **data format** by using the right **interface** to the tax authorities.

In this case, you can either use the **SAP interface to the tax authorities** for the SAP core functionalities (**Transaction FOTV**), or you can use SAP solutions such as **SAP Document and Reporting Compliance** for statutory reporting as a processing solution to create a **VAT return** and to submit the report to the tax authorities.

### Tax Planning

**Tax planning** can include the estimation and calculation of a **target effective tax rate** for **direct tax purposes** (in accordance with **pillar two**, or **Global Anti-Base Erosion [GloBE]**, minimum taxation regulations).

The interdisciplinary approach of tax planning is to structure the **operating model** and **supply chain** to end up with **destination status** where indirect tax, direct tax, and transfer pricing follow a common logic. For example, in a **branch model** where the branches act as **low-risk distributors** with a **buy-sell structure** (flash title) to avoid high margins in foreign branches and to enable a centralized approach for tax management.

Ideally, the branches are equal from an **indirect** and **direct tax perspective** to enable a reconciliation of indirect and direct taxes.

### Direct Tax

Having looked at indirect taxes, let’s now take a look at the **direct tax lifecycle** in Figure 1.23. The **direct tax lifecycle** also consists of four parts: 

- **tax reporting**
- **tax declaration**
- **tax audits**
- **management**

In the tax reporting practice, there is a clear focus on the areas of **indirect taxes** and **income taxes**. The **statutory reporting** includes further tasks for tax functions that cover determination of **current and deferred taxes** together with the annual report, for example, regarding **tax risks**.

Tax reporting is often based on procedures of initial **direct** and **indirect tax-related tax registration**, which are important to be compliant and to enable the declaration of **VAT** and the **VAT deduction** in our sample case in Germany.

![Figure 1.23 Direct Tax Lifecycle](attachment:figure_1_23_direct_tax_lifecycle.png)

**Figure 1.23 Direct Tax Lifecycle**

Automation potentials include the use of **tax ledgers**, the SAP implementation of the **E-Balance taxonomy**, **tax determination for direct taxes (tax tagging)**, or the extended **WHT function**, which we detail in Chapter 3, Section 3.1.1.

### 1.5.2 Business Processes

In the following sections, we’ll discuss the most important **business processes** in practice with **high tax relevance** in the end-to-end scenarios of **sales**, **production**, and **finance**.

#### Procure-to-Pay

The **procure-to-pay scenario** bundles the business processes of **purchasing** from the request and execution of an order to the receipt of deliveries or services and the processing of the invoice receipt to the payment run (see Figure 1.24).

Procure-to-pay processes are especially subject to **automation** as the highest potential for efficiency gains and cash optimization occurs here. See Chapter 4, Section 4.2.5, for an overview of procure-to-pay solutions.

Along the procure-to-pay process, a lot of **tax-relevant data** is recorded:

- **Business partner** (vendor, supplier)
- **Material type**
- **Material group**
- **Procurement type**
- **Order quantity**
- **Terms of delivery/Incoterms**
- **Terms of payment**
- **Conditions** (gross prices, discounts, freight costs, taxes)
- **Account assignment type** (assets, cost centers, production orders)
- **Purchasing organization** (beneficiary company code)
- **Plants** (place of delivery/place of supply)

This data covers requirements for all tax types, **indirect taxes** (VAT deduction), **WHT** (suspension certificates), **transfer prices** (target margin), and **income taxes** (benefit in kind).

![Figure 1.24 Procure-to-Pay Process](attachment:figure_1_24_procure_to_pay_process.png)

**Figure 1.24 Procure-to-Pay Process**

#### Order-to-Cash

The **order-to-cash scenario** specifies the business processes of sales from the contact to **account receivables** with **tax-relevant impacts** for all tax types (see Figure 1.25).

During the order-to-cash process, SAP S/4HANA records tax-relevant data for all tax types, for example:

- **Business partner** (customer)
- **Material type**
- **Order quantity**
- **Terms of delivery/Incoterms**
- **Conditions** (gross prices, discounts, freight costs, excise taxes)
- **Delivery plant** (place of delivery/place of performance)

![Procure-to-Pay Logistics Execution Order-to-Cash](attachment:procure_to_pay_logistics_execution_order_to_cash.png)

## Prepurchase Activities Purchasing Goods/Service Receipt Payment

- **Purchase Request**
- **Purchase Order**
- **Goods Receipt**
- **Invoice Posting**
- **Procure Payment**

## Tax Types

- **VAT** (Value-Added Tax)
- **Corporate Income Tax**
- **Customs**
- **WHT** (Withholding Tax)
- **Transfer Pricing**

---

## 1 Introduction to Taxes with SAP S/4HANA

### Record-to-Report

The **record-to-report scenario** includes **tax reporting** activities and **statutory accounts** activities. Important activities relevant mainly for **income taxes** are:

- Identification of **temporary differences** based on deviating commercial law/tax recognition and valuation regulations/tax accounting
- Review of tax items for tax effects from **previous years (true-ups)**
- Determination of **current taxes** in the individual financial statements of the current financial year
- Assessment and approach of **tax risks** with regard to future tax audits
- **Arm’s length compliant control of transfer prices** through continuous monitoring

### Summary (1.6)

This chapter has examined the opportunities of introducing **SAP S/4HANA** from a **tax perspective**. It covered:

- Project procedures and SAP components/products
- Transformation roadmaps for the tax department and its target operating model
- Main topics of **people**, **data**, **processes**, and **control** in the target operating model
- Important challenges with transformation requirements

The next chapter moves on to **core tax topics**: indirect and direct tax requirements.

---

## Order-to-Cash and Procure-to-Pay Processes

### Overview

- **Order-to-Cash**
- **Procure-to-Pay**
- **Forecast-to-Manufacture**
- **Logistics Execution**
- **Record-to-Report**

### Sales Process Flow

- Contact Inquiry
- Quotation
- Sales Order
- Delivery Transfer Order
- Billing Document
- Accounts Receivable

### Tax Areas

- **Transfer Pricing**
- **Direct Tax**
- **Tax Controversy**
- **Tax Guidelines**
- **Indirect Tax**

### Tax Management Functions

- Tax CMS (Compliance Management System)
- Manage **VAT/WHT**
- Compliance Management
- Manage Tax Data
- Manage VAT Determination
- Continuous Monitoring
- Tax Tagging
- Manage WHT Recording
- Manage VAT Monitoring
- Manage Process-Integrated Controls

---

## 2 Indirect and Direct Tax Requirements

### Introduction (Chapter 2)

This chapter introduces:

- **Indirect** and **direct tax concepts** across different geographies
- The foundation for tax activities in **SAP S/4HANA**
- Global context of tax requirements before system implementation

### Topics Covered

- Indirect tax systems
- Taxable events
- Exemptions
- Grouping
- Transaction types
- Standard and **digital reporting requirements (DRRs)**
- Direct tax requirements overview

---

### 2.1 Indirect Tax

**Indirect taxes** are levied on **goods and services** rather than on profit or income as in direct taxes.

---

### 2.1.1 Global Indirect Tax Systems

Countries have systems to levy taxes on goods and services, independent of income or profit. Usually, the **final consumer** covers the tax, but the **provider** of goods/services is responsible for collection and payment.

- Taxes may be charged at **federal**, **state**, **municipal**, or **city** levels.
- Multiple levels can tax the same transaction.
- Various **indirect tax systems** exist worldwide.
- Some indirect taxes target special goods/services, such as **tobacco**, **alcohol**, or **energy**.

---

### European Union Harmonized Value-Added Tax System

- **Value-added tax (VAT)** is an indirect tax calculated on the **taxable amount**, usually the full amount paid for goods/services.
- VAT is **added on top** of the net price.
- At every supply step toward the final customer, VAT is calculated.
- Unlike sales tax, VAT can be **deducted as input tax** by suppliers.
- VAT is collected in a **staged collection process**.

#### Example of VAT Calculation (20% VAT)

| Party          | Amount                  | VAT Paid | VAT Deducted | VAT Effect Cash Flow |
|----------------|-------------------------|----------|--------------|---------------------|
| Manufacturer   | 120 € (100 Net + 20 VAT) | 20 €     | 0 €          | Pays 20 € VAT       |
| Wholesaler     | 144 € (120 Net + 24 VAT) | 24 €     | 20 €         | Pays 4 € VAT        |
| Retailer       | 192 € (160 Net + 32 VAT) | 32 €     | 24 €         | Pays 8 € VAT        |
| Government     | -                       | 44 €     | -            | Receives total 32 € VAT |

- The government collects **32 € VAT (20%)** on the final net price of 160 € through the chain:  
  20 € (manufacturer) + 4 € (wholesaler) + 8 € (retailer) = 32 €

#### VAT Directive 2006/112/EC

- Came into force in **2007** for all **EU member states** (27 states as of 2022).
- Replaced the Sixth Directive (established in 1977) to harmonize EU VAT systems.
- The goal is **neutrality in competition** so similar goods/services bear the same tax burden regardless of production/distribution chain length.

**Quote:**  
*“…result in neutrality in competition … such that within the territory of each Member State similar goods and services bear the same tax burden, whatever the length of the production and distribution chain.”*  
— Council Directive 2006/112/EC on the common system of value added tax

- The Directive sets general regulations acting as guidelines; member states can apply **derogations** or **exceptions**.
- Minimum **standard VAT rate is 15%**, but reduced rates are possible.

#### Intra-EU Cross-border Transactions

- Simplified compared to non-EU cross-border transactions.
- **Reverse charge mechanism** for **B2B transactions** applies:  
  The **recipient** pays VAT instead of the supplier.
- Example:  
  Italian manufacturer ABC sells machine to French company XYZ and transports it to France.  
  ABC’s intra-community supply is **tax exempt**. XYZ must pay French VAT and can deduct it, making it non-cash effective.

#### EC Sales List

- A periodic report of all intra-community transactions by customer VAT ID.
- Helps reduce tax evasion via cross-border communication.
- Less detailed than real-time reports but unique in the international tax domain.

---

### Other VAT Systems

- Many countries have adopted VAT, including **China** in 1994.
- China’s **VAT Provisional Regulations and Implementing Rules** came into effect in **2009**.
- VAT operates similarly with **input VAT deductions** and the final consumer paying VAT.
- The **Gulf Cooperation Council** (Saudi Arabia, UAE, Oman, Qatar, Bahrain, Kuwait) has a similar VAT framework.
- VAT rate in Gulf states is around **5%**, but Saudi Arabia raised it to **15% as of January 1, 2020**, due to COVID-19 and oil price drops.

---

### Goods and Services Tax (GST) Jurisdictions

- GST functions similarly to VAT.
- Both VAT and GST follow OECD International VAT/GST Guidelines.
- OECD groups VAT and GST under the same definition:
  
  **Quote:**  
  *“The terms ‘value added tax’ and ‘VAT’ are used to refer to any national tax by whatever name or acronym it is known such as Goods and Services Tax (GST) that embodies the basic features of a value added tax.”*  
  — OECD International VAT/GST Guidelines 2017

- Example: **India** had a VAT system until 2017, then implemented a unified GST system combining multiple taxes into one with a unified rate.
- Under GST, **excise taxes, VAT, and service taxes** are consolidated.

---

## 2.1 Indirect Tax

Both in **GST** and **VAT jurisdictions**, there are two different approaches to defining the amount a business needs to pay in the staged collection process. All countries defined under **VAT jurisdictions** use the **invoice-credit method**. **Japan**, globally seen as a **GST jurisdiction** with its consumption tax, uses the **subtraction method** (planned to change to invoice-credit method as of **2023**). Both methods are defined in **Table 2.1**.

### Sales Tax Jurisdictions

Whereas **VAT/GST** aim at eliminating the cascading effect by collecting the tax at every stage of value gained, **sales tax** is a **single-layer tax** that only taxes the last step when the final consumer purchases the goods or services.  

Let’s consider the example shown in **Figure 2.2**, in which the different stages from manufacturer to final customer are shown. The **sales tax** is paid only at the end by the end consumer. In total, the government collects the **32 €** (same as in VAT example) but only at the end and not along the whole chain at different stages.

**Figure 2.2 Example with 20% Sales Tax**

The **United States** has the globally most well-known **sales tax jurisdiction**. It’s a very complex system, especially regarding tax determination in **enterprise resource planning (ERP) systems**. Some of the factors that make up the complex **US sales tax system** are listed here:

- **Level of taxing authorities**  
  Sales taxes aren’t charged on a **federal level**, but only on a **state, municipality, and city level**. Every state has different regulations on who charges and collects the taxes.

- **State taxes**  
  45 of the 50 states have implemented at least one general state tax. In several of these states, there are additional local taxes to be considered. For example, **Illinois** has a standard sales tax rate of **6.25%** with an additional local tax rate of **0% to 5.25%** (as of **February 2022**). The taxes are collected by the **supplier** of the goods and services and then need to be distributed to both the state authority as well as to the local authority (if applicable).  

- With 50 different states, the list of different tax rates and collecting authorities becomes very exhaustive. Because there are so many different tax rates, the chance of a change of the rates is very high. If a company does business in many states and implements one ERP system, an automated solution should be implemented to cover these difficulties.

- **Sales versus use tax**  
  Many states have also implemented a **use tax**. Use tax tries to ensure that consumers don’t go from a high sales tax state to a state with a lower sales tax rate to purchase goods. If this does happen, a *use tax* is charged.

- **Seller privilege tax versus consumer tax**  
  The states have implemented two different rules concerning the liability of the tax payment. No matter which method the state implemented in their local regulations, the **seller** is always the party responsible for paying the tax to the relevant authorities.  

  - In the **seller privilege tax (SPT)**, the seller doesn’t need to state the sales tax on the invoice.
  - In states where **consumer tax (CT)** is implemented, the sales taxes need to be stated on a tax invoice.  
  - The liability for **SPT** lies solely with the seller, whereas for **CT**, both the seller and the consumer can be held liable for the taxes, and taxes can be collected from either party.

Another country that uses sales taxes is **Malaysia**. After having a **GST** plan for three years, in **2018**, the **sales and service tax (SST)** plan was reinstated. Like the sales tax in the US, it’s a single-layer tax imposed only on one layer of the value chain. However, the point of sales taxation isn’t at the final step on the consumer level but at the first step, the **manufacturer**. This is atypical for indirect taxes as the consumer never sees the tax amount levied on goods.

### Table 2.1 Comparison: Invoice-Credit Method vs Subtraction Method

| Method               | Description                                                                                                  |
|----------------------|--------------------------------------------------------------------------------------------------------------|
| **Invoice-Credit Method** | Transaction based. A supplier charges **VAT/GST** for each supply and creates an invoice that is sent to the recipient of the goods or services. The recipient can credit the **input tax** based on the invoice against the **output tax** that they charge for their own sales. |
| **Subtraction Method**    | Entity based. An entity has to take the **VAT/GST** calculated on allowable purchases and subtract it from the VAT/GST on taxable supplies. It’s not relevant if the entity has a tax invoice to be able to deduct the input tax. |

---

### Special Indirect Tax Jurisdictions: Brazil

The Brazilian indirect tax system is known for being the **most complicated** one in the world, and we can only provide a high-level insight here. In general, Brazil has a **VAT system** for the manufacturing and sale of goods. There are different types of taxes levied for different types of transactions:

- **IPI**  
  **Imposto sobre Produtos Industrializados (IPI)** is a tax on manufacturing and importing of industrial products. The rates can vary from **0% to 300%** for luxury goods such as cigarettes. The rates are to be found in a **table of taxes on industrialized products**.  

  This makes implementation in an ERP system very difficult as there are no general standard/reduced rates as in many other countries. **IPI rates** may vary from time to time, depending on the economic goals of the government.  

  - If an IPI rate is reduced, the application will be immediate.
  - If an IPI rate is increased, the application will be **90 days after the enactment of the law**.

- **ICMS**  
  **Imposto sobre Circulaçao de Mercadorias e Serviços (ICMS)** is a tax on the circulation of goods as well as transportation and communication. Based on the state level, the rates differ by states with federal regulations only for the interstate transactions with minimum and maximum rates of **7% and 12%**, depending on the original state and destination state, and **4%** for all interstate transactions of imported goods.

- **PIS/COFINS**  
  **Program of Social Integration (PIS)/Contribution for the Financing of Social Security (COFINS)** is a tax charged for social security reasons in addition to other taxes. The rates differ based on the system the entity uses. The total tax rate is either **9.25% (noncumulative)** or **3.65% (cumulative)**.

  - The lower tax rate of **3.65% in the cumulative system** (Lucro Presumido system, which is on presumed profit for income taxes) means that the company isn’t allowed to deduct input taxes.
  - This depends on the activity of the company: Turnover from service of communication, transportation of people, and health services, for example, are taxed by the **cumulative system** even if the company isn’t under the Lucro Presumido system.

- **ISS**  
  **Imposto Sobre Serviços (ISS)** is a tax on services and is collected by the municipalities. It isn’t part of the regular VAT system. The tax rate can be determined by the municipality and must lie between **2% and 5%**.

- **ICMS-ST and PIS/COFINS-Monofásico**  
  The **Substituição Tributária (ICMS-ST)** and **PIS/COFINS-Monofásico** concentrate taxation of all production and distribution stages at the level of the industry, having them prepay the tax for the whole chain. The other players on the chain won’t be able to credit ICMS. The calculation is complex and can vary from state to state and from industry to industry.

---

### Taxable Basis in Brazil

The taxable basis in Brazil is, unlike other countries, not based on the **gross amount** of goods and services provided. They use the **gross-up mechanism**, and the final price received is the taxable basis.

Let’s consider the example shown in **Figure 2.3**. In both methods, we assume the price the customer is paying is **120.00** and the tax rate is **20%**.  

- In the **general VAT calculation**, the taxable basis is the **net amount (100.00)**, meaning the VAT of **20.00** is added on top of the net amount.  
- In Brazil, the taxable basis is the **gross amount of 120.00** resulting in a VAT of **24.00**. So, although the price and the tax rate are the same, the payable VAT differs.

**Figure 2.3 Comparison: Brazil versus Standard VAT Calculation with a 20% Tax Rate**

| Method                 | Taxable Basis | Tax Amount | Price  |
|------------------------|--------------|------------|--------|
| General VAT Calculation | 100.00       | 20.00      | 120.00 |
| Brazilian VAT Calculation | 120.00     | 24.00      | 120.00 |

Brazil differentiates between a **commercial invoice** and a **tax invoice**. 

- Tax invoices can only be issued in the **Portuguese language**, in **BRL currency**, and **electronically**, making it difficult for foreign entities to establish global processes.
- For **PIS/COFINS**, however, no invoice must be issued as the monthly amount due will be paid based on generated turnover.

---

## 2.1.2 Taxable Events

The first question that needs to be answered to determine if indirect taxes apply is whether the event is **taxable**. Otherwise, it would be considered **nontaxable** or outside the scope of indirect taxes. A **taxable event** is defined differently in every jurisdiction. However, when looking at the **OECD International VAT/GST Guidelines** as well as different jurisdictions, there are general principles that are followed.

Regularly, three main categories of taxable events are implemented:

### Supply of Goods

One type of a **taxable event** is the **supply of goods**. This is incorporated in every indirect tax jurisdiction. A differentiation is often made between **tangible** and **intangible goods** as well as **movable** and **immovable property**.

The **transfer of ownership** and the **right to dispose of the goods** is the simplest definition for the supply of goods. Japanese law defines it as the "**supply of assets**" to incorporate both tangible and intangible assets into one category.  

- **Intangible goods** such as **intellectual property**, patents, software, and so on are more difficult to grasp as these are assets that can’t be physically transferred from one entity to another.  

For certain regulations regarding the supply of goods, see **Section 2.1.5**.

### Supply of Services

A **supply of services** is everything that isn’t a supply of goods. Some examples are **postal services**, **telecommunication services**, and **labor services** (e.g., repair, replacement, and processing of goods).  

There are special cases, for example, **transportation services**, which can fall under the different regulations of goods (Brazil) or services (China) or have a special regulation for taxation altogether (EU). Some jurisdictions have a **catalog of services** to differentiate tax rates or tax exemptions accordingly.

### Importation of Goods

**Importation of goods** is defined as the transfer of goods from outside the borders to inside the borders of a country’s tax jurisdiction. One key factor is that importation of goods usually crosses a "**controlled**" border where **custom clearance** is required.  

In the case of **EU countries**, importation is a transfer of goods from outside EU borders. Other jurisdictions, such as in **Russia** or the countries of the **Gulf Cooperation Council**, also have specific definitions for the regions where the transfer of goods is considered an import.

## 2 Indirect and Direct Tax Requirements

### Importation of Goods as a Taxable Event

The **importation of goods** as a **taxable event** ensures the **destination principle** of indirect taxation (see **Figure 2.4**). When determining the **place of taxation** for international trade, either the **country of origin** or the **country of destination** (final consumption) can be chosen to avoid double taxation. Only the **destination principle** ensures the **neutrality of international trade**.

In the example in **Figure 2.4**, the **retailer** located in **Country B** purchases goods both from **Country A (import)** and locally in **Country B**. The **neutralizing effect** of the destination principle is illustrated. When the destination principle is applied, the retailer pays the same price whether the goods are purchased locally or imported. The **tax is neutral** when comparing different manufacturers.

Under the **origin principle**, the manufacturer from **Country A** with a higher tax rate is at a disadvantage compared to the local manufacturer in **Country B**.

For **importation** in the **United States** and a **sales tax jurisdiction**, no sales tax is generally imposed. **Sales and use tax** are only applied when goods come to rest and are sold to the **final consumer**. Thus, the neutrality of indirect tax is also valid in the United States.

### Additional Taxable Event Categories

In addition to the main categories, there are two further categories supplementing the definitions of taxable events:

#### Self-Supply, Gifts, and Similar Transactions

- **Self-supply** and **gifts** are deemed taxable events in many jurisdictions.
- Goods supplied free as a **gift** may still be considered a **taxable supply** (often with certain thresholds).
- Use of **self-manufactured goods** for individual consumption is treated as a taxable event, aiming to tax **final consumption**.
- In countries where self-supply or gifts are not taxable, the **input credit** needs to be reversed.
- Creating a **global approach** for handling individual supplies without multinational cooperation is challenging.

#### Intrastate Acquisitions

- **Intrastate acquisitions** are not considered importation as they do not cross customs borders.
- In territories like the **EU**, **Gulf Cooperation Council**, or countries with VAT/GST at state level (e.g., Brazil, India), intrastate acquisitions are taxable events.
- The basis remains the **destination principle** defined under importation of goods.

---

### 2.1.3 Tax Exemptions

The first step is to determine if the event is **taxable** or **nontaxable** (**out of scope** of indirect taxes). For example, **damage compensations** (e.g., contractual penalties) are usually **nontaxable** because they do not involve an exchange of services.

Even if taxable, transactions can be **tax exempt**, meaning they fall under indirect tax law but are exempt under certain legal rules. This section outlines main types of tax-exempt transactions commonly found in indirect tax jurisdictions:

#### Main Categories of Tax Exemptions

- **Export and Intra-Community Supplies**

  - Importation and intra-community acquisitions are taxable.
  - To ensure the **destination principle**, **exports** and **intra-community supplies** must be declared **tax exempt**.
  - Example: sale of a machine is taxable locally, but if sold cross-border as export, it remains taxable but **exempt from taxes**.

- **Public Interest**

  This broad category can include:
  
  - **Education**: Often tax exempt if criteria such as official approval or minimum course duration are met. Some countries only exempt official institutions.
  - **Social Security**: Includes payments/activities for social security, refugee care, community support, and aid for the homeless.
  - **Medical Care**: Includes services by doctors, midwives, dentists, delivery of blood and organs, mainly for maintaining health and treating illness.

- **Financial, Insurance, and Postal Activities**

  - Services such as loans, insurance, and postal services are often exempt.

- **Sale and Rental of Immovable Property and Land**

  - Transactions involving **immovable property** or land supply are often exempt from indirect tax.

### Benefits and Choices in Tax Exemptions

- Tax exemptions primarily avoid **double taxation** or serve **public interest** reasons.
- Some laws, e.g., the **EU VAT Directive (Article 137)**, allow an option to **waive tax exemptions** and opt for taxation.
- Opting for taxation allows **deduction of input taxes**, which can be restricted in tax-exempt cases.
- This preserves the idea of VAT/GST as a **pass-through** on a **B2B level**.

---

### 2.1.4 Grouping

Some countries implement **tax groups** for indirect taxes to simplify compliance. Countries with VAT groups include:

- Europe (various countries)
- Australia
- Ghana
- Norway
- Saudi Arabia
- Singapore
- United Kingdom

**Grouping Types:**

- **Mandatory Grouping** (e.g., Germany, Austria)
- **Voluntary Grouping** (e.g., Poland, Italy)

Groups consist of two or more entities meeting specific criteria and are treated as **one taxable entity** by tax authorities.

#### Typical Grouping Criteria

- **Financial Link**: One legal person controls another via majority decision power.
- **Economic Link**: Economic integration where entities support or complement each other.
- **Organizational Link**: Interdependence in personnel, e.g., same managing director.

#### Benefits of Tax Grouping

- **Reduced compliance tasks**: Often only one tax return needed.
- Example (**Figure 2.6**): Four entities submitting individual returns reduced by 50% with grouping.
- Intra-group transactions are usually **nontaxable** because the group is treated as a single legal entity (**Figure 2.7**).
- A tax group can create a **cash flow benefit** due to timing differences in paying output tax and deducting input tax.

#### Downsides of Grouping

- Aggregating amounts from multiple ERP systems can be complex.
- Reconciling tax payments across the group may increase manual work.

---

### 2.1.5 Transaction Types

Certain transaction types pose special challenges and tax treatments. These vary by jurisdiction. Below is a key example:

#### Chain Transactions

- **Chain transactions** involve goods delivered directly from the manufacturer to the buyer without passing through the dealer’s warehouse.
- The dealer performs only an **intermediary function**.
- Also called **drop shipping** or **ABC contracts**.
- Due to the increase in **e-commerce**, drop shipping is frequently used to describe chain transactions.
- **Figure 2.8** shows the simplest form: a company orders goods from another company in a two-leg delivery process.

## Chain Transactions

As **Company B** doesn’t have goods in stock, they order the goods from **Company A** (first leg). The delivery of the goods goes directly from **Company A** to **Company C**. The **indirect tax laws** state that each leg of the **chain transaction** is regarded as a separate supply.

If all transactions/legs of the chain transaction are within one **tax jurisdiction** (e.g., one country or one state in the US), they are all taxable and taxed locally. When more than one country is involved, taxation becomes more complex.

For example, if **Company A** is in one country and **Company B** and **Company C** are in another, Company A directly delivers from one country to another. The question arises if the transaction between A and B, and between B and C, are tax-exempt as an **export** or **intra-community supply**.

Within the **EU**, only one transaction in a chain can be tax-exempt. All other transactions will be taxed locally in one of the member states. Important dependencies include:

- **VAT ID number** used
- Responsibility of transport (e.g., does Company A or Company C transport the goods?)

The **EU VAT directive** defines where the taxable transaction is. Defining taxable and tax-exempt transactions in these chains is tricky and needs evaluation.

There are special simplification rules such as **intra-community triangulation** — for example, when three parties from three different countries are involved. The EU created a construct to avoid registration obligation for the party in the middle of the triangle.

Simplified example:  
- **Company A** makes a tax-exempt intra-community supply to **Company B** in Denmark.  
- The Danish Company B makes an intra-community triangulation sale without VAT to **Company C**.  
- **Company C** will be liable for VAT in Hungary as an intra-community acquisition.  
- Company B can act as the middle party with only Danish registration and no reporting obligations in France or Hungary.

---

## Drop Shipping in the United States

- Whether **sales tax** applies depends on:
  - Locations of the three parties
  - Taxability of the goods
  - Where the seller/supplier has **nexus** (obligation to collect sales tax)
  
- Usually, the customer pays sales tax to the seller, who remits it to the state.
- The seller provides a **resale exemption certificate** to the supplier, exempting the supplier from paying taxes.
- Several different constellations exist based on criteria similar to the EU.

Chain transactions with special treatment mostly exist where tax exemptions apply to cross-border transactions. Many countries have no special chain transaction regulations; each leg will be taxed accordingly.

---

## Importance of Analyzing Chain Transactions

When setting up indirect tax processes, **chain transactions** and their tax treatment must be analyzed to avoid:

- Double taxation
- Registration obligations
- Compliance tasks
- Complex ERP setup

(See Chapter 4, Section 4.4.1 for more information)

---

## Tooling

To produce special parts, suppliers sometimes create special **tools** or **molds**. Although tooling costs are charged to customers, only the final part is delivered. The tools remain with the supplier.

- Deliveries of manufactured special parts shipped to another EU country are generally billed as tax-exempt **intra-community supplies**.
- Tax treatment of tooling costs that remain with suppliers varies and should be reviewed carefully.
- From an indirect tax perspective, cross-border tooling costs may trigger:
  - Local taxable transactions
  - Extra costs or registration requirements for purchasers

To simplify, some laws allow tooling costs to receive the same tax treatment as the original sale of goods. This simplifies tax reporting and registrations but complicates areas where goods movement needs reporting alongside tax exemption.

Since no goods move physically, suppliers lack proof of delivery for tax exemptions.

---

## Work Delivery

Focus on **EU** regulations relating to **work supply** and **work performance**.

- A **work delivery** is a supply consisting of processing goods that do not belong to the supplier.
- Important for cross-border transactions to differentiate between:
  - **Normal supply**
  - **Work supply**
  - **Work performance**

### Table 2.2: Differentiation between Normal Supply, Work Supply, and Work Performance

| Description                          | Example                                            |
|------------------------------------|----------------------------------------------------|
| **Normal Supply**                   | A furniture store sells a table to a restaurant.   |
| **Work Supply**                    | A carpenter makes a table, sourcing wood herself, and sells to a restaurant. |
| **Work Performance**                | A carpenter makes a table with wood provided by the customer and sells to a restaurant. |

When a taxable event contains both supply and service elements, it must be checked whether it is work supply or work performance.

- This affects:
  - Place of performance
  - Tax exemption
  - VAT liability

Errors can lead to significant VAT consequences for supplier and recipient.

---

## 2.1.6 Standard Reporting Requirements

To ensure correct **indirect tax** payment, countries establish **reporting requirements**. These vary by period and complexity, mainly based on the country's **tax gap**.

- **Tax gap**: Difference between taxes legally due and taxes actually paid.
- High tax gap → More frequent and detailed reporting.
- Low tax gap → Less frequent, simpler reporting.

Standard reporting includes monthly, quarterly, or annual **tax returns** submitted to tax authorities.

For details on **digital tax reporting** (e.g., e-invoicing), see Section 2.1.7.

---

### Table 2.3: Filing Frequency for Standard VAT/GST Reporting Obligations in 2022

| Country        | Filing Frequency   | Country        | Filing Frequency   |
|----------------|--------------------|----------------|--------------------|
| Argentina      | Monthly            | India          | Monthly*           |
| Australia      | Monthly*           | Indonesia      | Monthly            |
| Belgium        | Monthly*           | Ireland        | Bimonthly          |
| Brazil         | Monthly*           | Israel         | Monthly*           |
| Bulgaria       | Monthly            | Kazakhstan     | Quarterly          |
| Canada         | Monthly            | Kenya          | Monthly            |
| Chile          | Monthly            | Korea          | Quarterly          |
| Colombia       | Bimonthly*         | Latvia         | Monthly*           |
| Costa Rica     | Monthly            | Lithuania      | Monthly            |
| Czech Republic | Monthly*           | Luxembourg     | Monthly*           |
| Denmark        | Semiannually       | Malaysia       | Bimonthly          |
| Egypt          | Monthly            | Malta          | Monthly*           |
| Estonia        | Monthly            | Mexico         | Monthly            |
| France         | Monthly            | Netherlands    | Quarterly*         |
| Germany        | Monthly*           | New Zealand    | Monthly*           |
| Greece         | Monthly*           | Norway         | Bimonthly          |
| Hungary        | Quarterly*         | Pakistan       | Monthly*           |
| Panama         | Monthly            | Sweden         | Monthly*           |
| Philippines    | Monthly            | Switzerland    | Quarterly          |
| Poland         | Monthly*           | Thailand       | Monthly            |
| Romania        | Monthly            | Tunisia        | Monthly            |
| Russia         | Quarterly          | Turkey         | Monthly*           |
| Serbia         | Monthly*           | Ukraine        | Monthly*           |
| Singapore      | Quarterly          | United Kingdom | Quarterly*         |
| Slovenia       | Monthly*           | United States  | Monthly*           |
| South Africa   | Monthly*           | Uruguay        | Monthly*           |
| Spain          | Monthly*           | Venezuela      | Monthly*           |
| Sri Lanka      | Monthly            | Vietnam        | Monthly*           |

*Based on revenue thresholds or applications to tax authorities, different filing frequencies can apply.

---

## Electronic Filing and Information Provided

- In every country, **electronic filing** is possible.
- Some countries restrict filing to electronic format exclusively.
- Others still accept paper tax returns.

Indirect tax returns typically include:

- **Total value of sales**
- **Output tax**
- **Input tax**

More detailed returns may require:

- Taxable revenue split by tax rates
- Output tax split by tax rates
- Tax-exempt revenue with/without input tax deduction
- Intra-community acquisitions split by tax rates
- Reverse-charge revenues
- Input tax for invoices
- Input tax for intra-community acquisitions
- Import tax

---

## Supplemental Reporting Requirements

Beyond standard monthly indirect tax reporting, additional **transactional-based reports** may be required, such as:

- **Sales lists**: Local requirement listing invoices monthly.
- **EC Sales List** (EU)
- **Intrastat** (intra-community trade statistics)

### Intrastat

- Mandatory in all EU countries.
- Collects statistics on intra-community movement of goods (dispatches and arrivals).
- Data is recorded by the **Federal Statistical Office**.
- Tax authorities share this data with statistical offices to monitor compliance.

## Payment or Refund in Indirect Tax Returns

One important piece of information as the result of almost every **indirect tax return** is the **payment or refund**. These processes are handled differently in each **country**.

- In case of a **payment**, it is usually due with the filing of the tax return.
- For a **refund** (due to higher input tax compared to output tax), handling varies widely:
  - Some tax authorities pay the refund amount directly.
  - Others require a **carryforward** to the next period.
  - Often an **official refund request** or an **audit result** is needed before receiving the refund.

Even within the **EU**’s unified tax system, **reporting requirements** and **payment/refund mechanisms** differ significantly.

---

## Amendment of Tax Returns

Once a tax return has been submitted, it generally **should not be altered**. However, in practice:

- Mistakes, system errors, or new information may require **amending a tax return**.

Methods of amendment can include:

- Correcting the old tax return with an **informal letter** to tax authorities.
- Resubmitting a tax return with **new figures**.
- Submitting an **official correction form** provided by tax authorities.
- Including changes in the **current tax return** with comments to tax authorities.
- Using **special available boxes** in the current tax return for changes.

Amendments are usually possible until the **final assessment (after an audit)** or up to a **certain number of years** defined by law.

- Submission of amendments may lead to **interest payments** to tax authorities if payable amounts increase.

---

## 2.1.7 Digital Reporting Requirements (DRRs)

In this section, we discuss **Digital Reporting Requirements (DRRs)**, synonymous with:

- **Transaction-Based Reporting (TBR)**
- **E-invoicing**

TBR can be:

- **Periodic Transaction Controls (PTC)**
- **Continuous Transaction Controls (CTC)**

Many tax authorities worldwide are planning or implementing these measures to:

- Increase **transparency**
- Close **VAT gaps**

**Digitization** drives demand for efficiency and automation in tax reporting.

---

### Developments in the EU and Leading Organizations

Key players and initiatives include:

- **European Commission (EC)**  
- **European E-invoicing Service Providers Association (EESPA)**  
- **Pan-European Public Procurement OnLine (Peppol) network**

---

### Legal EU Requirements for DRRs

Per the **European Commission (EC)**:

- Member states can impose obligations besides VAT returns based on **Article 273 of the VAT Directive**.
- These obligations aim to ensure **proper VAT collection** and **fraud prevention**.
- Conditions for imposition:
  - Must **not interfere** with VAT principles.
  - No distinction between **cross-border and domestic transactions**.
  - No added **border formalities**.
  - No additional invoicing requirements beyond those in **Chapter 3** of the VAT Directive.

**Article 273** permits member states to introduce additional **transaction-based reporting** like business transaction reports or tax/accounting data.

---

### E-invoicing

- Mandatory e-invoicing does **not** allow use of the derogation under Article 273.
- Instead, member states must request a derogation under **Article 395**, subject to **unanimous council agreement** based on the Commission proposal.

**Business-to-government (B2G)** transactions operate differently:

- 19 member states must enable their **public administrations** to accept **structured e-invoices** using the European standard.
- Taxable persons issuing e-invoices in B2G must follow this standard.
- Some provinces still have areas where B2G e-invoicing is **not yet mandatory**.

---

### Recapitulative Statements

- Include **EC Sales and Purchase Lists** or **VAT Information Exchange System (VIES)** listings.
- Cover **intra-EU transactions**.
- Governed by **Articles 262–271** of the directive.
- VAT-registered traders must report **aggregated details** of taxable persons involved in intra-community supplies and cross-border reverse charge services (Article 196).

---

### Types of Digital Reporting Requirements (DRRs)

- **Periodic Transaction Controls (PTC)**: Aggregated transactional data reported regularly.
- **Continuous Transaction Controls (CTC)**: Electronic submission of transactional data before, during, or shortly after the transaction, including e-invoicing.

---

### Key Specifications of PTC and CTC (Table 2.4)

- **VAT listings**  
  - List transactions including base values and business partners’ VAT IDs.  
  - Submitted monthly or quarterly, aligned with VAT filing frequency.

- **SAF-T**  
  - A specific TBR form based on the OECD standard.  
  - Used for tax auditing and accounting, reporting direct and indirect tax data.  
  - Adopted locally with country-specific standards, e.g., Poland replaced VAT returns with SAF-T.

- **Real-time reporting**  
  - Transactional data transmitted (near) real-time after transaction.  
  - Includes invoice data but not the invoice itself.  
  - Example: Spain requires reporting within 4 days of invoice issuance.

- **E-invoicing**  
  - Exchange of **structured**, **machine-readable** electronic invoices between business partners.  
  - Can be centralized through public portals or decentralized via private providers.  
  - Tax administrations seek transparency; companies seek efficiency.

---

### EU DRR Landscape and Future

- 12 EU countries have implemented TBR or e-invoicing.
- Several others are working on DRR mechanisms.
- The **European Commission** is exploring potential **harmonization**.
- Proposal expected in **October 2022** on the future of DRRs in the EU.

---

### Drivers, Problems, and Effects of DRRs (Figure 2.11)

- **Drivers:** Wide discretion granted to member states under Article 273.
- **Problems:** Fragmented regulations, legal uncertainty, compliance costs, limited effectiveness.
- **Effects:** Suboptimal VAT fraud combat, revenue losses for member states and the EU.

---

### E-invoicing/CTC System Models (Figure 2.12)

Common categories of CTC models include:

- **Interoperability model**
- **Real-time invoice reporting model**
- **Clearance model**
- **Centralized exchange model**
- **Decentralized exchange model**

---

### Zones in CTC Models

- **Regulation zone:**  
  Tax administrations define data and process requirements. Future aim is common data standards.

- **Standardization zone:**  
  Certified providers exchange invoices using technical standards developed by standards organizations.

- **Non-standardization zone:**  
  Exchange between invoice issuers and recipients with no regulation or standard.

---

### Common DRR Models (Table 2.5)

1. **Interoperability model**  
   - Known as the **four-corner model**: invoice issuer, invoice recipient, and their certified service providers.  
   - Standardization between providers enables efficient invoice exchange.  
   - No tax authority involvement in exchange.  
   - Used in countries like **Finland** with small VAT gaps.  
   - Benefits like transparency are available to auditors and tax administrations.

2. **Real-time invoice reporting model**  
   - Invoice info reported directly to tax authorities without partner or provider exchange.  
   - No forwarding by tax authorities.  
   - From taxpayer perspective, an additional reporting burden with no efficiency gain.  
   - Provides greater transparency for tax authorities.

## Clearance Model (Variations Possible)

In this model, the **tax authority** has a **clearance function** and no forwarding or exchange function depending on the variant of the model. The issuer of the invoice needs **clearance of an invoice** by the **tax administration** to be able to issue the invoice to the invoice recipient. 

There is no **standardization zone**, which means no standardization through **certified service providers** between the tax authorities and the business operators.

### Centralized Exchange Model

Similar to the clearance model, there is no standardization zone, meaning that the tax authority determines the **document format** and **exchange methodology**. This creates a variety of different country-specific requirements. It differs from the clearance model by the presence of the exchange function of the tax administration portal, also called the **v-model** based on how data is exchanged through the tax portal.

### Decentralized CTC and Exchange Model (DCTCE)

“Decentralized” means the data **validation** and **exchange** are managed by **certified service providers** rather than only a public fiscal platform. The government CTC platform is relieved of heavy technical burden.

All zones are present in this model: 
- The **standardized zone** for the central governmental platform
- The **non-standardization zone** for business operators 
- The **regulated zone** for certified service providers following predefined minimum technical standards

With this approach, existing **industry Electronic Data Interchange (EDI) standards** can continue after implementing the DCTCE model.

---

### Interoperability Model Characteristics

| Model                          | Network                              | Data Format            | Invoice Exchange           | Tax Reporting                 | Timing of Reporting            | Clearing Function                    | Exchange Platform                      | Validation |
|-------------------------------|------------------------------------|-----------------------|---------------------------|------------------------------|-------------------------------|-------------------------------------|--------------------------------------|------------|
| Real-Time Invoice Reporting    | Network of certified/private providers | Unified data format     | Invoice exchange not unified | Tax reporting of invoice data or subset thereof | Near real-time after invoice issue | Possible extension to further details (e.g., payment information) | Transfer of invoice details to tax authorities | Clearing of invoice                  | After clearing, transfer may be nonstandardized or regulated |
| Clearance Model               |                                    |                       |                           |                              |                               |                                     |                                      |            |
| Centralized Exchange Model    | Central regulated exchange platform as single exchange instance |                       |                           |                              |                               | Data validation by the platform    | Invoice transfer regulated and limited |            |
| Decentralized Exchange Model  | Central regulated exchange platform and additional channels of standardized exchange through certified providers |                       |                           |                              |                               | Invoice validation by the platform  |                                      |            |

---

## 2.1 Indirect Tax

The **clearance model** requires clearance from the tax administration before issuing invoices but lacks a standardization zone. The **centralized exchange model** uses a tax authority-determined format and exchange method, often resulting in country-specific requirements, and adds an exchange function via a tax portal. The **decentralized CTC and exchange model** utilizes certified service providers to manage validation and exchange, allowing for existing EDI standards to continue, and involves a regulated standardized zone.

## 2.2 Direct Taxes

Managing **direct taxes** is a key activity of the tax function. Compared to indirect taxes, direct taxes are levied and paid at the level of the **taxable entity** or group. Major direct taxes include:

- **Income Tax**
- **Corporate Income Tax**
- **Withholding Tax (WHT)** (special form of levy)

Direct taxes are typically calculated by applying a **tax rate** on **taxable profit**. 

Local tax legislation varies significantly between countries regarding tax rates and provisions for **taxable income** determination. Meeting global direct tax requirements is complex and time-consuming. This complexity applies to local tax filings (e.g., tax returns) as well as cross-border filings like **Base Erosion and Profit Shifting (BEPS)**, **Country-by-Country Reporting (CbCR)**, or **SAF-T**.

Important external financial reporting also exists according to **Generally Accepted Accounting Principles (GAAP)** or **International Financial Reporting Standards (IFRS)**.

---

### 2.2.1 Base Erosion and Profit Shifting (BEPS)

The **BEPS framework** was developed by the **OECD** and other countries to improve international tax standards and reduce harmful tax competition. Global companies often minimize tax burdens through **aggressive tax planning**, harming competition among companies.

The main purposes of BEPS are:

- Taxation at the place of **entrepreneurial activity** and **economic value creation**.
- Improved transparency and information exchange on **tax rulings** between tax administrations.

In **October 2015**, the OECD published a **15-action plan** with recommendations for involved countries:

- **Action 1:** Addressing the tax challenges of digitalization  
- **Action 2:** Neutralizing effects of hybrid mismatch arrangements  
- **Action 3:** Strengthening controlled foreign company (CFC) rules  
- **Action 4:** Limiting base erosion via interest deductions and other financial payments  
- **Action 5:** Countering harmful tax practices with transparency and substance  
- **Action 6:** Preventing treaty abuse  
- **Action 7:** Prevent the artificial avoidance of permanent establishment (PE) status  
- **Actions 8-10:** Ensuring transfer pricing outcomes align with value creation  
- **Action 11:** Developing methods to obtain data on profit cutting and profit shifting  
- **Action 12:** Developing disclosure regimes for aggressive tax planning  
- **Action 13:** Guidance on transfer pricing documentation and **CbCR**  
- **Action 14:** Improving administrative cooperation in mutual agreement and arbitration proceedings  
- **Action 15:** Developing a multilateral instrument  

Because these measures are comprehensive and complex, this book focuses on **CbCR** (Action 13) as an example of a direct tax reporting requirement.

---

### Country-by-Country Reporting (CbCR)

**CbCR** is designed to give tax administrations an overview of the **global distribution of income and taxes** and indicators of **geographical economic activity** of multinational companies. 

The reporting includes three documentation levels:

- **Master file:** Overview of multinational company’s **business activities** and **transfer pricing policy**  
- **Local file:** Country-specific taxpayer documentation of related-party transactions  
- **CbCR report:** Summary of global financial, tax, and operational data  

The **CbCR report** includes:

- Sales revenue and other income from transactions with **related parties**  
- Sales revenue and other income from transactions with **unrelated parties**  
- Total sales and other income as defined above  
- Income taxes paid during the financial year  
- Income taxes paid and accrued for that year  
- Profit before income taxes  
- Shareholders’ equity  
- Retained earnings  
- Number of employees  
- Tangible assets  

Proper structuring of **ERP data** is essential to obtain the required tax data.

---

### 2.2.2 BEPS 2.0

Despite BEPS efforts, risks remain for adverse global taxation of multinational companies, especially concerning new **digital business models**.

A fundamental reform of the **world tax order** is being discussed to address remaining BEPS risks with two pillars:

- **Pillar One:** Reallocation of taxation rights for largest and most profitable multinational corporations  
- **Pillar Two:** Introduction of a **global effective minimum tax** to combat aggressive tax structuring and harmful tax competition  

---

### Pillar One

This pillar revises **nexus** and **profit allocation rules** in favor of **market jurisdictions** based on a unified approach. It expands taxing rights of market jurisdictions through:

- **Amount A:** Allocation of additional taxing rights to market jurisdictions via a **formulaic apportionment** of residual profits of a **multinational enterprise (MNE) group**  
- **Amount B:** Fixed remuneration for specific marketing and distribution activities physically occurring in market jurisdictions  

**Amount B** follows existing international tax order rules, and requires **physical presence** (e.g., subsidiary or permanent establishment). **Amount A** introduces new nexus and profit allocation rules, enabling jurisdictions to tax **in-scope MNEs** irrespective of physical presence.

**Amount A** is the central response to the tax challenges of **digitalization** by reallocating a part of a MNE’s residual profit.

## New Taxing Right and Nexus for Market Jurisdictions

The **new taxing right** will provide for an additional allocation of taxing rights to **market jurisdictions** using a **formulaic profit split**. This establishes a new **nexus** for in-scope **MNEs** to enable taxation in market jurisdictions where MNEs participate significantly in the economy but without physical presence.

**Amount A** applies to **automated digital services** and **consumer-facing businesses**. The **unified approach** defines the generation of **sales** in a market jurisdiction over a period of years as the primary indicator of significant economic activity. Sales are accounted for whether generated directly through physical presence or via group or third-party sales partners.

The new nexus is designed as a **standalone treaty provision** in addition to the **PE (Permanent Establishment)** definition according to the **OECD model convention**.

---

## Profit Allocation and Amount A under Pillar One

Currently, **profit is allocated to physical presence**, so profit allocation rules need revision to allow allocation regardless of whether taxpayers maintain a **marketing or sales presence (PE or subsidiary)** or conduct business through **independent parties**.

Under **Pillar One**, MNEs apply the existing **transfer pricing system** (supplemented by **Amount B**). **Amount A** applies only to MNE groups with automated digital services and consumer-facing activities exceeding a turnover of **$750 million**, meeting profitability and **de minimis tests**.

Amount A is determined by a combination of the **profit split** and **formulaic apportionment** method:

1. Total profits of the MNE group are identified using **consolidated financial accounts**.
2. The **routine profit** is approximated at an agreed-upon profitability level and excluded from reallocation.
3. The **nonroutine profit** (profit above the routine level) is split via formulaic apportionment into nonroutine profits attributable to market jurisdictions and those from other factors.
4. The deemed nonroutine profits attributable to market jurisdictions are allocated based on **sales**, provided the nexus test is satisfied.

---

## Amount B and Marketing/Sales Functions

**Amount B** operates in the existing system, requiring **physical presence**, and complements the **arm’s length principle**. It enables taxation of **marketing and sales functions** in the source state.

To minimize disputes, **fixed remuneration** will be designed for baseline marketing and sales activities. The fixed remuneration model options include:

- Guaranteed minimum return
- Safe harbor with rebuttable presumption
- Traditional safe harbor
- Risk assessment approach

The remuneration will be set either **regionally** or by **specific industry**.

---

## Pillar Two: Global Anti-Base Erosion (GloBE) Proposal

The rationale of **Pillar Two** (or **GloBE proposal**) is to address remaining **BEPS challenges** and disincentivize **profit shifting** and **tax competition** by establishing a globally effective **minimum taxation** of corporate profits.

The proposal preserves **jurisdictional sovereignty** in tax rate setting but provides tools to tax profits when other jurisdictions apply **low effective taxation** or fail to exercise taxing rights.

---

### Four Interrelated Rules under GloBE

- **Income Inclusion Rule**  
  Supplements **CFC rules** by allowing taxation of low-taxed foreign subsidiary profits in the parent corporation's jurisdiction. Provides a **top-up tax** to ensure a **minimum level of taxation**. The minimum tax rate is designed as a **blended fixed rate**, computed using **financial accounts**.

- **Switch-over Rule**  
  Ensures the income inclusion rule covers **foreign PEs** by allowing the parent jurisdiction to apply the **credit method** instead of exemption if income is untaxed or taxed at low rates. This attributes foreign PE income to the parent corporation's state of residence.

- **Undertaxed Payments Rule**  
  Denies deduction of payments to related foreign corporations that have not been taxed at a **minimum level**, thus taxing the company's profit before deduction of such payments.

- **Subject-to-Tax Rule**  
  Applies **withholding tax (WHT)** and denies benefits from **double taxation agreements** for certain incomes if payments are not taxed at minimum rates. This generally applies to related parties but may extend to unrelated parties for **interest** and **royalties** as per **OECD model convention Articles 11 and 12**. Implementation requires comprehensive amendment of worldwide double taxation treaties.

---

## 2.2.3 Standard Audit File for Tax (SAF-T)

Tax audits are conducted to verify compliance with tax obligations, requiring examination of **accounting records**. Multinational companies face complexity due to varying tax laws and different ERP systems.

Tax audits check if businesses have paid the **correct tax** on time under local tax law. Auditors gather **audit evidence** using compliance and substantive testing against accounting records and source documents.

---

### Audit Approaches and Challenges

- Compliance testing ensures transactions are authorized and properly recorded in the **ERP system**.
- Ineffective compliance testing leads to **substantive audit procedures**.
- Use of electronic files calls for auditors to understand taxpayers’ **ERP systems** beyond paper documentation.
- ERP-based audit approaches provide an effective methodology for future audits.

---

### SAF-T Definition and Benefits

The **OECD Committee on Fiscal Affairs (CFA)** defined **SAF-T** to allow standardized electronic testing of accounting data for tax audits. It enables:

- Testing electronic accounting data in a **structured and standardized** format.
- Compatibility with SMEs to MNEs using **standard audit software**.
- Internal and external auditors can use ERP data electronically, reducing reliance on paper files.
- Possibility of **remote audit procedures**, lowering costs for tax authorities and businesses.
- Early identification and quantification of accounting errors at the **line level**.
- Efficient allocation of audit resources and identification of high-risk audit areas.

---

### SAF-T Data Areas

SAF-T collects data from:

- **General ledger**
  - Journal accounts
- **Accounts receivable**
  - Customer master files
  - Invoices
  - Payments
- **Accounts payable**
  - Supplier master files
  - Invoices
  - Payments
- **Fixed assets**
  - Asset master files
  - Depreciation and revaluation
- **Inventory**
  - Product master files
  - Movements

---

### SAF-T File Format

- Aims for a widely used and easy-to-read international **file format**.
- The current **XML definition** is the reference format.
- Relevant when considering requirements during an **SAP S/4HANA** transformation.

---

## 2.2.4 Financial Reporting Standards

Direct tax requirements arise from external financial reporting under **US-GAAP** and **IFRS**. The objective is to provide useful information for investors and stakeholders to make business decisions.

---

### Tax Position vs. Financial Reporting

- The **tax position** presented in financial reporting often diverges from the **financial position** due to **temporary or permanent differences** in tax and financial accounting.
- Financial accounting focuses on a **true and fair view**.
- Tax accounting emphasizes **tax policy**, e.g., early taxation of profits or tax incentives like **accelerated depreciation**.
- Financial reporting usually involves **global consolidated group financials**, whereas tax position is determined at the **single entity level** using local tax regulations.

## Tax Liability Recognition and Reporting

Financial reporting standards require an entity to recognize a **current tax liability (asset)** for **unpaid (overpaid) taxes** for current and prior periods and a **deferred tax liability (asset)** for all **temporary differences** between the tax base of an asset or liability and its carrying amount in the financial position statement.

- **Tax liabilities (assets)** for current and prior periods are measured at the amount expected to be paid to (recovered from) the taxation authorities using the tax rates that have been (substantively) enacted by the end of the reporting period.
- A **deferred tax liability** arises if an entity will pay tax if it recovers the carrying amount of another asset or liability.
- A **deferred tax asset** arises if an entity will pay less tax if it recovers the carrying amount of another asset or liability or has unused tax losses or unused tax credits.

To align financial and tax positions, further information must be provided on how the tax position has been determined. This mainly includes:

- Calculation of current and deferred taxes
- Preparation of notes and disclosures
- Reporting of tax risks for group reporting purposes

---

## 2.2.5 Tax-Relevant Reporting Requirements

Although requirements differ by reporting scenario, significant key **tax information** relevant to each scenario includes:

- Split into **intra- and extra-group revenues**
- Information on **taxable income**, including **nondeductible business expenses** and **tax-exempt income**
- **Statutory-to-tax adjustments** in accounting
- Split into **material and immaterial assets**
- **Tax relevant movements** in equity accounts (e.g., capital contributions, dividends paid)
- **Tax receivables and liabilities**
- **Tax expense**
- **Tax cash flow/taxes paid across jurisdictions**
- **Taxable income across jurisdictions**
- **Taxes paid across jurisdictions**
- **Deferred taxes**
- **Nominal and effective tax rate**
- **Tax rate reconciliation**
- **Tax forecast**
- **Tax loss carryforwards**
- **Tax risks**
- **Entity structure**

The higher the complexity of **direct tax requirements**, the more important is a global, standardized approach to managing direct taxes. A key building block is a **harmonized tax data model** established by setting your **ERP** as the single source of truth for direct tax data.

SAP S/4HANA supports a wide range of direct tax activities covering the end-to-end process:

- Data capturing and tax determination
- Analysis and monitoring
- Tax filing and reporting
- Forecast and planning

These activities include:

- Implementing a **tax ledger** for a harmonized single source of truth across parallel accounting standards (from group reporting to statutory reporting to accounting for taxes)
- Setting up a **tax-enabled chart of accounts** for compliance and reporting data granularity
- Standardizing **postings on tax accounts** by adding tax attributes such as **tax period**, **tax type**, and **tax authority** for tax management reporting
- Adding **tax attributes** to other tax-relevant business transactions for tax tagging across multiple reporting requirements
- Implementing the **extended WHT function** to automate **Withholding Tax (WHT)** determination and improve filing processes
- Introducing **tax forms** to capture tax data in a standardized format
- Defining **tax data marts**, **tax reports**, and interfaces to make tax data available and build connectors to global tax authorities
- Setting up **integrated preventive tax controls** to ensure tax data quality
- Performing **tax data analytics** (detective controls) to check for **e-audit readiness**

---

## 2.3 Summary

This chapter provided an overview of the **fundamental characteristics of indirect and direct tax systems** in key global regions relevant to SAP S/4HANA implementations. 

- Covered **standard periodic indirect tax reporting** and **transaction-based reporting requirements**
- Discussed principles of **direct tax systems** and their impact on tax requirements of **legal entities** and **individuals**
- Emphasized the importance of understanding regional tax requirements as a key indicator of project complexity in SAP S/4HANA transformations

The next chapter will focus on **configuring the basic settings for tax** in SAP S/4HANA.

---

## Chapter 3  
### Basic Settings in SAP S/4HANA

In previous chapters, you learned about **direct and indirect taxes** and their implications on the organizational structure in SAP S/4HANA. This chapter explores the **tax-relevant settings in SAP S/4HANA**, their connections, and interrelations.

The **finance settings** in SAP S/4HANA form the basis for **tax accounting**, covering:

- **Tax codes**
- **Tax accounts**
- **Exchange rates**
- **Plants abroad setting**

Settings in **Sales and Distribution** and **Materials Management** modules underpin **indirect tax determination** and enrich master data with tax-relevant indicators. This chapter finally addresses the integration between individual tax functionalities.

---

### 3.1 Financial Accounting

SAP S/4HANA consists of several key functionalities (modules). Among these, **financial accounting** is crucial for tax because:

- **Accounting documents** in financial accounting are the basis for most tax reporting.
- All **tax reporting-relevant settings** reside here:
  - Definition of all tax-relevant transactions via **tax codes**
  - Configuration of **tax accounts** to fit company needs
  - Settings related to **registrations or establishments abroad**
  - Configuration of **exchange rates** per jurisdictional requirements

SAP also offers special financial accounting configurations for complex jurisdictions like **Brazil**, **Argentina**, **Hungary**, **South Korea**, and others. Due to strong localization, these are not detailed here.

---

### 3.1.1 Tax Codes

A **tax code** captures the **tax treatment** of a business transaction. Each tax code generally contains:

- **Tax rate**
- **Description**
- **Tax account**
- Additional tax-relevant information for a scenario

In SAP S/4HANA, tax codes are available for:

- **Withholding Tax (WHT)**
- **Output tax** (sales)
- **Input tax** (purchasing)

We will also examine **tax procedures** for indirect tax, which form the basis for tax code structure.

---

### Tax Codes for Withholding Tax (WHT)

Withholding Tax is a **tax withheld from payments remitted to suppliers** and can be divided into two categories:

- **Basic WHT**
- **Extended WHT**

#### Basic Withholding Tax

- WHT codes differ significantly from indirect tax codes.
- Created centrally on a **country-by-country basis**.
- Access settings via Transaction **SPRO**:

  `Financial Accounting • Financial Accounting Global Settings • Withholding Tax • Withholding Tax • Calculation • Maintain Tax Codes`

- To **create** a new WHT code, click **New Entries**.
- To **view/edit** an existing WHT code, double-click it or select it and click **Details**.

Relevant settings for WHT codes include:

- **Off.WH Tax Code**: Official WHT code used in country tax returns.
- **Withholding tax base amount calculation**:
  - **Percentage Subject to Tax**: Percentage of the tax base amount used for WHT calculation (e.g., 100%).
  - **Net base for tax contributions** indicator: Controls if calculation includes or excludes indirect tax.
- **Withholding Tax Rate**: Rate to be withheld, reported, and paid.
- **Reduced Rate**: Reduced WHT rate where applicable.

To post WHT, **debit** and **credit** accounts for **Transaction type QST** must be defined:

- Access via Transaction **SPRO**:

  `Financial Accounting • Financial Accounting Global Settings • Withholding Tax • Withholding Tax • Posting • Define Accounts for Withholding Tax`

- Enter the **Credit** and **Debit accounts** for WHT.
- Posting keys (standard):
  - **40** for debit entry
  - **50** for credit entry

Save changes after configuration.

---

#### Extended Withholding Tax

The **extended WHT** functionality allows:

- Booking of WHT based on a combination of **WHT type** and **WHT tax code**.
- Increased scope of defining WHT matters in SAP S/4HANA.

**Activation** of extended WHT for a company code is required before use:

- Use Transaction **SPRO**:

  `Financial Accounting • Financial Accounting Global Settings • Withholding Tax • Extended Withholding Tax • Company Code • Activate Extended Withholding Tax`

- Click the checkbox in the **Ext.WTax** column next to your company code to activate.

Important notes:

- The decision to use extended WHT should be made at system setup.
- Migration from basic to extended WHT is possible.
- Once activated, extended WHT **cannot be deactivated**.

---

#### Define Withholding Tax Types

Definition of WHT types is the first step:

- Use Transaction **SPRO**:

  `Financial Accounting • Financial Accounting Global Settings • Extended Withholding Tax • Basic Settings • Withholding Tax Type • Define Withholding Tax Type for Payment Posting`

- In the overview, double-click an existing tax type or select **New Entries** to create a new one.

WHT types can also be defined for **invoice posting** as an alternative.

---

## Withholding Tax (WHT) Timing Difference

The key difference is the **timing**:

- A **WHT type for payment posting** posts the WHT at the **time of payment**.
- A **WHT type for invoice posting** posts the WHT at the **time of invoice**.

The choice depends primarily on the **legal requirements** of the jurisdiction.

Double-click the **WHT type** to see its details as shown in **Figure 3.6**. The WHT type contains characteristics for calculating the **tax base amount**:

### Characteristics of WHT Type for Tax Base Calculation

- **Base amount**  
  Radio buttons determine from which value the tax base amount is taken:  
  - **Net Amount**: Uses net amount as the base amount.  
  - **Tax Amount**: Uses tax amount as the base amount.

- **Rounding Rule**  
  Determines how the WHT is rounded. See **Chapter 4, Section 4.1.2** for details on rounding rules.

- **Cash discount**  
  Radio buttons define the timing of WHT application relative to cash discount:  
  - **W/tax pre c/dis**: WHT applied before cash discount.  
  - **C/disc pre W/tx**: Cash discounts applied before WHT.

- **Accumulation type**  
  - **No Accumulation**: WHT is calculated without accumulation.  
  - Other options add the **accumulated WHT base amount** from the current period.

- **Control**  
  Checkboxes control the WHT type settings, such as whether the tax base amount or tax amount can be entered manually.

---

## Using Withholding Tax in Sales and Distribution

The **WHT type/WHT tax code model** is automatically translated into a **WHT condition type/WHT tax code model**. This enables WHT representation at the **customer** and **material** level.

A **condition type** must be assigned to the WHT type. You can set this using **Transaction SPRO** following the menu path:

```
Financial Accounting
  • Financial Accounting Global Settings
    • Extended Withholding Tax
      • Basic Settings
        • Withholding Tax Type
          • Assign Condition Type to Withholding Tax
```

As shown in **Figure 3.7**, assign the condition type defined in Sales and Distribution to the financial accounting **WHT Type**.

---

## Extended WHT Codes

The second part of the extended WHT functionality involves **WHT codes**. To configure:

1. Use **Transaction SPRO** and follow the path:  
   `Financial Accounting • Financial Accounting Global Settings • Extended Withholding Tax • Basic Settings • Withholding Tax Code`

2. In the overview, double-click a **tax code** to view or edit or click **New Entries** to create one.

In **Figure 3.8**, settings include:

- **Official WHT code (Off. W/Tax Key)**: Code used for official reporting by financial authorities.
- **Base amount**: Percentage of the base amount subject to WHT.
- **WHT rate (WTax Rate)**.

---

### Exemption Reasons

Extended WHT allows defining **exemption reasons**. To manage them:

- Use **Transaction SPRO** and follow:  
  `Financial Accounting • Financial Accounting Global Settings • Extended Withholding Tax • Basic Settings • Define Reasons for Exemption`.

- Click **New Entries** to create new exemption reasons.

- Exemption reasons are customizable. A typical example is a **certificate of exemption** (see **Figure 3.9**).

---

### Account Assignment for WHT

Assign accounts for WHT by using:

```
Transaction SPRO
  • Financial Accounting
    • Financial Accounting Global Settings
      • Extended Withholding Tax
        • Posting
          • Accounts for Withholding Tax
            • Define Accounts for Withholding Tax to Be Paid Over
```

- Accounts are defined based on the combination of **Withholding tax type** and **Withholding tax code**.
- Enter the **credit** and **debit** accounts accordingly (see **Figure 3.10**).  
- Posting keys are usually predefined as:  
  - **40** (debit entry)  
  - **50** (credit entry)

**Save your changes!**

---

### Country-Specific Settings

SAP provides **predefined additional WHT settings** for various countries, including:

- **India**
- **Ireland**
- **South Korea**
- **Argentina**
- **Brazil**
- **Colombia**
- **Mexico**
- **Qatar**
- **Great Britain**
- **Public sector in Spain**

---

## Tax Procedures for Indirect Tax

Indirect tax is calculated differently across countries. Tax codes for indirect tax are assigned to a **tax procedure**. The tax procedure defines all info needed to calculate indirect tax, based on **condition types**.

### Accessing Tax Procedure Settings

Use **Transaction SPRO**, menu path:

```
Financial Accounting
  • Financial Accounting Global Settings
    • Tax on Sales/Purchases
      • Basic Settings
        • Check Calculation Procedure
          • Define Procedures
```

(Figure 3.11 shows this screen)

---

### Important Notes

- **Do not change** SAP standard tax procedures and condition types.
- SAP provides **hotfixes** for regulatory changes without adapting the standard.
- If customization is needed, **copy** an existing standard tax procedure and adapt it.

---

### Viewing or Editing Tax Procedures

Select the tax procedure and double-click **Control Data** to view condition types, as in **Figure 3.12**.

The tax procedure:

- Defines condition types for indirect tax calculation.
- Must be assigned to a country for use.

---

### Assigning Tax Procedures to Countries

Via **Transaction SPRO**, use the path:

```
Financial Accounting
  • Financial Accounting Global Settings
    • Tax on Sales/Purchases
      • Basic Settings
        • Assign Country to Calculation Procedure
```

Enter the **tax procedure** in the **Proc.** column for the respective country.

Example in **Figure 3.13**: tax procedure **TAXCN** assigned to **China**.

---

## Tax Procedures without Plants Abroad

- The tax procedure is triggered by the **company code country**.
- Without plants abroad activated, only **one tax procedure** can be used per company code.
- For details on plants abroad, see **Section 3.1.3**.

SAP recommends creating a new tax procedure **TAXEUR** containing specifications for all countries in scope. See **SAP Note 63103** for details.

---

## Preassigned Tax Procedures

- Some countries come with **predefined and preassigned tax procedures**.
- Certain countries offer multiple tax procedures based on the **tax calculation approach** (with or without jurisdiction codes).

---

### Tax Jurisdiction Codes

For regions with indirect tax differences, you define **tax jurisdictions**.

Access settings with **Transaction SPRO**:

```
Financial Accounting
  • Financial Accounting Global Settings
    • Tax on Sales/Purchases
      • Basic Settings
        • Specify Structure for Tax Jurisdiction Code
```

In **Figure 3.14**:

- Define **tax jurisdiction code structure** per tax procedure with jurisdiction code.
- Create new structures with **New Entries**.

---

### Structure Details

- Length (**Lg**) is defined for the levels in the hierarchy within the jurisdiction code.
- **Indicator Tx In**: Defines if taxes are determined line-by-line or at header level.

#### Example: Jurisdiction Codes for TAXUSX

- 2 digits for **state code** (e.g., Alaska = AK)
- 3 digits for **county code** (e.g., area code 907 = Anchorage County)
- 4 digits for **city code** (e.g., ANCH = Anchorage)

Lengths must match customer master's tax data tab.

---

### Multiple Tax Procedures Example

For **Brazil** and the **United States**:

- One tax procedure is for internal **SAP** indirect tax.
- Another is for **external** tax determination, e.g., tax procedure **TAXUSX** works with external engine **Vertex**.

See **Chapter 7** for external tax solutions.

---

### Defining Tax Jurisdiction Codes

Access via **Transaction SPRO** path:

```
Financial Accounting
  • Financial Accounting Global Settings
    • Tax on Sales/Purchases
      • Basic Settings
        • Define Tax Jurisdictions
```

In the screen shown in **Figure 3.15**:

- Use existing configurations or create **New Entries** for jurisdiction codes.
- Codes must follow the defined format, else an error occurs.

---

### Jurisdiction Indicators

Two indicators can be applied:

- **DiN**: Discount base amount is the net value (sales tax not included in discount base).
- **TxN**: Base amount for sales tax calculation is reduced by the discount.

---

## Tax Codes for Indirect Tax

The **tax code** is a vital setting for indirect tax in SAP S/4HANA.

Upcoming sections will cover:

- **Tax code design**
- **Tax code maintenance**
- **Reporting dates for indirect tax**

## Tax Code Design

To determine the appropriate number of **tax codes**, consider the following:

### Indirect Tax Rate

In **SAP S/4HANA**, a **tax code** can only have exactly one **indirect tax rate**. Therefore, there must be one **tax code** per **indirect tax rate**. 

**SAP** has recently started rolling out **time-dependent taxes** functionality for on-premise **SAP S/4HANA**. In **SAP S/4HANA Cloud**, this functionality is already available. Historically, it wasn’t possible to change the **indirect tax rate** of a **tax code** in an **audit-proof** way. Therefore, when changing the **tax rate**, the **tax rate** of past transactions was also changed, even if the **tax rate** on those transactions had been correct.

This led to the need to create new **tax codes** every time a **tax rate** was changed in a jurisdiction. Some jurisdictions have very frequent **tax rate** changes, leading to a high number of **tax codes** that may become obsolete rather quickly.

**Time-dependent taxes** make it possible to change the **tax rate** of the **tax code** in an **audit-proof** and **history-proof** manner.

### Reporting in the Periodic Indirect Tax Return

The **SAP best practice** approach is to create at least one **tax code** for each **business transaction** that must be reported in different fields of the **indirect tax return**. For example:

- One **tax code** for **tax exempt domestic supplies** of certain goods
- One **tax code** for **tax exempt cross-border supplies**

if these two transactions are to be reported separately.

### Reporting in the Annual Return

In addition to the **periodic indirect tax return**, it may be useful to differentiate between cases where a transaction must be reported differently in the **annual return**.

For example, in **Ireland**:

- **Domestic purchases** are reported in field **T2** of the **VAT3** value-added tax (VAT) return.
- In the **return of trading details (RTD)** in the **annual return**, purchases for resale are reported in field **R1** of the RTD, and purchases not for resale are reported in field **R2** of the RTD.

### Real-time Reporting

With **real-time reporting** becoming more popular worldwide, transactions must often be reported at the time of issuing an invoice. This reporting generally contains:

- A large amount of information
- Mapping of the **tax code** of the transaction to a **nature code** of the real-time reporting obligation

There should be enough **tax codes** to enable reporting of all relevant **nature codes** in the real-time reporting requirements.

### Nature Code

A **nature code** is defined by the tax authorities to show the **nature of a transaction**. It is usually alphanumerical and describes reasons such as:

- Standard sales
- Reduced-rate sales
- Reason for tax exemption

For example, in **Spain**, all taxable transactions must be reported in the **Immediate Supply of Information (Suministro Inmediato de Información - SII)** with a code explaining the cause of exemption. Since this reporting must be automated and in real time, this mapping must be available.

### Exemption Reasons on the Invoice

In many jurisdictions, an explanation—the **exemption reason**—must be printed on the invoice if no **indirect tax** is applied. It is often useful to trigger the printing of the invoice text via the **tax code**.

For example, if all requirements are fulfilled, an **export from Germany** is **tax exempt**. This must be indicated on the invoice, for example:

> “Export supply is tax exempt according to Art. 146 (1) a, b Directive 2006/112/EC”.

## Maintain Tax Codes

To maintain **tax codes** for indirect tax, use **Transaction SPRO**, and follow the path:

**Financial Accounting** • **Financial Accounting Global Settings** • **Tax on Sales/Purchases** • **Calculation** • **Define Tax Codes for Sales and Purchases**

Alternatively, use **Transaction FTXP**.

In the popup, enter the **country** for which you want to create the **tax code**. On the following screen, enter the two-digit alphanumeric code defined as a **tax code** (e.g., Tax Code C1).

If the **tax code** was already created, **SAP** will lead you to the tax code settings view.

Here, you can see the settings of the **tax code**, which are triggered by the settings made earlier in the **tax procedure**—you’ll recognize the condition types for tax procedure **TAXCN**.

The **tax type** is entered in the **Properties** settings.

To maintain the **tax code**, enter the appropriate **rate** in the relevant line under **Tax Percent. Rate**. For example, when creating a **tax code** for **output VAT** in **China**, the tax rate is entered in the **Output Tax** line.

### Reverse Charge

**Reverse charge** generally means that the **recipient** of a certain good or service is liable for **indirect tax**, unlike usual transactions where the **supplier** pays and collects the indirect tax.

Usually, the indirect tax due can be instantly deducted by the liable party.

To create a **reverse charge tax code**:

- Enter the output tax amount in a line for **output tax**
- Enter the corresponding deductible tax in a line for **input tax**

The **From Lvl** column determines from which **Level** the amount is deducted for the **input tax**.  

For example:

- Deductible input tax entered in the **Input tax** line would be deducted from **Level 100** — the **tax base amount** (e.g., enter “-13,000”).
- For a **service tax**, output tax is entered in **Service Tax Cred. CN** line and deductible input tax in **Service Tax Deb. CN** line. The debited amount is calculated from **Level 300** (e.g., enter “-100,000” to deduct 100% from Level 300).

If the tax code is new, **SAP** will prompt with a popup to enter the **Properties** overview.

### Tax Code Properties

In the **Properties** overview, enter a **description** of the **tax code**. It should be concise and explain the nature of the underlying transaction, for example:

- “China - 13% Output VAT”
- “CN Output VAT standard rate”

For **tax-exempt** or **nontaxable tax codes**, a more detailed description or a legal code reference can be included, e.g.:

- “DE – Local RC - §13b(2)Nr.4” for local reverse charges in Germany.

Additional options in the **Properties**:

- **Tax Type**: Differentiates **output tax (A)** from **input tax (V)**; relevant for assigning **tax accounts**.
- **CheckID**: If set, an error message appears if the tax amount is incorrect at booking; otherwise, a warning appears.
- **EU Code/Code**: Determines whether the **tax code** is relevant for **European Commission (EC) Sales List/EC Purchase List** reporting.

#### EU Code Indicators:

- Blank: Not EU relevant
- 1, 3, 4, M, H: Output tax codes at 0% relevant for EC Sales List
- 5, 6, 7, 8: Input tax codes for EU acquisitions or reverse charge relevant for ESL (6 is for Hungary)
- 9: Input tax codes for acquisition tax, expecting total of conditions to be exactly 0
- A: Input tax codes for reverse charge not relevant for EC Sales List
- B: Tax codes not EU related and not requiring a zero balance

### Special EU Codes

There are additional **EU codes** for **Spain** and **Poland** for special transaction types. For details, refer to SAP Notes:

- 2402710
- 732750
- 1619948

### Target Tax Code

A **Target Tax Code** can be defined (e.g., for **deferred tax**), creating a relationship between two tax codes. Bookings with one **tax code** can be converted into another.

If a **tax code** references a **target tax code**, an interim **deferred tax account** should be used. The tax can be deferred from the original to the target tax code, and from the interim to the final tax account, using:

- Report **RFUMSV50**
- Transaction **S_AC0_52000644**

More details are in **SAP Note 1800344**.

### Tol.per.rate

Defines the **percentage tolerance rate** accepted between a calculated tax value and an entered tax value.

### Reporting Cntry

Only available if **Plants Abroad** is active. Specifies the jurisdiction in which tax base amounts and indirect tax amounts must be reported.

### Inactive

Marks a tax code as **inactive**. Tax codes should never be deleted for audit reasons. Marking as **inactive** prevents bookings with this tax code.

### MOSS TaxRepCtry

The **Mini One Stop Shop (MOSS)** is a special EU scheme allowing taxes to be paid in the country of residence even if the transaction happened abroad. It is useful for small companies providing electronic services or e-commerce.

### Tax Accounts for Tax Code

To maintain the **tax accounts**:

- Click the **Tax accounts** button at the top of the tax code maintenance screen.
- Enter the **chart of accounts**.

An account must be assigned for every active line:

- For **output tax**, enter an account.
- For **reverse charge tax codes** where input and output tax are booked at the same time, both input and output tax accounts are needed.

### Tax Reporting Date

The **tax reporting date** (**technical field VATDATE**) is when the indirect tax is due or must be reported to tax authorities. Normally, it matches the posting date or document date, but it can be adapted.

To change it, the setting must first be activated in the **global settings** of the **company code**.

(For more information, see **SAP Note 1232484**.)

## 3.1 Financial Accounting

To reach this overview, use **Transaction OBY6**, **Transaction SM30** with view **V_001_B**, or **Transaction SPRO**, and follow path:

- Financial Accounting
- Financial Accounting Global Settings
- Global Parameters for Company Code
- Enter Global Parameters

Double-click the **company code** you want to activate the **tax reporting date** for. On the bottom of the **Processing parameters** box, as shown in Figure 3.20, you’ll find the **Tax Reporting Date Active** checkbox to select.

**Warning!** After activating this checkbox, every **tax-relevant accounting document** that contains a **tax code** must have a valid date in field **VATDATE**. If it’s not tax relevant, this date will be empty.

The actual determination of the **tax reporting date** is realized in **SAP S/4HANA** via a pre-delivered **business add-in (BAdI)**. To check the status of the BAdI, go to path:

- Financial Accounting
- Financial Accounting Global Settings
- Tax on Sales/Purchases
- Basic Settings
- Define and Check Tax Reporting Date

In the configuration menu, the enhancement implementation **VATDATE_VALUES_DEFAULT_SAP** is available.

To check the enhancement implementation itself, use **Transaction SE19**, and enter the name of the BAdI into the **Enhancement Implementation** field. There are two pre-implemented methods:

- **VATDATE_DETERMINE**: To determine the tax reporting date
- **VATDATE_CHECK**: To check the tax reporting date

Enabling the **tax reporting date** also enables you to use a different date for the determination of the exchange rate.

To change the **exchange rate determination** for the company code for which you activated the tax reporting date, use **Transaction OBC8** or **Transaction SM30** with view **V_001_V**. When choosing **Tax Crcy Translation 5: Exchange Rate Determination According to Tax Reporting Date**, the exchange rate for the company code will be determined based on the tax reporting date.

---

### 3.1.2 Tax Accounts

To avoid incorrect bookings into expense or revenue accounts, **tax-relevant settings** can be made in the **general ledger accounts**.

To maintain settings in general ledger accounts, use:

- **Transaction FS00** or 
- Menu path: Financial Accounting → Financial Accounting Global Settings → General Ledger Accounting → Master Data → G/L Accounts → G/L Account Creation and Processing → Edit G/L Account (Individual Processing) → Edit G/L Account Centrally.

Tax-relevant settings are found in the **Control Data** tab. The most relevant field is the **Tax Category**. This indicator determines multiple things:

- Whether the account is **tax relevant** (if it has a tax category assigned)
- This means booking with a **tax code** is allowed or even mandatory
- You can limit the type of **tax code** that can be used (e.g., input tax code should not be used for booking to an output tax account)

Generally, groups of tax codes and all tax codes of the **tax procedure** assigned to the company code are available. Choosing a tax code as the tax category means only bookings with that **tax code** can be made to the account.

Usually, groups of tax codes are used rather than a distinct tax code:

- **+ (Only output tax allowed)**
- **− (Only input tax allowed)**

These groups identify tax type—**A** for output tax and **V** for input tax as defined in the **tax code**.

For **explicit tax accounts** (accounts to which indirect tax is booked), tax categories:

- **< (Input Tax Account)**
- **> (Output Tax Account)**

should be chosen. If a tax category is assigned to a general ledger account, a tax code must be used.

To allow bookings with and without a tax code, set the indicator **Posting without tax allowed**.

In some cases, you may want to assign multiple tax codes to a certain account. This is possible via the menu path:

- Financial Accounting → Financial Accounting Global Settings → Tax on Sales/Purchases → Posting → Assign Country and Tax Code to G/L Accounts

Choose your chart of accounts, then navigate to the **G/L A/c Tax Code** folder and enter the **G/L Account**, **Country**, and **Tax Code** to assign.

If this table is maintained, the system checks whether the tax code is admissible for every accounting document creation.

---

#### VAT Accounts

Generally, there is no limit on the number of tax accounts that can be created. Recommended VAT general ledger accounts:

- **Output VAT**: An account for output VAT.
- **Input VAT**: An account for deductible input VAT.
- **Import VAT**: An account to post import VAT. Usually relates to customs notice and may be connected to other SAP modules like **SAP Global Trade Services (SAP GTS)** or posted manually.
- **Nondeductible VAT**: An account for nondeductible input VAT used for all indirect tax input transactions that cannot be deducted. Options include:
  - **Account key NVV**: Distribute to relevant expense/revenue items. Nondeductible VAT is automatically added to expenses/revenue (no separate account necessary).
  - **Account key NAV**: Separate line item. Booking onto a separate account is necessary.
- **VAT payable**: Define if VAT payable should be posted automatically.

---

### 3.1.3 Plants Abroad

The **plants abroad** feature enables an organization to use plants as representations of **indirect tax registrations** in other countries.

---

#### Importance of Plants in SAP S/4HANA

- **Company codes** represent legal entities.
- **Sales and purchasing organizations** are responsible for sales or purchase of goods/services and hold pricing and contract information like tax codes.
- **Plants** hold inventory or business equivalent, define the primary source for evaluation of departure country, and generally start the country for transport.
- Plants can be assigned to a company code directly or via a sales organization.
- **Storage locations** are physical locations to store inventory within a plant.

---

#### Cross-border Goods Movement and Reporting

When moving your own goods cross-border:

- Inside the EU, transactions lead to a combination of **EU supply of goods** and **EU acquisition of goods**, which must be reported.
- Outside the EU, there is an export in the departure country followed by an import in the destination country.

The **plants abroad** feature is especially useful for:

- One legal entity with indirect tax registrations in multiple countries
- Establishments (e.g., warehouses) in several countries

Refer to **SAP Note 10560** for implementation and impacts.

---

#### Important Notes on Plants Abroad

- Not necessary for automatic postings to the **EC Sales List** and **Intrastat** within the EU.
- Standard report extraction for tax on sales and purchases (**RFUMSV00**) can be done on tax code level.
- The setting is activated/deactivated per company code in financial accounting configuration.
- Alternative solutions (e.g., separate company codes instead of plants abroad) are very complex.
- Additional settings are necessary after activation (e.g., maintenance of VAT IDs abroad, reporting country in tax code).
- Standard setting is designed mainly for use inside the EU but can be adapted.
- Not suitable if branches abroad are separate legal entities. SAP recommends creating your own company codes for permanent establishments (see **SAP Note 1707438**).
- Configuration is also possible with profit centers or business areas.
- Designed for countries without **jurisdiction codes** (see **SAP Note 1929128**).

---

#### Table 3.1 Pros and Cons of Activating Plants Abroad

| **Benefits**                                                                 | **Downsides**                                                                                         |
|-------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------|
| Tax codes are assigned to a certain **reporting country**.                   | Additional settings necessary after activation.                                                     |
| Tax-relevant bookings are assigned via the **tax code** to a reporting country. | Standard use is inside the EU; requires adaptation for other uses.                                 |
| Settings for discounts and currencies are possible at the **reporting country** level, not just company code. | Alternative solutions require complex customizing.                                                 |
| Choose reporting country when running VAT return and EC Sales List programs. | Can’t be used if branches abroad are separate legal entities.                                       |
| Reporting in different currencies supported.                                | Not suitable for countries with jurisdiction codes.                                                |
| Functionality supports cross-border stock transfers with pro forma invoices, supply, and acquisition. | SAP recommends separate company codes for permanent establishments requiring separate financial statements. |

---

#### Example

**Company S** has headquarters in **Romania** and is registered for VAT in **Romania**, **Slovenia**, and **Slovakia**, with warehouses in each country.

The company considers activating **plants abroad** due to multiple benefits:

- Association between tax codes and reporting country
- Automatic conversion to local currency (important as Romania does not use the euro)
- Automatic tracking for the **EC Sales List** and **Intrastat**
- Automatic use of the correct **VAT ID** on transactions

The following sections will walk through the activation, setup, and relevant scenarios for **plants abroad**.

## Activation and Setup

To activate **plants abroad**, use **Transaction SPRO**, and follow this menu path:  
**Financial Accounting • Financial Accounting Global Settings • Tax on Sales/Purchases • Basic Settings • Plants Abroad • Activate Plants Abroad**.  

On the screen, check the **Plants Abroad Active** box and save to activate the functionality (see Figure 3.27).

After activation, the SAP standard table for **country data** receives four additional fields:  
- **Country currency**  
- **Exchange rate type**  
- Two fields to determine the **tax base amount** in relation to **discounts**  

These two fields control whether **cash discounts** are deducted from the tax base amount and whether the **tax amount** is considered for calculating discounts.  

**Warning!** Settings made on a **country basis** when activating plants abroad will overrule settings made for the **company code**.

---

Settings for these indicators per country, as shown in Figure 3.28, can be made with **Transaction OBY6** or through the menu path:  
**Financial Accounting • Financial Accounting Global Settings • Global Parameters for Company Code • Enter Global Parameters**.  

Double-click the respective company code to configure the following settings:  
- **Tax base is net value** (technical field **XMWSN**)  
- **Discount base is net value** (technical field **XSKFN**)  

- When activating **Tax base is net value**, the **tax base amount** is reduced by the discount.  
- When activating **Discount base is net value**, the base amount for discounts excludes **indirect tax**.

---

### Country Settings for Company Code

The activation of these indicators depends on jurisdictional requirements. The following scenarios apply:

- **No indicator activated**: gross tax base and gross discount base. Tax base isn't reduced by discount; discount includes indirect tax.  
- **Only discount base is net value activated**: gross tax base and net discount base. Tax base isn't reduced by discount; discount excludes indirect tax.  
- **Both indicators activated**: net tax base and net discount base. Tax base is reduced by discount; discount excludes indirect tax.  
- **Only tax base is net value activated**: net tax base and gross discount base. Permitted only for specific jurisdictions; tax base is reduced by discount and discount contains indirect tax.

---

To make further settings related to the company code and plants abroad, use **Transaction SPRO** and navigate:  
**Financial Accounting • Financial Accounting Global Settings • Global Parameters for Company Code • Maintain Additional Parameters**.

Since **plants abroad** is a global setting, it affects all company codes. Setting the **Plants Abroad Not Required** flag on the company code level (see Figure 3.29) allows deactivating this global setting for individual company codes.

---

## Value-Added Tax Registration

After activating plants abroad, enter the **VAT registration number** for VAT registrations abroad. Use **Transaction SPRO** and follow:  
**Financial Accounting • Financial Accounting Global Settings • Tax on Sales/Purchases • Basic Settings • Plants Abroad • Enter VAT Registration Number for Plants Abroad**.

- Double-click the combination of **company code and country**, or click **New Entries** to create a new VAT registration.  
- Enter the **VAT registration number (VAT Reg. No.)** and a description for the **Company Name** (see Figure 3.30).

**Note:** The domestic VAT registration number of the headquarters isn't maintained here; it is maintained in the **company code global settings**.

---

### Indirect Tax Registrations Abroad and Stock Transfers

Companies often transfer goods between countries where they hold indirect tax registrations. This requires specific settings and processes for indirect tax determination.

- **Condition records** must be created for conditions: **WIA1**, **WIA2**, and **WIA3**.  

**WIA invoice** (invoice for posting internal stock transfers relevant for indirect tax reporting) contains:  
- **WIA1**: Input tax in the **country of destination**. For example, 21% indirect tax for intra-community acquisition of goods in the Netherlands.  
- **WIA2**: Output tax in the **country of departure**. Example: 0% indirect tax for intra-community supply of goods.  
- **WIA3**: Output tax in the **country of destination**. The WIA1 amount is booked as a negative line, reporting and deducting the acquisition simultaneously.

---

### Cross-Border Movements

If using different tax procedures across countries of registration, create respective **input tax codes** for cross-border movements in the **tax procedure** of the outgoing country.  

Example: For a stock transfer from Italy to France, the **Italian tax procedure** must contain tax codes for both:  
- Italian intra-community supply of goods  
- Intra-community acquisition of goods in France

The **reporting country** is entered in the **properties of the tax code** for reporting purposes.

---

### Non-EU Stock Transfers

Though plants abroad functionality is designed for EU stock transfers, it can also be used for transfers involving **non-EU countries**.

Use **Transaction VOV7**, or menu path:  
**Sales and Distribution • Sales • Sales Documents • Sales Document Item Define Item Categories**.

- The SAP standard item category (**ItCa**) for stock transfer items is **NLN** (see Figure 3.31).  
- Double-click the relevant item category for stock transfers.  

To automate **indirect tax postings** for cross-border transfers to non-EU countries:  
- Set the **billing relevance** of the item category to **relevant for billing**.  
- SAP standard is **J (Relevant for deliveries across EU countries)**; setting this to **A (Delivery-related billing document)** makes it billing relevant for all countries (see Figure 3.32).

---

**Warning!**  
Billing and reporting processes must be adapted to local jurisdiction requirements. It may be necessary to adapt the **pricing procedure** for the creation of the **WIA invoice** to ensure no reverse charge posting occurs for non-EU countries, but an input tax posting is triggered.

This setting affects **all company codes** in the system.

---

After activation, an additional **Reporting Cntry** field becomes available for each tax code (see Figure 3.33).  

The **reporting country** is essential for using the **Tax Reporting Country functionality** in the SAP standard report for **advance return for tax on sales and purchases (report RFUMSV00)** (see Figure 3.34).

Additionally, the **Nat.Crcy Instead of Local Crcy** checkbox becomes available, enabling reports extraction in the **local currency** of the reporting country even when the original transaction currency differs.  

Example: Extract the indirect tax return for Romania in **Romanian leu**, even if the invoice was posted in **euro**.

---

## 3.1.4 Exchange Rates

Most jurisdictions require companies to report **indirect taxes** in the currency of the country. When receiving or sending invoices in different currencies, amounts must be translated using an **exchange rate** (usually to a certain date and rate from institutions like the **Federal Reserve Bank**).

---

### Basic Settings for Exchange Rates

The system generally uses the currency maintained as the **company code currency** in the path:  
**Enterprise Structure • Definition • Financial Accounting • Edit, Copy, Delete, Check Company Code**.

From the popup submenu, select **Edit Company Code Data** (see Figure 3.35).

---

As explained in Section 3.1.3, reporting in currencies other than the company code currency is possible only when **plants abroad** is activated.

---

### Assigning Multiple Currencies to One Company Code

You can assign multiple currencies to one company code by two methods:  

- **Method 1:** Assign currency on the company code level. Limited to a maximum of **two additional currencies**:
  - The **first currency** is always the **company code currency**.  
  - The **second currency** is usually the **group currency**.

Example: A legal entity in the UK (GBP) belonging to a US group may have **USD** as group currency.

- The **third local currency** can be chosen freely but is limited by **currency types**:
  - Company code currency  
  - Group currency  
  - Hard currency (assigned at country level)  
  - Index-based currency (for high inflation)  
  - Global company currency (assigned to company/internal trading partner)

---

Use **Transaction OB22 or FINSC_LEDGER**, or navigate:  
**Financial Accounting • Financial Accounting Global Settings • Ledgers • Ledger • Define Settings for Ledgers and Currency Types**.

- Choose the ledger to customize.  
- Navigate to company code settings for the ledger.  
- Double-click the company code to customize.  

In the overview (see Figure 3.36), three currencies can be defined freely in the **Crcy type** field.  

Example: Only two defined — company code currency (**indicator 10**) for company code 1010 and group currency (**indicator 30**).

---

### Important Fields in Currency Configuration:

- **ExRateType:** Exchange rate type (standard is **M**). Custom exchange rates can be assigned here for special legal requirements.  
- **Currency:** Currency code maintained (e.g., EUR).  
- **Srce Crcy:** Source currency type, defining the basis for translation.  
- **TrsDte Typ:** Translation date type.

---

## Additional Local Currencies for the Company Code

Generally, the **transaction currency** is used as a basis for the calculation, as using another currency (e.g., the **group currency**) can lead to **exchange rate differences**. The **translation date type** defines which date is used for the translation. Options include **document date**, **posting date**, and **translation date**. **Translation date** is the default but may need to be adjusted depending on legal requirements.

In the second option, the **additional local currency** is maintained only for the **advance return on sales/purchases**. To configure this, follow the menu path:

- **Financial Accounting** • **General Ledger Accounting** • **Periodic Processing** • **Report** • **Sales/Purchases Tax Returns** • **Activate Alternative Local Currency for Advance Tax Return**

Select the "**Alternative Local Crcy Active?**" checkbox.

After activation, assign a **currency** and **exchange rate type** to the combination of **company code** and **selection program** via:

- **Financial Accounting** • **General Ledger Accounting** • **Periodic Processing** • **Report** • **Sales/Purchases Tax Returns** • **Specify Alternative Local Curr. for Advance Tax Return**

Click "**New Entries**" to create a new line for your company code. The program name **RFUMSV00** is the standard report for the advance return for tax on sales/purchases. In the example, company code **1010** uses **Polish zloty (PLN)** as the additional currency due to indirect tax registration and reporting requirements in Poland.

Both options are only possible if there is **one additional currency**. If more currencies are relevant, activating **plants abroad** might be a good starting point.

---

## Maintain Exchange Rates

To translate currency amounts from a foreign currency to another currency (e.g., the official in-house or group currency), the **exchange rate** must be maintained in the system. 

Exchange rate maintenance can be reached using:

- **Transaction OB08**  
or  
- Menu Path: **SAP NetWeaver** • **General Settings** • **Currencies** • **Enter Exchange Rates** in the SAP Reference IMG.

The maintained exchange rates can be viewed by **exchange rate type** and **date**. For example, a **daily exchange rate** is maintained for conversions from **HKD to EUR**. 

- The **exchange rate type (ExRt)** is **B** (standard translation at selling rate).
- For each day (shown by the **ValidFrom** date), a rate is entered.
- The example shows an **indirect quotation**, converting **EUR** to **HKD** by defining the cost of one unit of local currency in units of foreign currency (0.84970 HKD = 1 EUR).
- With **direct quotation**, the other currency (HKD) is converted to EUR.

Generally, either direct or indirect quotation works, and only one needs to be defined.

You can maintain **different exchange rates** with different **exchange rate types** for the same period. 

In practice, exchange rates are often automatically uploaded:

- Program **RFIMPNBS** can import exchange rates from an XML file provided by the **European Central Bank** (refer to SAP Note 1286897).
- Program **RFTBFF00** reads exchange rates from market data.

In many jurisdictions, the **prescribed exchange rate** differs from the exchange rate used for accounting. For guidance on using different exchange rate types for the advance return on tax on sales/purchases, refer to **SAP Note 730466**.

---

## Four Steps for Exchange Rate Setup

1. **Activate plants abroad**  
   For details, see Section 3.1.3.

2. **Define exchange rates**  
   Use **Transaction OB07** or follow menu path:  
   **SAP NetWeaver** • **General Settings** • **Currencies** • **Check Exchange Rate Types**.  
   Click "**New Entries**" to create a new exchange rate type by entering the abbreviation in the **ExRt** column and description in the **Usage** column.  
   Example: Creation of a new exchange rate for the **HMRC daily rate** used in Great Britain.

3. **Assign the new exchange rate to the country**  
   In SAP NetWeaver, choose:  
   **General Settings** • **Set Countries** • **Define Countries in mySAP Systems**.  
   Double-click the relevant country and enter the new exchange rate in the **Exch. Rate Type** field.

4. **Set tax conversion rules**  
   Use **Transaction SPRO** and follow menu path:  
   **Financial Accounting** • **Financial Accounting Global Settings** • **Global Parameters for Company Code** • **Enter Global Parameters**.  
   Choose the relevant company code and in the **Processing parameters** box, set the **Tax Crcy Translation** field to **1 (Manual exchange rate entry possible)**.  

This is also where you can change the exchange rate date for the company code if it is different from the pricing date.

---

## Exchange Rates in Sales

The **relevant date for exchange rate determination** in sales is the **pricing date**, which depends on the sales document type. 

Settings can be found under:  
**Sales and Distribution** • **Sales** • **Sales Documents** • **Sales Document Header** • **Define Sales Document Types** (SAP Reference IMG).

In the **Requested delivery date/pricing date/purchase order date** area, the proposed pricing date is based on the **requested delivery date** (field: **Prop.f.pricing date**). If empty, the system uses the current system date.

Alternative options include:

- **A** = requested delivery date  
- **B** = valid-from date  
- **C** = contract start  

Additional settings for **pricing date** can be made in the **condition type** for sales or in **copying control**.

---

### Pricing Date in Condition Types

Access via:

- **Transaction V/06** or  
- **Transaction SPRO** → **Sales and Distribution** • **Basic Functions** • **Pricing** • **Define Condition Types** • **Set Condition Types for Pricing**

In the **Control Data 2** section, the field **Pricing Date** can be set:

- Default: blank (uses pricing date from **PRSDT** or date of services rendered)
- Other options: date of services rendered, billing date, price date, creation date, or order date

Different pricing dates can be set for different conditions. For example, the **net price** might be based on creation date, while tax pricing might use the invoice pricing date.

---

### Exchange Rate in Copying Control

Access via:

- **Transaction SPRO** → **Sales and Distribution** • **Billing** • **Billing Documents** • **Maintain Copying Control for Billing Documents**

You can maintain the **pricing exchange rate type** at the item level during data transmission between preceding documents (e.g., sales order or delivery note) and billing documents.

Options include:

- Blank (default: no special setting)  
- Copy exchange rate determination from sales order  
- Match to accounting rate, billing date, pricing date, current date, or date of services rendered  

**Warning:** This setting only affects the billing document. The exchange rate in **financial accounting** is always based on the **posting date**.

---

## 3.2 Sales and Distribution and Materials Management

Various settings in **Sales and Distribution** (SD) and **Materials Management** (Purchasing) influence **tax determination** for both input and output.

Some settings relate to **master data**, while others concern **tax determination** and **pricing**.

This section covers:

- **Tax classifications**
- Settings related to master data
- The relevance of **indirect tax determination** as part of pricing

---

### 3.2.1 Tax Classifications

**Tax classifications** are the basis for **indirect tax determination**. They are indicators describing characteristics of a product, customer, vendor, or taxable transaction.

---

### Material

Maintaining correct **material master data** is crucial for reliable automatic **indirect tax determination**.

The **material tax classification** in **Sales and Distribution**, and the **tax indicator** for material in **Materials Management**, define the applicable **tax rate** used in pricing to calculate indirect taxes.

---

### Material Tax Classification in Sales and Distribution

The **material tax classification** (technical field **TAXM1**) is a critical indicator in Sales and Distribution.

To maintain it:

1. Use **Transaction MM02** on the **sales organization** level.
2. Enter the material number and press **Enter**.
3. Input plant, sales organization, and distribution channel details, then press **Enter** again.
4. Access the **Sales: sales org. 1** tab to find the **tax data**.

An example shows:

- For **Country Germany** with condition type **TTX1**, the material has tax classification **1 (Full tax)**.
- This indicates the product is subject to the **regular VAT rate** in Germany.

The same product can have different indirect tax rates in different countries. For example:

- Children’s clothing has regular VAT in Germany.
- The same product has a **zero tax rate** in Ireland.

You can assign:

- Tax classification **1 (Full tax)** for Germany
- Tax classification **4 (Zero rate)** for Ireland

Thus, the same material supports tax calculation per jurisdiction.

## Displayed Entries in Table View

The entries shown in the **Tax data table view** for materials are influenced by the **plants** that are available for the **sales organization** in table **TVKWZ** and the **company code**. To check or maintain this relation, use Transaction **OVX6**. 

The **material tax classifications** available can be maintained via Transaction **SPRO**, followed by the path:  
**Sales and Distribution • Basic Functions • Taxes • Define Tax Relevancy of Master Records • Maintain Tax Relevance for Materials**. 

As shown in **Figure 3.47**, the SAP standard offers four material tax classifications for **MWST** or **TTX1**:
- **Exempt, in VATreturn**: For products exempt from indirect tax
- **Full tax**: For products subject to the regular indirect tax rate
- **Reduced Tax**
- **Low Tax**

You can create new material tax classifications using the **New Entries** button.

If you’re using a different tax category, i.e., **condition type** for tax determination, you’ll need to add material tax classifications as well. Best practice is to create as many material tax classifications as there are different indirect tax rates or other characteristics of the material that may lead to different indirect tax treatment.  

For example, it is recommended to create a new material tax classification for **services** as these often have a different place of supply. The condition type **TTX1** listed in the overview in Figure 3.47 as **Tax Categ.**, similarly to the tax procedure you’re already familiar with, must be assigned to a **country**. For details, refer to Section 3.2.3.

---

## Material Tax Classification in Materials Management

Similar to the sales side, the **tax indicator for material** (technical field **TAXIM**) on the purchasing side is maintained in the **material master data**—but this time in the **Purchasing tab** of Transaction **MM02**, as shown in **Figure 3.48**. 

There is only one field for the **material tax indicator** (*Tax ind. f. material*) rather than multiple for different countries as on the output side. This indicator is related only to the plant you’re currently editing the material for—e.g., Plant **1010** in Germany in the example. 

For example, children’s clothing may have **tax indicator material 1 (Full tax)** for Germany and indicator **4 (Zero rate)** for the Irish plant.

---

## Tax Indicator Material for Nonstock Items

Some material master settings don’t permit entering a tax indicator for material. For example, if the material is defined as a **nonstock item**, the field may not be available.

If you want to do **tax determination via condition technique** for these products, you can:
- Define a new **item category group**
- Create a new **condition table**, which includes the material group as a product definition

To use a **tax indicator for a material**, it must be defined first. To check available tax indicators or define new ones, use menu path:  
**Materials Management • Purchasing • Taxes • Set Tax Indicator for Material**.

**Figure 3.49** shows an example of defined material tax indicators. The indicators must be defined by **destination country** and have an **indicator** and a **description**. In the example, Germany has three indicators:
- 0 (Exempt)
- 1 (Full tax)
- 2 (Reduced Tax)

For consistency, maintain the same tax indicators for materials on the purchasing side as you have for material tax classifications on the sales side.

---

## Customer

The **customer tax classification** (technical field **TAXK1** during pricing, technical field **KNVI-TAXKD** in master data) contains information about the characteristics of a certain customer. 

It indicates the respective **VAT treatment** applicable to the customer per tax departure country and follows the same logic as the **VAT ID determination** of a customer (refer to Section 3.2.2).

The **customer tax classification** is maintained at the **customer master level**. To reach customer master data, use either Transaction **XD02** (enter customer) or Transaction **BP** (choose business partner).

Note the **sales and distribution customer number** and the **business partner number** aren’t always the same. In SAP S/4HANA, the **business partner** can be linked to a **supplier number** and a **customer number** simultaneously. Choose the **Customer business partner role** and click **Sales and Distribution** to maintain customer master data for sales and distribution.

You’ll find the setting in the **Billing tab**. The **Output Tax table view** at the bottom of **Figure 3.50** shows customer tax classifications, following the same rules as the **Tax data table** for material master data. This view can be maintained via Transaction **OVX6**.  

For **Country Germany**, this customer has tax classification **1 (Liable for Taxes)**, meaning they are subject to VAT in Germany. The same customer can have different indirect tax treatments in different jurisdictions, defined by the customer tax classification.

---

### Example: Construction Services

In the EU, **construction services** are usually subject to **reverse charges**. If a customer is proven to be a construction provider, they are liable to pay indirect tax instead of the supplier. The customer may receive products from inside or outside the EU, where the **reverse charge regulation** may or may not apply.

---

Similar to material tax classifications, **customer tax classifications** can be maintained via Transaction **SPRO** following the path:  
**Sales and Distribution • Basic Functions • Taxes • Define Tax Relevancy of Master Records • Maintain Tax Relevance for Customers**. 

You’ll reach the screen shown in **Figure 3.51**. SAP standard offers two customer tax classifications for **MWST** or **TTX1**:
- **Tax Exempt**: For tax-exempt customers
- **Liable for Taxes**: For standard customers

New customer tax classifications can be created via the **New Entries** button.

If using a different tax category (condition type), you must add customer tax classifications for it as well. This overview is automatically populated with tax categories defined for the country, showing custom condition types such as **ZTX1** instead of **TTX1**.

Best practice is to avoid using customer tax classifications for characteristics that can be determined otherwise, e.g., it’s unnecessary to create classifications specifically for EU customers since this is done by other parameters.

However, it may be advisable to add customer tax classifications for characteristics such as **construction providers**, subject to reverse charge mechanisms.

---

## Customer Tax Classification in Materials Management

There are **no customer tax classifications** in **Materials Management**.

To use **sales and distribution customer tax classifications for indirect tax determination** in materials management, refer to Chapter 4, Section 4.2.4, and Chapter 6, Section 6.2.

---

## Examples

To better understand the impact of customer and material tax classifications in sales and distribution, consider three examples:

1. A **German company code** supplies a **standard product** via the German sales organization to a German customer.

   - Product has **material tax classification 1 (Full tax)**
   - Customer has **tax classification 1 (Liable for Taxes)**
   - Tax determination in pricing automatically uses these characteristics to determine a tax code with the **19% regular tax rate** via a condition record (Figure 3.52).

   For more on condition records and tax determination, see Chapter 4, Section 4.1.

2. The same German company supplies a **reduced rate product** to the same customer:

   - Product has **material tax classification 2 (Reduced Tax)**
   - Customer has **tax classification 1 (Liable for Taxes)**
   - Tax determination uses these to assign a tax code with the **7% reduced tax rate** (Figure 3.53).

3. The German company supplies a **standard rate product** to another customer, an **embassy** generally **exempt from German VAT**:

   - Product has **material tax classification 1 (Full tax)**
   - Customer has **tax classification 0 (Tax Exempt)**
   - Tax determination uses these to assign a tax code with **0% tax rate** (Figure 3.54).

---

## Plant

The **tax indicator plant** (technical field **TAXIW**) is relevant for **pricing on the purchasing side**. 

It determines whether a plant is **standard and taxable** or **tax exempt**. Tax exemptions are relevant for **free ports** or **customs warehouses**.

To define tax indicators for plants, use Transaction **OMKM** or follow menu path:  
**Materials Management • Purchasing • Taxes • Set Tax Indicator for Plant**.

In **Figure 3.55**, two indicators for plants in Germany appear:
- 0 (Exempt)
- 1 (Taxable)

To add new indicators or countries, use **New Entries**, then enter country, indicator, and description.

After defining tax indicators, assign them to plants using Transaction **OMKN** or menu path:  
**Materials Management • Purchasing • Taxes • Assign Tax Indicators for Plant**.

**Figure 3.56** shows the assignment of tax indicator 1 (Taxable) to plant 1010 in Germany.

---

## Import

Like the tax indicator for plants, the **tax indicator import** (technical field **TAXIL**) is only relevant on the **purchasing side**. 

It describes whether a purchase refers to a **local** or **cross-border transaction**.

## Tax Import Indicators in SAP S/4HANA

There are three values that can be determined for **import indicators**:

- **0 (No import)**: Refers to a **domestic purchase**.
- **1 (Import)**: Refers to a **cross-border purchase not within the EU**.
- **2 (Import within the EC)**: Refers to an **EU intra-community acquisition**.

This import indicator is determined automatically by function **ME_FILL_KOMP_PO**. The determination depends on the **tax departure** and **tax destination country**:

- If both countries are equal, import indicator **0** (No import) is assigned.
- If the countries differ, import indicator is either **2** (if within the **EU**) or **1** (if outside the EU).

The **tax departure country** is essential and generally determined by the **supplier’s country**. If a **goods supplier role** is maintained, then the tax departure country is that of the **goods supplier**.

---

## Account Assignment and Tax Indicators

The **tax indicator for account assignment** specifies whether an item affected via a certain **auxiliary account** is subject to **indirect tax**.

- This allows calculation of **indirect tax in purchasing** based on the use of procurement process.
- Differentiation is useful when **indirect tax depends on purchase reason**, such as resale or asset acquisition.

### Maintaining Tax Indicators for Account Assignment

- Use **Transaction OMKL** or menu path:  
  `Materials Management • Purchasing • Taxes • Set Tax Indicator for Account Assignment`.
- In the screen shown in **Figure 3.57**, you can view and maintain tax indicators.
- Example for **Germany**:  
  - **0 (Exempt)**  
  - **1 (Taxable)**
- To add new indicators or countries, use the **New Entries** button.

### Assigning Tax Indicators to Account Assignment Categories

- Use **Transaction OMKO** or menu path:  
  `Materials Management • Purchasing • Taxes • Assign Tax Indicators for Account Assignment`.
- Example shown in **Figure 3.58**:  
  - For **account key K** and destination country **Germany**, tax indicator **1 (Taxable)** is assigned.

---

## Examples of Tax Indicators in Purchasing

### Example 1

- A **Swedish company** purchases from a **Finnish vendor**.
- Goods have **tax indicator material 1 (Full tax)**.
- Delivered to a **standard warehouse in Sweden** (**tax indicator plant 1 (Taxable)**).
- System determines **import indicator 2 (Import within the EC)**.
- This qualifies as an **intra-community acquisition** with an **acquisition tax code**.  
*(See Figure 3.59)*

### Example 2

- A **Swedish company** purchases from a **Swedish vendor**.
- Goods have **tax indicator material 1 (Full tax)**.
- Supplied to a **standard warehouse in Sweden** (**tax indicator plant 1 (Taxable)**).
- System determines **import indicator 0 (No import)**.
- This is a **domestic purchase subject to Swedish domestic VAT**.  
*(See Figure 3.60)*

### Example 3

- A **Swedish company** purchases from a **US vendor**.
- Goods have **tax indicator material 1 (Full tax)**.
- Supplied from the **United States** to a **standard warehouse in Sweden** (**tax indicator plant 1 (Taxable)**).
- System determines **import indicator 1 (Import)**.
- Subject to **import VAT in Sweden**.  
*(See Figure 3.61)*

### Example 4

- A **Swedish company** purchases from a **US vendor**.
- Goods have **tax indicator material 1 (Full tax)**.
- Supplied to a **customs warehouse in Sweden** (**tax indicator plant 0 (Exempt)**).
- Transaction is **outside the scope of VAT in Sweden**.
- An **outside scope of VAT tax code** is determined.  
*(See Figure 3.62)*

### Example 5

- A **Swedish company** purchases goods classified as **samples with no commercial value**.
- Goods have **tax indicator material 0 (Exempt)**.
- Delivered to a **standard warehouse in Sweden** (**tax indicator plant 1 (Taxable)**).
- This is **outside the scope of VAT**, so an **outside scope of VAT tax code** applies.  
*(See Figure 3.63)*

---

## Vendor: Withholding Tax (WHT)

You can assign **WHT types**, **WHT codes**, and **exemptions** to vendors for **automatic WHT determination**.

- The **customer tax classification** is maintained on the **customer master level**.
- To maintain, use **Transaction XK02** or **Transaction BP** for business partners.
- Note: **Business partner** numbers can link supplier and customer numbers simultaneously.
- Select **business partner role Supplier (Fin.Accounting) (New)** and open the **Company Code** section.
- Maintain the **Vendor: Withholding Tax** tab.

### Example

- Transactions with a vendor are subject to **WHT for licenses (WHT type C1)**.
- WHT code: **W3** (WHT license 10%).
- Recipient type:  
  - **JP** (legal person)  
  - **NP** (natural person)
- An exemption of 5% applies from 2020 to 2022 with a valid certificate (reason **AB**).  
*(See Figure 3.64)*

---

## 3.2.2 Tax Identification Numbers (Tax IDs)

A **tax identification number (tax ID)** is a **unique identifier** for natural or legal persons in tax contexts.

- In many jurisdictions, **B2B transactions require valid tax IDs displayed on invoices**.
- Some countries (e.g., **India**) allow multiple tax IDs for different regions.
- In Europe, **valid VAT ID numbers are required for tax exemptions on cross-border sales**.

### Key Business Partner Roles for Tax ID Determination

- **Sold-to party**: Customer who buys the goods.
- **Ship-to party**: Customer who receives the goods, may differ in chain transactions.
- **Payer**: Customer paying the invoice, often same as sold-to.
- **Bill-to party**: Receives the invoice, usually payer or sold-to party.  
  SAP standard does **not** determine tax ID based on bill-to party.

### Maintaining Tax IDs

- Tax IDs for customers and vendors are maintained in **business partner master data**.
- Use **Transaction XD02** or **Transaction BP**.
- Business partner numbers may link supplier and customer numbers simultaneously.
- Select business partner role **Business Partner (Gen.)** and open the **Identification tab**.
- One business partner may have multiple tax IDs but cannot have two identical tax IDs.

### Validating Tax IDs

- Use **Transaction OY17** or path:  
  `SAP NetWeaver • General Settings • Set Countries • Set Country-Specific Checks`.
- Define **Length** and **Checking rule** for tax numbers.
- Nine checking rules exist, with **Checking rule 9** providing country-specific format validation.
- Enable the **Other data** checkbox for system validation using assigned programs in **V_TFKTAXNUMTYPE**.  
*(See Figures 3.65 and 3.66)*

## 3 Basic Settings in SAP S/4HANA

### Maintain Tax ID of Business Partner

If a certain **tax ID** isn’t available, you can maintain it via **Transaction SM30**. In the **Table/View field**, enter **V_TFKTAXNUMTYPE**, and click the **Maintain** button.

You’ll find that many **tax number categories**—the equivalent to a certain type of tax number in a jurisdiction—are already available. For those that aren’t available, SAP may offer an **SAP Note** (e.g., **SAP Note 2998790** for the VAT ID in Northern Ireland).

To add a new tax number category, click the **New Entries** button, and enter the **Tax Number Category**, **Name**, and **Funct.Name**. The function name is the underlying proofing function used for the validation of the tax number, based on checks such as **length**, **format**, **check sums**, and **gaps** for each country. Occasionally, these rules change or must be adapted due to regulatory changes, such as the introduction of new dummy **VAT IDs** in Europe for Intrastat reports on chain transactions.

Via **Transaction SM30** in the **V_TFKTAXNUMTYPEC** field table/view, you can maintain **duplicate checks for tax IDs**. When a new tax ID for a customer or vendor is created, the system checks whether this tax ID is already maintained for a different customer. This may be useful as tax IDs are generally unique.

### Online Validation

SAP allows you to implement **online validation** both on the master data and transactional levels. For details, refer to **SAP Notes 2976739 and 2975262**.

---

### Value-Added Tax ID Determination

The customer's **VAT ID** number is necessary within the **EU** for several reasons:  
- Perform tax-exempt **intra-community supplies** and B2B services  
- Fulfill **invoice requirements** (local or EU supplies)  
- Fulfill **reporting obligations** (e.g., EC Sales List)

Usually, the **VAT ID of the customer** for the place of supply is required.

---

### Determination Rules

For the determination of the **customer VAT ID**, SAP provides five different determination rules:

#### Rule <blank>

When the field is blank or empty, a hierarchical set of rules is followed:

- **Priority 1:** Validates if the **payer** has a VAT ID in master data. If the payer is not the same as the sold-to party, VAT ID and customer tax classification are taken from the payer based on the tax country of destination.  
- **Priority 2:** If payer and sold-to are the same or payer does not have a VAT ID, the system checks if the **ship-to party** has a VAT ID when the sold-to does not.  
- **Priority 3:** If payer and sold-to are the same and ship-to party has no VAT ID, the system takes it from the sold-to party.

> **Warning!** Despite being SAP’s default, it is recommended to use **rule A** or **rule B** instead due to downsides of incomplete master data and potential for incorrect VAT ID determination.

Example scenario: A Spanish customer (sold-to & payer) orders goods for a Belgian customer (ship-to). Because of this rule, the Spanish VAT ID is chosen over the Belgian VAT ID, even though the Belgian VAT ID would be preferred.

#### Rule A

The VAT ID and customer tax classification are generally taken from the **sold-to party** according to the tax country of destination.

#### Rule B

The VAT ID and customer tax classification are generally taken from the **payer** according to the tax country of destination.

#### Rule C

Corresponds to the standard priority rule <blank> but if the sold-to and ship-to are different and the sold-to has a VAT ID for the tax determination country, that VAT ID is chosen rather than the residence country VAT ID.

#### Rule D

The VAT ID and customer tax classification are generally taken from the **ship-to party** according to the tax country of destination.

The rule for VAT ID determination can be maintained on the **sales organization level** via **Transaction SPRO** and the menu path:  
**Sales and Distribution • Basic Functions • Taxes • Maintain Determination of Value-Added Tax Registration Number**.

---

### Determination Extensions

SAP offers **user exits** in programs **V05EZZRG** (extension for rule B [payer]) and **V05EZZAG** (extension for rule A [sold-to party]) as described in **SAP Note 91109**.

Common extensions include cases where the payer or sold-to party:  
- Has a VAT ID for their country of residence but not for the tax destination country. SAP standard would not consider the VAT ID from the residence country, applying domestic VAT instead. Extensions allow considering the residence country's VAT ID if no VAT ID exists in the tax destination country.  
- Is outside the EU with no VAT ID in the country of destination but registered for VAT in another EU country. This may qualify as an **EU triangular deal** and requires checks for an EU VAT ID in the customer master data.

The system workflow for validation tests the VAT ID in the tax destination country first and then attempts the country of residence.

---

### Determination Examples

VAT ID is considered during sales order **pricing** but finally determined only in the **billing document**.

#### 1. Rule <blank>, priority 1

The payer has a VAT ID of the tax destination country and is not the same business partner as the sold-to party.

- VAT ID for billing document originates from the **Payer**.

#### 2. Rule <blank>, priority 2

The payer and sold-to party are the same and have no VAT ID in the country of destination.

- The **ship-to party** has a VAT ID in the country of destination.  
- VAT ID originates from the **Ship-to party**.

#### 3. Rule <blank>, priority 3

The payer and sold-to party are the same; the ship-to party does not have a VAT ID in the country of destination.

- VAT ID originates from the **Sold-to party**.

#### 4. Rule <blank>, VAT ID of the tax departure country

A foreign customer (ship-to party, payer, sold-to party) only has a VAT ID of the tax departure country on master data.

- System treats this as no VAT ID determined because it cannot identify a VAT ID for the tax destination country or foreign residence.

#### 5. Rule B, VAT ID from tax destination country

Under rule B, the VAT ID is determined only from the **payer** role.

- VAT ID from **payer** role according to the tax destination country is used.  
- VAT IDs from sold-to or ship-to parties are never considered.

## Rule B, VAT ID from Residence Country

If there is no **VAT ID** available for the **country of destination**, the **VAT ID** of the **country of residence** of the **payer** should be considered. In Figure 3.80, you can see that the **ship-to party** and, therefore, the **country of destination**, is no longer **France**, but **Czech Republic**.

**Figure 3.80 Rule B, VAT ID from Residence Country: Business Partners**

Note that we’ve implemented the instructions of **SAP Note 91109** with effect on this example. This isn’t SAP standard behavior. In Figure 3.81, you can see that the **tax destination country** differs from the country of the **VAT registration number**. Additionally, you’ll notice that the origin of the **VAT ID** is from **D (Payer (KNAS segment))**. This means that it was taken from the payer, but not for the country of destination.

**Figure 3.81 Rule B, VAT ID from Residence Country: Result**

---

## 7. Rule B, No VAT ID

If the **payer** doesn’t have a **VAT ID** at all, and the **goods movement** is inside the **EU**, the system uses a safety measure and applies **domestic indirect tax**. In Figure 3.82, the payer is from the **United States** and therefore doesn’t have a **European VAT ID**. The goods are being shipped to another **EU country**.

**Figure 3.82 Rule B, No VAT ID: Business Partners**

This is due to the **access requirement** for **cross-border supplies** not being fulfilled. For details on access requirements, refer to **Chapter 4, Section 4.1.5**. In Figure 3.83, you can see that the system tried to determine the **VAT ID** from **partner role B (Payer)**, but there was no VAT ID available.

**Figure 3.83 Rule B, No VAT ID: Result**

---

## 3.2.3 Tax Determination with Pricing

In **SAP S/4HANA**, every item considered during pricing—so the **net price**, **surcharges**, **discounts**, and also **indirect tax** or **WHT**—is represented by a **condition type**. Each type of tax must be represented by a condition type as well.

In the following sections, we’ll discuss settings for **indirect tax determination** and **WHT determination** in pricing scenarios.

### Indirect Tax Determination

**Indirect tax** is a **transactional tax**. For each **sales and purchase transaction**, whether it’s subject to indirect tax and the tax rate need to be determined. During pricing in **SAP S/4HANA**, this process of determining the correct indirect tax on the transaction can be automated. For each sale or purchase, indirect tax is calculated as a part of the price, together with the **net value** of the invoice and any surcharges or discounts.

Most jurisdictions only have one type of indirect tax, such as **VAT** or **goods and services tax (GST)**. However, there are also jurisdictions that have multiple types of indirect tax. In these jurisdictions, the indirect tax is often determined and accumulated on several levels, for example, on the **federal**, **state**, and **city** levels.

For countries where there is only one type of indirect tax, you’ll find that the standard condition type for indirect tax is **MWST** or **TTX1**, depending on the **SAP version** you’re using. For countries where there are multiple indirect taxes, multiple condition types must exist.

These condition types are assigned to the respective country via **Transaction SPRO**, followed by menu path:

- Sales and Distribution
- Basic Functions
- Taxes
- Define Tax Determination Rules

In the overview shown in Figure 3.84, the condition type is represented by the **tax category (Tax Categ.)**. This tax category is assigned to the respective country in an **n-to-n relationship**. One country can have multiple tax categories, and one tax category can also be assigned to multiple countries.

When assigning multiple tax categories to one country, such as for the example of **Canada**, a **sequence (Seq.)** must be added, which determines the hierarchy of the multiple indirect taxes.

**Figure 3.84 Assign Condition Type to Country**

In Figure 3.84, **Australia**, for example, only has one tax category: **MWST**. This means that for sales issued by an Australian sales organization, one type of indirect tax will be determined using condition type **MWST**.

In contrast, **Canada** has three tax categories, **CTX1**, **CTX2**, and **CTX3**. This is due to the special indirect tax system in Canada, where the federal government imposes a base tax (**GST**). Depending on the province, a provincial tax (**PST**) is added. The combination of this is known as **harmonized sales tax (HST)**. Therefore, there are three tax categories for Canada:

- **CTX1** for **GST**
- **CTX2** for **PST**
- **CTX3** for the combination of both (**HST**)

In SAP naming, this is shown as **PST (Base+GST) Cdn**.

For each item in the sales order, invoice, or purchase order, a **pricing procedure** is used. This pricing procedure should include the tax category defined previously, depending on the supplying country.

In the example, for country Australia, one condition type—**MWST**—is determined.

In Figure 3.85, you can see an example of the pricing conditions determined in a sales document—in this case, a sales order. As tax determination happens on the **item level**, this tab can be accessed by double-clicking on the item in the sales order and navigating to the **Conditions tab**.

At the top of the **Pricing Elements table**, you can see the price condition type **PPR0**, followed by down payment condition type **YZWR** and condition type **MWST** for indirect tax.

**Figure 3.85 Pricing Example for a Sales Document**

As a condition type can only be linked to one **access sequence**, there may be situations where you’ll need to create a new condition type for jurisdictions with only one indirect tax. For more details on condition types and access sequences, see **Chapter 4, Section 4.1**.

---

### Example

**Company A** is a resident of **South Africa** using standard condition type **MWST** with the standard condition tables, as it’s sufficient for their needs.

While they are in the course of integrating a recently acquired group, **Company B** in **Great Britain**, into their **SAP S/4HANA** system, they realize that the current setup isn’t sufficient for indirect tax determination in Great Britain.

Company A decides that company B will receive their own **condition type** and **access sequence** to fit their more complex needs.

**Table 3.2 Different Condition Types for Company Codes**

| Company A – South Africa | Company B (Same Group as A) – Great Britain |
|-------------------------|----------------------------------------------|
| **Condition type:** MWST | **Condition type:** ZMWS                      |
| **Access sequence:** MWST | **Access sequence:** ZMWS                    |
| **Condition table A002:** Domestic tax | **Condition table A368:** Domestic tax with region |
| **Condition table A011:** Export tax | **Condition table A002:** Domestic tax          |
|                          | **Condition table A011:** Export tax           |

Note, however, that this isn’t a best practice recommendation and is only meant to illustrate how the need for different condition types for different company codes may arise.

In this case, there would be two different pricing procedures for these two countries. The pricing procedure for company A would include condition type **MWST**, and the pricing procedure for company B would include condition type **ZMWS**.

This is possible within the SAP standard. If you require two different condition types for the same country, there will be coding involved. For details on this, see **Chapter 6**.

---

### Withholding Tax Determination

In comparison to indirect tax determination, there are some differences regarding **WHT determination** in SAP.

For WHT determination in sales and distribution, the WHT isn’t considered via condition records as it is for indirect tax. Instead, the **WHT value** at the time of **invoice posting** is based on the **WHT type** and **tax code** as maintained on the **customer master level**.

You’ve learned about **WHT types** in **Section 3.1.1**.

To use **WHT during pricing**, it must be assigned to a **condition type**.

---

## 3.3 Integration of SAP Modules in Tax Determination

In the previous sections, you’ve learned about the basic settings in **financial accounting**, **sales and distribution**, and **materials management**. However, these modules are rarely involved in the sales or purchasing process alone.

This section will describe the interactions between the modules with relation to **tax**.

### 3.3.1 Materials Management and Financial Accounting

The input process from beginning to end in SAP is often referred to as **procure-to-pay**. This term combines all **procurement-relevant processes**, beginning at the **requisition** until the **payment** of the procured goods or services.

**Figure 3.86 Schematic Overview of SAP S/4HANA Purchasing Process for Indirect Tax**

Generally, the procure-to-pay process in SAP begins in **materials management** with a **purchase requisition**. The purchase requisition is an internal request to procure certain goods or services. Therefore, it already contains **tax-relevant data**: the goods or services that are to be procured and the **requesting plant**. This plant is the **tax destination country**.

After this purchase requisition is approved, a **purchase order** can be created from it. The purchase order is the **external request** to procure certain goods or services and contains—in addition to the information contained in the purchase requisition—information about the **vendor**, **expected pricing**, and **purchasing organization**.

The purchase order is usually the place where the initial **tax determination** happens if you’re using **purchase info records** or **condition techniques** for your indirect tax determination on the input side. For more details on the purchase order and tax determination in materials management, see **Chapter 4, Section 4.2**.

Now you’ve requested the goods or services from your vendor. As they supply the goods to you, you’ll have a **goods receipt**: a notification from the logistics workstream confirming the receipt of the goods you’ve ordered.

Note that there are certain things for which a goods receipt isn’t required, such as **services**.

---

### Integration Flow: Materials Management and Financials

| Materials Management | Financials            |
|---------------------|----------------------|
| Purchase Requisition|                      |
| Purchase Order      |                      |
| Goods Receipt       |                      |
| Incoming Invoice    |                      |
|                     | Accounting Document  |
|                     | Payment Request      |
|                     | Payment Run          |

- **Proposed indirect tax determination**
- **Final indirect tax assignment**

---

Having received the goods or service you’ve ordered via your purchase order, you’ll also receive an **invoice** from your vendor for their supply.

When entering this invoice into your system, you can create a reference to a **purchase order**. This enables you to copy over the **tax code** that was determined on the purchase order.

From this incoming invoice, an **accounting document** is created automatically.

Note that there is **no determination of a tax procedure** while entering the incoming invoice.

For incoming invoices posted in materials management, the **tax procedure of the purchasing company code** is used in all cases.

---

### Example

Let’s look at the example shown in Figure 3.87.

**Company E**, a **Spanish company code** with a **tax registration** in Spain and in Portugal, is purchasing goods from a supplier in **Spain**.

The **tax code** on the incoming invoice is always posted with the tax procedure of the **country of the company code**, **TAXES**.

**Figure 3.87 Example of Tax Procedure Determination in Procure-to-Pay**

This leads to the need for creating every **tax code** of plants in other countries — in this example, **Portugal** — for the country of the company code.

This means that for country **ES**, there will exist a tax code with **Reporting Cntry** (reporting country) **PT**, as shown in Figure 3.88.

## Figure 3.88 Workaround for Tax Procedure Determination in Procure-to-Pay with Plants Abroad

- **Supplier**: Spain  
- **Plant of company E** in Portugal  
- **Plant of company E** in Spain  
- **Tax procedure**: TAXES  
- **Tax code V1** – Domestic purchase of goods in Spain  
- **Tax code PI** – EU acquisition of goods in Portugal  

---

## 3 Basic Settings in SAP S/4HANA

### Incoming Vendor Invoice Dates

When creating a posting for an **incoming vendor invoice** in **materials management** via Transaction **MIRO**, there are two dates available (Figure 3.89):

- **Invoice date**: The date printed on the invoice when the vendor issued the invoice or supplied the goods/service.  
- **Posting date**: The date the invoice was entered into the system.

### Procure-to-Pay Document Dates in Accounting

During the creation of the **accounting document** via Transaction **FB03** (Figure 3.90):

- The **invoice date** is copied into the **Document Date** field.  
- The **posting date** is copied to the **Posting Date** field of the document.

---

The **standard reports** in SAP related to **indirect tax** generally use the **posting date**, not the **document date**. To check the reporting date in the accounting document, click the header icon to see the **Tax Reporting Date** (Figure 3.91). For more details, refer to Section 3.1.1.

---

## 3.3 Integration of SAP Modules in Tax Determination

### Vendor Tax ID Number Transfer

- The **tax ID number** of the vendor may transfer from **materials management** to **financial accounting**, but only if entered during the incoming invoice posting.  
- If not entered, the field remains empty.  
- The **Rep. Country** field in Transaction **MIRO** refers only to the country of the vendor's tax ID and has no indirect tax relevance (Figure 3.92).

---

- The tax ID number is copied to **financial accounting** at the item level.  
- To view it, double-click the line item in Transaction **FB03** and click **Additional Data** to see the **VAT Reg. No.** field (Figure 3.93).

---

### 3.3.2 Sales and Distribution and Financial Accounting

The **order-to-cash** process in SAP encompasses all sales-relevant processes from the **sales order ("order")** to the **receipt of payment ("cash")**.

- Figure 3.94 shows a schematic overview of this process.

---

### Order-to-Cash Process Overview

- Sales orders are rarely entered manually; they are often interfaced via ordering tools or online shops.  
- Preliminary pricing includes **indirect tax determination**, important for customer cost estimates.  

Following the sales order related to goods supply:

- **Outbound delivery** with picking, packing, and posting goods issue occurs.  
- For **services**, logistics processes are generally not required as services are intangible.

Billing Process:

- If delivery-related (goods), invoices are created from the **outbound delivery**.  
- If service-related or order-related, invoices are created directly from the **sales order**.  
- **Delivery-related invoicing** ensures goods dispatch verification, invoicing date alignment with actual supply, and possible adjustments (e.g., delivery address changes).

---

Inside the **billing document**:

- Final pricing and **indirect tax determination** occur based on a defined **pricing procedure** and **condition record**.  
- Condition record relates to the **tax departure country** (usually the supplying plant’s country).  
- The **customer tax classification** and tax number determination are finalized at billing document level.  

An **accounting document** is automatically created in **financial accounting** from the billing document, transferring the **tax code**. Note, no new tax procedure determination occurs during billing posting; the selling company code’s tax procedure is used.

---

### Sales and Distribution Financials Process Flow

- Sales Order  
- Outbound Delivery  
- Picking / Packing  
- Post Goods Issue  
- Accounting Document  
- Receive Payment  
- Report  
- Preliminary indirect tax determination  
- Final indirect tax assignment  
- Billing  

---

### Example: Company E Sales to Spain and Portugal

- Company E: Spanish company code with tax registration in **Spain** and **Portugal**.  
- Sells goods to customers in Spain.  
- Tax code on billing document posts with tax procedure **TAXES** of the company code country (Figure 3.95).  

---

- This requires creating tax codes of plants abroad (e.g., Portugal) for the company code country (Spain).  
- For country **ES**, tax codes with reporting country **PT** will exist.  

---

### Country-Independent Tax Procedures

- Possible to use a country-independent tax procedure such as **TAXEU** for all EU countries.  
- Beneficial for avoiding duplicate tax code creation when plants abroad are active.  
- This tax procedure must be assigned to all relevant countries.

---

### Condition Types in Sales and Distribution vs. Financial Accounting

- Both areas have similar condition types (e.g., **MWST** in Sales and Distribution, **MWAS** in Financial Accounting).  
- Both are required to calculate **indirect tax** in SAP S/4HANA.

In Sales and Distribution:

- Condition types reference an **access sequence** that finds a matching condition record based on transaction characteristics.  

Example:

- Condition record determined for tax departure country **Germany (DE)**, customer tax classification **1 (Taxable)**, and material tax classification **1 (Taxable)** (Figure 3.96).

---

In Financial Accounting:

- Condition types act as **calculation rules**.  
- Example: Condition type **MWAS** defines a 19% calculation on the tax base amount (Figure 3.97).

---

### Relevant Dates in Sales and Distribution (Not All Transferred to Financial Accounting)

- **Billing date (FKDAT)**: Date billing document is processed and posted for accounting. If invoice date defined, billing date is proposed from invoice date calendar (Figure 3.98).  
- Value from Billing Date is copied to **Document Date** in financial accounting; **Posting Date** in financial accounting is when accounting document is created (Figure 3.99).

---

- **Invoice date (FBUDA)**: Usually the **date of supply**.  
  - If no invoice date in calendar, actual goods issue date is used for delivery-related billing; sales order date used for order-related billing.  
  - For **services**, invoice date is date of services rendered.  
  - Found at item level in billing document under **Serv. Rendered** field (Figure 3.100).

---

- **Created-on date (ERDAT)**: Date the record/document was entered into the system.  
  - Visible at billing document header level (Figure 3.101).

- **Pricing date (PRSDT)**: Determines date-related pricing elements (condition records, exchange rates).  
  - Usually set to date of document creation.  
  - Changing pricing date causes document pricing recalculation.  
  - Used, for example, for credit notes referring to periods with different tax rates.  
  - Found on item level under **Item Detail** tab (Figure 3.100).

---

### Pricing Date Determination

- Adjusted at condition type level via Transaction **V/06**.  
- Usual setting is **blank rule** (Standard: KOMK-PRSDT; tax and rebate KOMK-FBUDA).  
- This means:  
  - Prefer **invoice date (FBUDA)** for tax conditions if available.  
  - Otherwise, use **document pricing date** (usually document creation date) (Figure 3.102).

## Other Options for the Determination of the Pricing Date

The pricing date that can be assigned on the **condition level** includes the following rules:

- **Rule A:** Date of services rendered (KOMK-FBUDA)
- **Rule B:** Price date (KOMK-PRSDT)
- **Rule C:** Billing Date (KOMK-FKDAT)
- **Rule D:** Creation Date (KOMK-ERDAT)
- **Rule E:** Order Date (KOMK-AUDAT)

**Figure 3.102** Pricing Date for the Condition Type

© 2022 by Rheinwerk Publishing Inc., Boston (MA)

---

## 3.3 Integration of SAP Modules in Tax Determination

The **tax ID number**, as shown in **Figure 3.103**, is transferred from the **billing document** in sales and distribution to the **accounting document** in financial accounting.

For details on the **tax ID number determination of the customer**, refer to **Section 3.2.2**.

The number is copied to financial accounting on the **item level**.

To see the tax ID number of the vendor in financial accounting:

- Double-click on the line item of the accounting document (accessed via document flow of the sales document or Transaction **FB03**)
- Click on the **Additional Data** button

This will lead to the view in **Figure 3.104**, where you can see the **VAT Reg. No.** of the customer.

**Figure 3.103** Order-to-Cash Tax ID Number in Sales and Distribution

**Figure 3.104** Order-to-Cash Tax ID Number in Financial Accounting

---

## Withholding Tax (WHT) Methods

**WHT can be withheld during:**

- **Payment** (financial accounting method)
- **Invoice posting** (sales and distribution method)

### Financial Accounting Method of WHT

- During **payment**, a WHT line is determined in the sales document and then captured in financial accounting at the time of **payment posting**.
- The WHT **won't be shown in pricing** of the sales and distribution document.

### Sales and Distribution Method of WHT

- During **invoice posting**, WHT is determined in the sales document at the time of invoice posting.
- The WHT amount is calculated as maintained at the **customer master level**, not based on the **condition technique**.
- After determining the WHT type, the system posts the value in financial accounting as a **WHT code**.

---

## 3.4 Smart Forms

**Smart Forms** is a cross-component SAP standard option to generate **PDF outputs** such as invoices, delivery notes, or purchase orders.

They consist of three central elements:

- The **form** itself
- The **report** that populates the form
- The **output type**

The smart form can be viewed and edited via Transaction **SMARTFORMS**.

Example: SAP standard form **LB_BIL_INVOICE** (see **Figure 3.105**).

Clicking the **Display** button leads to the smart form, which is a grid holding different **data fields** and information, created via drag and drop (**Figure 3.106**).

The fields in the form are filled by a **report**. For example, in SAP standard, this is report **SD_INVOICE_PRINT01** for the billing document.

**Figure 3.105** Smart Form Screen

**Figure 3.106** Smart Form LB_BIL_INVOICE

---

### Linking Smart Forms and Reports

- View and edit using Transaction **NACE**.
- Choose the application (e.g., application **V3** for Billing).
- Click the **Output types** button (see **Figure 3.107**).
- Select the **OutputType** (e.g., SAP standard output type **RD00** for invoice) and click **Processing routines** (see **Figure 3.108**).

**Figure 3.107** Application for Output Types

**Figure 3.108** Output Types

---

### Processing Routines for Output Types

- Displays the **Program** for print output and the **smart form** it fills (see **Figure 3.109**).
- The **FORM routine** field refers to the routine responsible for the output, typically **ENTRY**.
- Custom programs may have different routines (e.g., for **Electronic Data Interchange (EDI)**).

**Figure 3.109** Processing Routines for Output Types

---

### Printing Billing Document Output

- Access billing document via Transaction **VF03**.
- On top of the screen, choose **Billing Document • Issue Output To**.
- In the popup (see **Figure 3.110**), select the output type (e.g., **RD00**) and click the **Print Preview** icon to generate the invoice printout.

**Figure 3.110** Billing Document: Issue Output

© 2022 by Rheinwerk Publishing Inc., Boston (MA)

---

## 3.5 Summary

Basic settings for **tax** in **financial accounting**, **materials management**, and **sales and distribution** support many legal requirements and business models.

Key points to consider:

- Understand legal **jurisdiction requirements** and business requirements for SAP system design.
- Decide whether to **standardize system settings** over all company codes or maintain separate settings.
- The design of **tax codes** depends on these decisions.
- A **uniform SAP concept** can increase project complexity but is recommended for maintenance benefits.

The next chapter focuses on **indirect tax determination** in SAP S/4HANA.

---

# Chapter 4  
## Indirect Tax Determination in SAP S/4HANA

So far, basic settings related to tax in **financial accounting** and **sales/materials management** have been discussed.

In this chapter, you will learn about **indirect tax determination** and its importance in SAP S/4HANA.

---

### Importance of Indirect Tax Determination

- Indirect tax determination is critical due to the high volume of taxable transactions daily.
- Manually evaluating each transaction is impossible and errors may lead to significant **corrections** and **fines**.
- Proper setup aligned with legal requirements should be a central part of SAP S/4HANA implementation.

---

### Scope of Indirect Tax Determination

- Covers both the **sales** and **purchasing** sides.
- On the sales side, uses **condition techniques** within **sales and distribution**.
- On the purchasing side, uses either **condition techniques** or **purchase information records** within **materials management**.

---

## 4.1 Condition Logic in SAP

**Indirect tax determination in SAP is based on condition logic (condition technique).**

- Always used in the **order-to-cash** process for sales.
- Applicable in **procure-to-pay** process for purchase order tax determination.

Condition logic hierarchy (see **Figure 4.1**):

- **Pricing Procedure** (price calculation procedure)
- Contains multiple **Condition Types** (including tax condition type)
- Each condition type has an **Access Sequence**
- Access Sequence includes one or more **Condition Tables**
- Access to tables controlled by **Access Requirements**

---

### 4.1.1 Price Calculation Procedure

The **price calculation procedure** aggregates relevant condition types into a hierarchical structure.

This structure determines elements such as:

- Price
- Surcharges
- Discounts
- Transaction-related **indirect tax**

SAP provides predefined pricing procedures for sales and purchasing.

- Access the sales pricing procedure via Transaction **V/08** or menu:
  - Sales and Distribution • Basic Functions • Pricing • Pricing Control • Define and Assign Pricing Procedures
- Example: SAP standard pricing procedure **RVAA01** (see **Figure 4.2**)

Purchasing pricing procedures can be accessed via Transaction **M/08** or menu:

- Materials Management • Purchasing • Conditions • Define Price Determination Process • Set Calculation Schema – Purchasing

---

### Hierarchical Nature of Pricing Procedure

- Calculated starting with condition type in the **step with the lowest number**, continuing sequentially.
- **Condition types** are represented by alphanumerical abbreviations (e.g., **MWST** = Output Tax).
- Example: Condition type **MWST** uses the amount calculated from all prior steps as the **tax base amount**.
- Certain condition types may reference others using the **From reference step** column (e.g., steps 901 to 905 refer to step 400, Rebate Basis).

---

### Settings for Each Condition Type

Each condition type in the price calculation procedure can be configured with checkboxes (see **Figure 4.3**):

- **Manual:**
  - If checked, condition is included only if **entered manually** or transferred externally.
  - Not recommended for condition type **MWST** (Output Tax), which should be **calculated automatically**.
  - Manual entry increases risk of **human error**.

- **Required:**
  - If checked, the condition is **mandatory** during pricing.
  - Highly recommended for condition type **MWST**.
  - If missing, document will trigger error: *Pricing error: Mandatory condition MWST is missing*.

---

### Condition Records

A **condition record** represents a set of parameters leading to a **tax code** that defines the tax treatment of a transaction.

## Making Condition Type MWST Required: Benefits

- **Indirect tax** must always be part of pricing due to **legal or regulatory obligations**. Consequently, the **condition type for tax** should always be included in pricing.
- The **condition type** prevents undesirable or incorrect scenarios. For example, an error will occur during pricing if indirect tax determination is not maintained for a certain scenario, avoiding unrealistic situations sent to the customer.
- **Missing master data** is identified since the condition technique relies on parameters including indicators in the **customer** and **material master data**.

## Statistics and Account Determination

- Setting the **statistical indicator** makes a condition type **statistical**, allowing inclusion of another condition type without changing the pricing procedure value. For example, condition type **GRWR** (statistical value) is used in the **Intrastat report**.
- The **account determination indicator** defines relevance for statistical price conditions in **profitability analysis** and enhances **management reporting** (e.g., frequency of warranties, surcharges, rebates, delivery cost).

---

## 4.1 Condition Logic in SAP

### Further Settings for Each Condition Type

- **Print T… (print type):** Controls output during document printing (sales orders, invoices).
  - `<blank>`: Condition line not printed.
  - **X:** Condition line printed at item level.
  - **S:** Condition line printed in totals block (used for tax condition lines).
  - **Alternative print types** exist but cannot be mixed with standard types **X** and **S**.
  
- **Subtotal:** Controls fields where condition amounts or subtotals are stored. If multiple amounts use the same field, the system sums them.

- **Requir… (requirement):** Condition type is considered only if the requirement is fulfilled. For example, condition type **MWST** uses requirement **10**, which verifies either the supplying plant or tax departure country is set.

- **Plant and Tax Departure Country:** Tax departure country is usually determined from the supplying plant’s country. If plant isn't set, tax departure country cannot be set.

- **Alt. Ca… (routine for alternative calculation of condition amount):** Allows calculation of condition amount via a formula. Example: **DIFF (Rounding Off)** rounds prices to two decimals.

- **Alt. Cn… (routine for alternative calculation of condition base value):** Formula can also determine the base value for a condition. For **DIFF**, routine 4 (Net value + Tax) is used.

- **Accou… (account key):** Determines the type of **general ledger account** for booking transactions. Account key **MWS** is for condition type **MWST (Output tax)** for sales tax or purchases.

- **Accruals:** Key identifying **general ledger accounts** for accruals or provisions.

---

## Pricing Procedures

- Different countries use different **pricing procedures**. For example, **RVAAUS** (U.S.) includes **state**, **county**, and **city sales tax**.

- Some pricing procedures serve specific purposes; for example, **RVWIA1** is for internal stock transfers related to plants abroad, including conditions for output and input tax.

- **Pricing procedure** determination is based on:
  - **Sales area** (combination of sales organization, distribution channel, division)
  - **Document pricing procedure**
  - **Customer pricing procedure**

- Use SAP Transaction **SPRO** to configure under:  
  `Sales and Distribution • Basic Functions • Pricing • Pricing Control • Define and Assign Pricing Procedures`

### Breakdown of Sales Area Components

- **Sales organization:** Central organizational unit responsible for sales, possibly equal to company code or legal entity. One entity may have multiple sales organizations.
  
- **Distribution channel:** Method how product/service reaches the customer (e.g., wholesale, retail, intercompany).
  
- **Division:** Groups materials (products or services).

- **Document pricing procedure:** Specifies pricing procedure per sales document type. E.g., **Document pricing procedure B** for Plants Abroad uses pricing procedure **RVWIA1**.

- **Customer pricing procedures:** Maintained at customer master data level; may differ for private vs. business customers.

- New combinations for pricing can be created by entering the sales organization, distribution channel, division, document pricing procedure, customer pricing procedure, and pricing procedure.

---

## 4.1.2 Condition Types

- **Condition types** represent parts of pricing contributing to the final price of sales or purchasing documents.

- Examples include **NETP (Net Price)**, **BO03 (Customer Rebate)**, or **MWST (Output Tax)**.

- To configure condition types:
  - Sales and Distribution: Transaction **V/06** or  
    `SPRO -> Sales and Distribution • Basic Functions • Pricing • Pricing Control • Define Condition Types`
  - Purchasing: Transaction **M/06** or  
    `SPRO -> Materials Management • Purchasing • Conditions • Define Price Determination Process • Define Condition Types`

- To edit or create condition types:
  - Select an existing type to edit.
  - Copy a similar existing condition type via **Edit • Copy As** for new creation.
  - Alternatively, use **New Entries** to create from scratch.

- Condition types must be included in a **pricing procedure** to be active.
- They typically have an **access sequence** assigned (see Section 4.1.3).
- Some conditions use formulas instead of access sequences, e.g., **DIFF (Rounding Off)** to round prices.

### Condition Type Settings: MWST

- **Condition class (Cond. class):** Identifies pricing type:
  - **A:** Discount or surcharge (e.g., customer rebate)
  - **B:** Prices (e.g., net price)
  - **D:** Taxes

- **Calculation type (Calculat.type):** Defines how condition value is calculated:
  - **A (Percentage):** Used by **MWST**, calculating indirect tax as a percentage of price.
  - **B (Fixed Amount):** Used for nondeductible input VAT.
  - Other types depend on material attributes (quantity, weight, volume).

- **Condition category (Cond. category):** Groups condition types by category:
  - **MWST:** Assigned category **D (Tax)**
  - Nondeductible input tax example: category **N (Input Tax non Deductible)**

- **Rounding rule:** Most use `<blank>` (Commercial rounding), which rounds values to nearest full number:
  - Round down if below 5
  - Round up if above 5

### Example

- 22% of 196.51 = 43.2322
- With **commercial rounding**, result = **43.23**

## Indirect Tax Condition Types and Rounding Rules

There are some **country best practices** where you’ll come across condition types for **indirect tax** with other rounding rules: **A (Round-Up)** or **B (Round-Down)**. For **round-up rounding**, the values will always be rounded **up** to the next full number. For **round-down rounding**, the values will always be rounded **down** to the next full number.

**Example:**  
22% of 196.51 is 43.2322. With **round-up rounding**, the resulting indirect tax value will be **43.24**. With **round-down rounding**, the resulting indirect tax value will be **43.23**.

---

## Group Condition Indicator

In the **Group condition** section, you can see a **Group cond. indicator** that determines whether the system calculates the condition value by taking all related items of the business document into account.

For **indirect tax**, the system will calculate the condition amount on the **header** and **item level** and will compare the values. The **RoundDiffComp field** and the **GrpCond.routine checkbox** aren’t relevant because they relate only to conditions with a **scale value** (e.g., mass discounts).

---

## Changes Which Can Be Made

For the **Manual entries** setting, you can see indicator **D (Not possible to process manually)**. This is a recommended **standard setting** for tax condition types, as **manually changing** the condition value of an indirect tax condition will lead to errors.

Therefore, it can only be calculated by the system and can never be changed manually.

---

## Condition Types Configuration for Indirect Tax

Condition types for **indirect tax** are configured as an **item condition**. This means that the indirect tax is calculated on the **item level** instead of the **header level**. Checking the **Item condition** indicator is important, as a sales order or invoice may include materials with different indirect tax rates associated to them, making it necessary to calculate on the **position level**.

---

## Standard Condition Type

SAP has introduced standard condition type **TTX1** with access sequence **TTX1** for indirect tax. This access sequence includes condition table **165 (Export Taxes Depending on the Billing Category)** instead of condition table **078**.  

See Section 4.1.4 for more information on **condition tables**.

---

## 4.1.3 Access Sequence

**Access sequences** determine the sequence of accesses to **condition tables**. Each access sequence must contain at least one condition table. The condition tables inside the access sequence are always ordered from **most specific** to **least specific**, which is important because the access sequence is **hierarchical**.

The SAP S/4HANA system will go through all condition tables in the access sequence, searching for a **matching condition record**.

---

### Access Sequence Configuration

- To configure access sequences in **Sales and Distribution**, use:
  - **Transaction V/07** or  
  - **Transaction SPRO** → Sales and Distribution → Basic Functions → Pricing → Pricing Control → Access Sequences → Set Access Sequences.

- To configure access sequences in **Purchasing**, use:
  - **Transaction M/07** or  
  - **Transaction SPRO** → Materials Management → Purchasing → Conditions → Define Price Determination Process → Define Access Sequences.

---

### SAP Standard Access Sequences for Indirect Tax

Figures 4.6 and 4.7 show the SAP standard access sequences for **indirect tax** for output tax (**MWST**) and input tax (**0003**), respectively.

- The **No.** field determines the order in which the condition tables are tried.
- The **Tab** field includes the number of the condition table.
- Condition tables in SAP start with an “A” (e.g., **A002**), but this letter is omitted in the access sequence.
- The **Requirement** field includes the access requirement for a condition table (see Section 4.1.4 for details).
- The **Exclusive** indicator determines whether the system should stop searching after finding a match or continue to find a preferable match.

---

### Example: Exclusive Indicator

If the **Exclusive** indicator isn’t set, a **predetermination** can be made in condition table **A078** (Departure Country/Destination Country) for cross-border supplies. Condition table **A011 (Export Taxes)** is more specific as it includes two more indicators apart from the departure and destination country.

If a match is found in **A011**, this is preferred and results in two matching condition records and two lines of indirect tax at the item level.

---

### Standard Access Sequence MWST (Tax on Sales or Purchases)

Contains three condition tables:

- **Table A078 (Departure Country/Destination Country)**
  - Contains fields: tax departure country, tax destination country
  - Assigned access requirement: **8 (export business)**

- **Table A002 (Domestic Taxes)**
  - Contains fields: tax departure country, customer tax classification, material tax classification
  - Assigned access requirement: **7 (domestic business)**

- **Table A011 (Export Taxes)**
  - Contains fields: tax departure country, tax destination country, customer tax classification, material tax classification
  - Assigned access requirement: **8 (export business)**

---

### Standard Condition Table 0003 (Tax Classification) in Materials Management

Contains the following five condition tables in SAP standard delivery:

- **Table A088 (Taxes: Material, Plant, Acc. Assignment, Origin and Region)**
- **Table A094 (Taxes: Material, Plant, Origin and Region)**
- **Table A087 (Taxes: Plant, Account Assignment, Origin and Region)**
- **Table A086 (Taxes: Material, Plant and Origin)**
- **Table A080 (Taxes: Material)**

No **access requirements** are assigned to these condition tables, meaning the system doesn’t pre-differentiate between domestic and cross-border transactions as it does in sales and distribution.

---

## Extend Standard Functionality

In most regions, it’s very uncommon to keep the SAP standard settings for access sequences in indirect tax determination as they don’t cover all relevant requirements.

### Examples of Extensions:

- Supplies to special regions such as **Northern Ireland** or the **Canary Islands** from Europe, requiring the inclusion of the **destination region**.

Refer to Section 4.2.4 (purchasing) and Section 4.3.3 (sales) for typical extensions.

---

### Adding a New Condition Table to the Access Sequence

1. Click the **New Entries** button to add a new condition table.
2. Enter the number of the condition table and the sequence number (No.).
3. Set the **Requirement** and **Exclusive** columns if applicable.

---

### Assigning Fields to the Condition Table

- Select the condition table line.
- Double-click the **Fields** subfolder.
- Assign the respective fields of the **pricing communication structures** to the condition table.

The **pricing communication structures**:
- **KOMK** – Communication header for pricing structure.
- **KOMP** – Communication item for pricing structure.

Assigning correct fields is crucial for **tax determination**.

SAP usually pre-fills these fields. For custom fields, mark the line and use the **Field catalog** button to choose the correct source value.

---

## 4.1.4 Condition Tables

**Condition tables** contain **key fields** that enable the determination of a **tax code** for a specific combination of parameters.

For every **SAP S/4HANA implementation project**, including indirect tax determination, condition tables are central.

This section covers condition tables for **sales and distribution** and **purchasing**, creation of new condition tables, and assignment of data fields.

You will recognize tax classifications and indicators already covered in Chapter 3, Section 3.2.1.

---

### Condition Tables in Sales and Distribution

The SAP standard access sequence **MWST** contains three example condition tables, each with assigned key fields:

- **Table A078 (Departure Country/Destination Country)**
  - Tax departure country (data field **ALAND**, pricing field **KOMK-ALAND**)
  - Tax destination country (data field **LLAND**, pricing field **KOMK-LAND1**)

- **Table A002 (Domestic Taxes)**
  - Tax departure country (data field **ALAND**, pricing field **KOMK-ALAND**)
  - Customer tax classification (data field **TAXK1**, pricing field **KOMK-TAXK1**)
  - Material tax classification (data field **TAXM1**, pricing field **KOMP-TAXM1**)

**Note:** Condition table A002 only has tax departure country as a key field because it is relevant only for domestic transactions, where departure and destination country are the same.

- **Table A011 (Export Taxes)**
  - Tax departure country (data field **ALAND**, pricing field **KOMK-ALAND**)
  - Tax destination country (data field **LLAND**, pricing field **KOMK-LAND1**)
  - Customer tax classification (data field **TAXK1**, pricing field **KOMK-TAXK1**)
  - Material tax classification (data field **TAXM1**, pricing field **KOMP-TAXM1**)

---

### Condition Tables in Purchasing

The access sequence **0003 (Tax Classification)** for indirect tax determination in materials management contains five example condition tables. Their key fields are as follows:

- **Table A088 (Taxes: Material, Plant, Account Assignment, Origin and Region)**  
_(Details continue in further sections)_

## Assignment, Origin, and Region

- **Tax indicator: material** (data field **TAXIM**, pricing field **KOMP-TAXIM**)
- **Tax indicator: plant** (data field **TAXIW**, pricing field **KOMP-TAXIW**)
- **Tax indicator: account assignment** (data field **TAXIK**, pricing field **KOMP-TAXIK**)
- **Tax indicator: import** (data field **TAXIL**, pricing field **KOMP-TAXIL**)
- **Tax indicator: region** (data field **TAXIR**, pricing field **KOMP-TAXIR**)

### Table A094 (Taxes: Material, Plant, Origin, and Region)

- **Tax indicator: material** (data field **TAXIM**, pricing field **KOMP-TAXIM**)
- **Tax indicator: plant** (data field **TAXIW**, pricing field **KOMP-TAXIW**)
- **Tax indicator: import** (data field **TAXIL**, pricing field **KOMP-TAXIL**)
- **Tax indicator: region** (data field **TAXIR**, pricing field **KOMP-TAXIR**)

### Table A087 (Taxes: Plant, Account Assignment, Origin, and Region)

- **Tax indicator: plant** (data field **TAXIW**, pricing field **KOMP-TAXIW**)
- **Tax indicator: account assignment** (data field **TAXIK**, pricing field **KOMP-TAXIK**)
- **Tax indicator: import** (data field **TAXIL**, pricing field **KOMP-TAXIL**)
- **Tax indicator: region** (data field **TAXIR**, pricing field **KOMP-TAXIR**)

### Table A086 (Taxes: Material, Plant, and Origin)

- **Tax indicator: material** (data field **TAXIM**, pricing field **KOMP-TAXIM**)
- **Tax indicator: plant** (data field **TAXIW**, pricing field **KOMP-TAXIW**)
- **Tax indicator: import** (data field **TAXIL**, pricing field **KOMP-TAXIL**)

### Table A080 (Taxes: Material)

- **Tax indicator: material** (data field **TAXIM**, pricing field **KOMP-TAXIM**)

---

## Create a Condition Record

To create a **condition record** in **sales and distribution**, use **Transaction VK11**. In **purchasing**, use **Transaction MEK1**.

When creating a condition record, the **condition type** for which the record is created must be entered because the same **condition table** can be used for multiple condition types.

### Example

For the **plants abroad** functionality, the SAP standard **condition table A011 (Export Taxes)** can be used. The **condition types** for plants abroad are **WIA1, WIA2, and WIA3**.

Choosing the correct **condition type** prompts a popup (see Figure 4.10) to choose the **condition table** where you want to enter the condition records. After choosing the condition table, click the green checkmark icon to enter the record.

### Example of Condition Record for Indirect Tax Determination (see Figure 4.11)

- **Country:** tax departure country is **DE (Germany)** where goods movement starts.
- **Dest. Ctry:** tax destination country is **CA (Canada)** where goods movement ends.
- **TaxCl1Cust:** customer tax classification is **1** (standard, taxable customer).
- **TaxCl.Mat:** material tax classification is **1** (standard, taxable material).

This combination determines **Tax Code A0** with a tax rate of **0%** for export of goods from Germany.

---

## Create a Condition Table

The **standard condition tables** may not suffice for all indirect tax business transactions.

### Configuration Steps in Sales and Distribution

- Use **Transaction V/03** or **Transaction SPRO**
- Follow path:  
  *Sales and Distribution* → *Basic Functions* → *Pricing* → *Pricing Control* → *Condition Tables and Field Catalog* → *Create Condition Table*

### Configuration Steps in Materials Management

- Use **Transaction M/03** or **Transaction SPRO**
- Follow path:  
  *Materials Management* → *Purchasing* → *Conditions* → *Define Price Determination Process* → *Maintain Condition Tables*

Enter the number of the table to create or copy from an existing table.

### Selecting Fields for New Condition Table

Select desired fields from the **Field Catalog** as shown in Figure 4.12.

---

## Technical View Settings for Condition Table

By clicking the **Technical View** button (see Figure 4.13), determine the selection screen when creating **condition records**:

- Fields marked as **Item Fld** become part of the **condition table**.
- Fields not selected become part of the **selection screen**.

Best practice: choose one or two fields, usually the **tax departure country** as the selection field for easier maintenance.

---

## Create a Domain

If the data field requires a new data type with a defined range of values:

- Use **Transaction SE11**
- Choose **Domain**
- Name your domain, preferably starting with **Z** for custom settings
- Click **Create**

### Example

Create a domain to classify if a country is part of a **customs union** (Chapter 6, Section 6.2).

---

### Domain Definition

- Add a **Short Description**
- Define the **Data Type** (e.g., **CHAR** with length 1)
- Assign a **development package** in Properties tab

---

### Value Range Setup

Define fixed values for the domain with descriptions (see Figure 4.16).

---

### Save and Activate Domain

Click **Save** then **Activate** the domain (see Figure 4.17).

---

## Create a Data Element

- Open **Transaction SE11**
- Enter the name for the **Data element**
- Click **Create**, select **Data element** from the popup

---

### Data Element Details

- Provide a **short description**
- Assign the **domain** created earlier
- Assign a **development package**
- Save and activate (see Figures 4.18 and 4.19)

---

## Add Data Element to Structure

Depending on the field type:

- **Header field:** add to structure **KOMKAZ**
- **Position field:** add to structure **KOMPAZ**

Warning: Use **KOMKAZ** or **KOMPAZ** for customer modifications, **not** **KOMK** or **KOMP** which are system tables.

---

### Appending Fields

- Use **Transaction SE11** on **KOMPAZ** or **KOMKAZ**
- Click **Append Structure** button
- Create a new append structure (e.g., **ZITX_DETERMINATION_FIELDS**) (see Figure 4.20)
- Add components with new data elements and domains (see Figure 4.21)
- Save and activate

Example final structure shown in Figure 4.22.

---

## Add Data Element to Field Catalog

Add the new data element to the **field catalog** for pricing:

- Use **Transaction OMKA** or **SPRO**
- Path:  
  *Sales and Distribution* → *Basic Functions* → *Pricing* → *Pricing Control* → *Condition Tables and Field Catalog* → *Change Field Catalog*
- Click **New Entries**
- Enter new field name (see Figure 4.23)
- Save

---

## Materials Management and Sales and Distribution

- **Structures KOMK and KOMP** are used for both modules.
- The **field catalog** is universal for **Materials Management** and **Sales and Distribution**.

## Condition Logic in SAP

You can use the **path** and **transaction code** given earlier for both modules, or you can use the path:  
**Materials Management • Purchasing • Conditions • Define Price Determination Process • Extend Field Catalog for Condition Tables** in the SAP configuration menu to reach it from the materials management settings.

### Populate Data Field during Pricing

Finally, your new field must be populated with a value during **pricing**. If this step is skipped, the field may exist in a condition table but can never have a value.

For details on how to populate the data field within **USEREXIT_PRICING_PREPARE_TKOMK** or **USEREXIT_PRICING_PREPARE_TKOMP** to use the value during pricing and tax determination, see **Chapter 6, Section 6.2.1**.

### 4.1.5 Access Requirements

**Access requirements** can be understood as a "**door opener**." They define a requirement that must be fulfilled to access a condition table.  
Access requirements are routines that contain **ABAP code** and can be created via **Transaction VOFM**, followed by the path **Requirements • Pricing**.

In the SAP standard, two access requirements are relevant for the **indirect tax condition tables**. They regulate the relation between the **tax departure country** and **tax destination country**, and include a check for the **VAT ID**. The reason behind this is that inside the **EU**, a valid **VAT ID** must be present to apply a **tax exemption** for cross-border supplies.

Let’s take a look at both relevant access requirements:

#### Access Requirement 7

**Access requirement 7** is assigned to condition table **A002 (Domestic Taxes)** in access sequence **MWST**. This access requirement contains the following checks:

- **Tax departure country** isn’t empty.
- **Tax destination country** isn’t empty.
- If both tax departure and tax destination country are part of the **EU**, and the **VAT ID** of the customer is empty, the requirement is fulfilled.
- If the **tax departure country** equals the **tax destination country**, the requirement is fulfilled.

The access requirement is fulfilled if the tax departure country and tax destination country are the same or if no **VAT ID** of the customer exists.

#### Access Requirement 8

**Access requirement 8** is assigned, for example, to condition table **A011 (Export Taxes)** in access sequence **MWST**. This access requirement contains the following checks:

- **Tax departure country** isn’t empty.
- **Tax destination country** isn’t empty.
- The tax departure country doesn’t equal the tax destination country.
- If, for a tax condition, both tax departure and tax destination country are part of the **EU**, and the **VAT ID** of the customer is empty, the requirement isn’t fulfilled.

The access requirement is fulfilled if the tax departure country and tax destination country aren’t the same. For **EU countries**, there is a check for the **VAT ID** — if no **VAT ID** of the customer exists, the access requirement isn’t fulfilled.

The check whether the countries are part of the EU is made via the indicator **XEGLD** in the country data table **T005**.

#### Exception Check

Both access requirements 7 and 8 were extended due to the **Northern Ireland regulations of Brexit**. They now also include an **exception check** for the region. Nevertheless, they still follow the same logic.

## 4 Indirect Tax Determination in SAP S/4HANA

## 4.2 Indirect Tax Determination in Purchasing

There are two options to automate **indirect tax determination** in purchasing apart from the **condition technique**. After reading this section, you’ll be able to establish which is the best option for you and your company or project.

In the following sections, we’ll take a deeper look into the different options for indirect tax determination in purchasing, the relevance of the **purchase order** and **invoice receipt**, and finally some typical scenarios where the **SAP standard** needs to be extended.

### 4.2.1 Electronic Data Interchange versus Purchase Info Records

**Electronic Data Interchange (EDI)** is an electronic format for **data exchange**. For tax determination in purchasing, EDI is a good option to determine indirect taxes on incoming invoices of either related suppliers (i.e., within the same group) or of suppliers with established and regular relationships.

To maintain EDI mapping, execute **Transaction OBCD** or follow menu path:  
**Materials Management • Logistics Invoice Verification • Electronic Data Interchange (EDI) • IDoc • Assign Tax Codes**.

With EDI, the outgoing invoice of the supplier is mapped to the purchasing side of the recipient. For indirect tax, this means a mapping of the **tax code** of the supplier is mapped to the tax code of the system.

In Figure 4.24, you can see an example of such mapping:

**Figure 4.24 EDI Mapping for Tax in Purchasing**

To add a new EDI mapping, click the **New Entries** button. There are different **partner types** that can be used, which you can select in the **Partn.Type** column. Most relevant for indirect tax on the purchasing side are partner types:

- **LI (vendor)**
- **LS (logical system)**

A logical system may, for example, be a **pre-system** such as a cash register or a sales system.

The **PartnerNo** column includes the name of the partner, which is either the vendor number or the description of the logical system.

The **Tax Type** is the outgoing tax code of the supplier. This is then mapped to a country in the **C/R** field and to a receiving tax code.

Such a mapping based on the country makes it possible to map the same tax code of the supplier to different input tax codes for different countries, which is necessary for tax reporting purposes.

Now, let’s discuss the other option for purchasing: **purchase info records**.

**Purchase info records** define—among other things—a tax code based on the combination of **supplier** and **material**.

Purchase info records are a good solution if the same products are purchased from the same vendor over and over, and if these products are always supplied from the same country.

With purchase info records, it’s not possible to differentiate when a vendor delivers goods from different locations.

#### Example

A vendor has two main warehouses: one in the **United States** and one in **Canada**. When this vendor delivers goods to a customer in the United States, they may ship from either of the warehouses.

From an indirect tax perspective, one of these transactions would be an **import**, while the other would be a **domestic purchase** of goods.

When using a purchase info record, it’s not possible to differentiate between the two, as only one tax code can be maintained for the combination of vendor and material group.

In such a case, either the **EDI mapping** or the **condition technique** with multiple ordering addresses are better options.

By using **Transaction ME12** and entering the vendor and material number, you’ll arrive at the screen shown in Figure 4.25, which shows the **Purch. Organization Data 1** menu where the **Tax Code** for a combination of vendor, material (or material group), and purchasing organization can be maintained.

Be aware that tax codes found with **condition techniques** always have priority over tax codes found with purchase info records.

To create new purchase info records, use **Transaction ME11**, and enter the vendor, material, and purchasing organization. Press (Enter) and click the **Purch. Organization Data 1** button.

Here, you can enter the **Tax Code** and other basic information such as the usual **Incoterms** for this transaction or the planned delivery time.

**Figure 4.25 Purchase Info Record: Purchasing Organization Data 1**

### 4.2.2 Purchase Order

The **purchase order** is a central point in the indirect tax determination on the input side.

Relevant transaction codes related to the purchase order are:

- Creation of a purchase order (**Transaction ME21N**)
- Creation of a purchase requisition (**Transaction ME51N**)
- Creation of a purchase contract (**Transaction ME31K**)

Before creating a purchase order, you can create a **purchase requisition** to get an offer from a vendor. This purchase requisition may also already contain a **tax code**. If this is the case, the tax code is copied from the purchase requisition into the purchase order.

Additionally, you can create a **purchase contract** over a certain time frame or value with a vendor. Purchase orders are then created with reference to the purchase contract.

#### Warning!

When copying an existing **purchase order** as a basis for a new purchase order, the tax code is also copied **without redetermination**.

A **tax code** is assigned on the **item level** in the purchase order.

To reach the view in Figure 4.26, use **Transaction ME21N**, and enter the relevant purchasing information.

Afterwards, you’ll see a dropdown menu on the bottom third of the screen with the items on your purchase order. Choose the item you want to review the tax determination for, and navigate to the **Invoice tab**.

Here, you see the **Tax Code** field with the determined or manually entered tax code.

**Figure 4.26 Tax Code on the Purchase Order**

### 4.2.3 Invoice Posting

When posting an **incoming invoice** in relation to a purchase order in **Transaction MIRO**, the **tax code** is copied from the purchase order as a **suggestion value**.

This means that contrary to the output tax determination, it’s still possible to change this manually.

After executing **Transaction MIRO**, you can navigate to the **Tax tab**, as shown in Figure 4.27.

In the bottom of Figure 4.27, the reference to the purchase order is made via the purchase order number. This then copies over the tax code and calculates the tax amount.

**Figure 4.27 Incoming Invoice in Transaction MIRO with Purchase Order Reference**

Using this suggested value from the purchase order on the incoming invoice posting greatly reduces the error rate on tax determination of incoming invoices.

Employees booking these incoming invoices into the system no longer have to decide the tax matter based on the incoming invoice—which may often be very difficult and prone to errors.

If the incoming invoice is different from the purchase order, changes can still be made.

#### Example

You receive an invoice from a provider of **construction services inside the EU**. This is a very special case from an indirect tax perspective, which leads to a **local reverse charge** in the country where the construction services were provided.

The received invoice will be **without indirect tax**, as you’re liable for indirect tax in this scenario.

With tax determination on the purchase order, or through purchase info records, you can predetermine the correct tax code on this matter.

Not using tax determination on the input side will very likely lead to errors in such a case.

### 4.2.4 Customization Scenarios

There are several scenarios in the SAP standard that may require users to extend the standard functionality, depending on the relevant business transactions.

In this section, we’ll briefly mention two scenarios where we often see extensions of the SAP standard during **input tax determination**.

Note that these scenarios aren’t comprehensive, and there are many other cases that may call for a system enhancement based on the business transactions or the local regulations.

#### Differentiation of Private and Business Vendors

In many jurisdictions, there is a difference between **private individuals** and **businesses**, as the businesses are **taxable entities** while private individuals aren’t.

To automate such transactions, it’s necessary to differentiate between **business vendors** and **private vendors**, and multiple options are available to do so:

- **Natural person**: Maintain the status in the master data of the business partner.

## Business Partner Identification in SAP

In the overview of the **business partner** accessed via Transaction **BP** (partner role **Business Partner (Gen.)**), under **General Data • Identification**, the **Natural Person** (technical field **LFA1-STKZN**) checkbox can be selected to identify that this is a **private individual** and not a business. This field must then be integrated into the **tax determination** as it’s not considered in the standard.

### Tax Number Validation

- Check the **tax number** of the vendor during pricing on the purchase order.
- Usually, a **private individual** won’t have a tax number maintained, while a business should have the tax number maintained.
- This can be managed by adding such a check in **EXIT_SAPLMEKO_001** and extending the tax determination, for example, by populating the **customer tax classification**, which usually isn’t used in input tax determination.
- For more information on this customer exit, see Chapter 6, Section 6.3.2.

**Warning!** Both approaches depend strongly on the quality of **vendor master data**. If the **tax ID** of the vendor is inconsistently maintained or private individuals are categorized incorrectly, the tax determination will deliver inconsistent results when using these indicators.

### Example

A **car dealership** buys new cars from other entrepreneurs such as **car dealerships** or **car producers**, as well as used cars from **private individuals**. Therefore, the car dealerships or car producers will sell the cars with **indirect tax**, while the private individuals won’t include indirect tax.

---

## 4.2 Indirect Tax Determination in Purchasing

### Purchase Orders with Nonmaintained Materials

When buying products and posting incoming invoices, there isn’t always a **material maintained** in the system. In such cases, the standard tax determination via the **condition technique** won’t be a viable approach.

- These transactions often occur for **self-use products** such as stationery or consulting services.
- For **resale products**, a material should always be maintained.

There are two options for such cases:

- **Use an SAP standard condition table**
  - Use table **A087** (Taxes: Plant, Account Assignment, Origin and Region) when the account assignment is detailed enough for the business case, e.g., different accounts and cost centers for services, self-use products, and other relevant cases.
  - Decision must be made on a **case-by-case basis**.
  - Note: It’s not possible to differentiate between products subject to different indirect tax rates using this table.

- **Create a new condition table**
  - Include the **material group** (technical field **KOMPMATKL**) as a key field.
  - Create a new condition table including the material group as a **super category** of certain materials.
  - This requires maintenance of the material groups in the **material master settings**.
  - The number of material groups applicable must be assessed **case-by-case**, but enough groups should reflect different relevant business scenarios.

### Example

A business buys **tax consulting services**, **food for the office**, and **stationery** without maintained materials, as these are for **self-use** and not for resale. There should be one material group for **services**, **reduced indirect tax rate products**, and **regular indirect tax rate products**.

---

## 4.2.5 Solutions for Procurement

SAP provides several products for **procurement** to consider during an SAP implementation, plus similar third-party products. These create **tax-relevant data** transmitted into the system in a standardized approach and support **tax-relevant processes** such as incoming invoice processing.

### Vendor Invoice Management (VIM)

- Also known as **OpenText**, it is the SAP solution for **invoice management** and processing.
- Integrable with the **OpenText Invoice Capture Center** for SAP solutions to enable **optical character recognition (OCR)** for automating paper invoice capture.
- Aims to optimize and simplify the **end-to-end accounts payable process**.

#### VIM Features

- **Review and approval processes**:
  - Supported by **exception handling**, reporting, and escalation functionalities.
- **Workflows**:
  - Create workflows to organize and track incoming invoice processing.
- **Routing and sorting**:
  - Define workflow steps to route invoices by user roles, authorization rules, or timelines.
  - Automatic classification and escalation for problematic invoices.
  - Touchless invoice management with automatic association to related purchase orders and historic documents or payment info.
- **Reports**:
  - Generate reports filtered by **region**, **business unit**, or **exception type**.
  - Provide analytics such as cause and effect diagrams.
- **Granularity**:
  - Graphic dashboard displays overall or individual system documents.
- **Location independent**:
  - Invoices can be entered into SAP from anywhere.
  - Stores all invoices electronically regardless of original format.
- **Status monitoring**:
  - Monitor payment status and invoice issues.
  - Supports vendor notifications and communication.

---

### SAP Ariba

SAP Ariba is an overall **procurement solution** with many subproducts for **suppliers** and **buyers**. It provides centralization, automation, and analytics capabilities for procurement activities.

#### Key Features

- **Guided buying**  
  Simple user interface directs employees to preferred suppliers, supports collaboration, and enforces company purchasing policies.

- **Demand management**  
  Supported by **SAP Ariba Supply Chain Collaboration for Buyers** and **SAP Ariba strategic sourcing solutions** for **strategic sourcing**.  
  - Develop strategic sourcing plans.
  - Manage sourcing events, procurement processes, and key project activities.
  - Identify qualified suppliers and compare agreements to generate savings and faster reaction times.
  - Centralize and standardize purchasing processes.
  - Offer analytics to identify spend and save opportunities.

- **Supplier management**  
  SAP Ariba provides three products:
  - **Supplier Information and Performance Management**
  - **Supplier Lifecycle and Performance**
  - **Supplier Risk**  
  These offer information and analytics on suppliers, spend, performance, and risk.  
  - Supports supplier due diligence and lifecycle management.

- **Supplier interaction (purchase agreements and invoicing)**
  - **SAP Ariba Contracts** manages procurement contracts and agreements lifecycle.
  - **SAP Ariba Invoice Management** automates invoices based on purchase orders and exceptions.
  - Tools include invoice dashboards and automated exception handling.
  - **SAP Ariba Discount Management** automates supplier discount processes like early payment or contract discounts.

- **Analysis**  
  **SAP Ariba Spend Analysis** provides in-depth spend analytics to help procurement and finance understand operations and areas for improvement.

#### Procurement Process Support

SAP Ariba tools support the entire purchasing process:

- Finding partners: **Sourcing, Discovery, Spend Analysis, Supplier Lifecycle and Performance, Supplier Risk**
- Contract management: **Contracts**
- Product management: **Catalog**
- Purchasing: **Buying**
- Invoice management: **Invoice Management, Discount Management**

---

### Coupa and JAGGAER

#### Coupa

- A **cloud-based spend management app** offering dashboards, spending reports, and analytics.
- Supports the entire procurement process: planning, budgeting, supplier management, transaction processing.
- Includes **inventory management**, **contract lifecycle management**, and analytics with audit trails.
- Provides an invoice system following general international standards.
- Focuses strongly on **responsible spend management** and visibility via analytics.
- Popular features: employee expense and invoice management.
- Known for stability and less implementation overhead compared to SAP Ariba.

#### JAGGAER

- A **software as a service (SaaS)** solution for direct and indirect **e-procurement**.
- Supports the full **accounts payable contract lifecycle** to optimize source-to-pay processes.
- Offers tracking and analytics for the full operational supply chain, including **spend analytics** for saving opportunities.
- Manages the contract lifecycle beginning with **supplier and sourcing management** through contract management.
- Supports **demand planning** with e-procurement tools for buying suggestions, tracking, and approval workflows.

---

© 2022 by Rheinwerk Publishing Inc., Boston (MA)

## Touchless Invoicing and Inventory Management

Creation, approval, and management of **invoicing** is possible in a **touchless invoicing process**, and **inventory management** is supported with **stock tracking** and **purchasing compliance**.

When deciding on a solution, we recommend determining your most important areas of need as the first step. If your **procurement process** is relatively simple, you may be happier with a smaller solution such as **JAGGAER**. In contrast, if your procurement process is more complicated, you’ll likely find that **SAP Ariba** or **Coupa** will fit your needs better.

## SAP Concur

**SAP Concur** is **SAP’s solution** for **expense management**. It’s a **cloud-based service** that supports management of **travel, expenses, and expense invoices**. Within these three areas, SAP Concur offers several features:

### Concur Travel

With **Concur Travel**, you’ll be able to support your traveling employees with:

- Comprehensive data for each traveler
- Location check-ins
- Online travel booking and comparisons
- Centralization of travel data and analysis of travel expenditures
- Enforcement of travel policies

### Concur Expense

**Concur Expense** enables you to track all your **expenses in one place**. You and your employees can capture **receipts**—paper or digital—from multiple card types. 

Concur Expense enables you to **reimburse employees faster** by allowing them to **review, submit, and approve expense reports** also via mobile. It also **enforces spending policies**.

### Concur Invoice

**Concur Invoice** was created to speed up the **invoicing process** and reduce costs related to it by reducing **paperwork**, **duplicate or late payments**, and by **e-mailing invoices**.

The benefits of SAP Concur are the **elimination of paperwork and errors**, as well as the **centralized overview of compliance and costs** related to travel and expenses. It makes the entire travel process easier, from travel booking to submission of prepopulated expense reports, approvals, and increased travel compliance.

## 4.3 Indirect Tax Determination in Sales

You’ve already learned about the **condition technique** in Section 4.1. **Indirect tax determination** in **sales and distribution** is always based on the **condition technique**.

In this section, we’ll dive deeper into how exactly the **condition logic** works and how to **validate the indirect tax determination**.

### 4.3.1 Sales Order

The first step for the sales process in SAP is the **sales order**. It is a formal order of a customer that defines the transaction.

Transaction codes related to the sales order start with **VA**:

- Creation of a sales order: **VA01**
- Change of a sales order: **VA02**
- Display of a sales order: **VA03**

For **indirect tax determination**, the following information is important:

#### Customer

Any validations of **tax numbers** are based on the **customer**. The customer covers two important data fields for indirect tax determination.

You’ve already learned about the relevance of the **VAT ID** and different **business partner roles** it can be determined from (Chapter 3, Section 3.2.2). These settings are applied in the sales order for indirect tax determination.

Note that there is **no visible determination of the tax identification number on the sales order**, but the **tax ID** and **customer tax classification** are considered during **pricing** on the sales order. However, the tax ID is visible only on the **invoice**.

The **customer master data** contains the information on the **customer tax classification** on the sales area level (Chapter 3, Section 3.2.1).

#### Ship-to address

The **tax destination country** is generally determined from the **ship-to address**. This is the address where the goods are delivered.

After executing Transaction **VA01** and entering the sales order type, you reach the screen where the **Sold-To Party** (customer) and **Ship-To Party** (recipient of goods) may be the same or different entities.

- If the **Sold-To Party** and **Ship-To Party** are the same, the delivery address is determined from the customer master data.
- If different, the delivery address is determined from the master data of the Ship-To Party.

#### Change of Addresses in the Sales Order

Addresses of all business partners involved can be changed by clicking the header icon and navigating to the **Partner tab**. This address is then used for the delivery address on the delivery note and for tax determination.

#### Supplying Plant

The **tax departure country** is one of the most important indicators for tax determination and indirect tax reporting. It is generally determined from the **supplying plant** on the item level.

The supplying plant is the **warehouse from which the goods are dispatched**. This field (**Plnt**) can be seen at the sales order line item level.

#### Material

The **material** is the product or service sold, determined on the item level.

The **material tax classification** (Chapter 3, Section 3.2.1) is important to determine the indirect tax rate of a product. It is determined from the **material master data** and visible on the sales order item level.

#### Indirect Tax Determination Process

The indirect tax is determined based on these fields.

- To view details of condition types determined for pricing on the indirect tax determination:

  1. Double-click on the item.
  2. Choose the **Conditions tab**.

- Pricing elements such as **net price**, **output tax rate**, and **total amount** are displayed.

- An **Analysis button** on the Conditions tab provides a detailed overview of all pricing elements inside the pricing procedure.

- Example: Condition type **TTX1** with matching condition record in step 010, table **A002** (Domestic Taxes).

- Details include:

  - Tax departure country (**DE**)
  - Customer tax classification (**TaxClass1-Cust 1**)
  - Material tax classification (**Tax class. material 1**)
  - Output tax amount

Selecting the condition type line and clicking the magnifying glass icon allows viewing details of the determined **output tax**, such as:

- **Tax Code** on the position
- Condition amount
- Validity period of the condition record

This is a good point to start **validation and error search** for incorrect indirect tax determinations.

### 4.3.2 Invoice

The **invoice** or **billing document** is the final step in the **tax-relevant sales and distribution process**.

Generally, an **accounting document** is created from an invoice, which allows for **tax reporting**.

Transaction codes related to the billing document start with **VF**:

- Creation of a billing document: **VF01**
- Change of a billing document: **VF02**
- Display of a billing document: **VF03**

#### Options to Create an Invoice

- **Sales order-based invoices**: Created directly from a sales order, suitable for invoices that contain materials without delivery (e.g., services).
  
- **Delivery-based invoices**: Created from a delivery, suitable for tangible products.

#### Copying Control Configuration

The process to create an invoice and view document type and pricing information copying is maintained in **copying control**.

- Access configuration via Transaction **SPRO** with this menu path:  
  Sales and Distribution • Billing • Billing Documents • Maintain Copying Control for Billing Documents.

- Alternatively, use:  
  - **VTFA**: Copying control from sales order to billing document  
  - **VTFL**: Copying control from delivery note to billing document

##### Examples of Copying Control

- Figure 4.35: Copying control between **standard order (order type OR1)** and **standard invoice (billing type F2)** for a service item (item category TAD).

- Figure 4.36: Copying control between **delivery note (type LF)** and **standard invoice (billing type F2)** for a standard item (item category TAN).

#### Important Indicators for Indirect Tax

- **Billing Quantity**  
  - For sales order to invoice: Rule A (Order quantity minus invoiced quantity)  
  - For delivery to invoice: Rule B (Delivery quantity minus invoiced quantity)  
  - Explanation: Multiple deliveries and invoices per sales order are possible, but not vice versa.

- **Pos./Neg. Quantity**  
  Determines quantity amount. Negative quantities are used to reduce quantities, e.g., when creating a sales order against a quotation.

- **Pricing Type**  
  Rule G (Copy price elements unchanged and redetermine taxes) ensures tax is determined at the most recent time.  
  Example: If a tax rate changes between sales order pricing and invoice creation, the correct tax rate applies to the billing document.

This **pricing type** can also be found in the **sales order** and **invoice** under the **Update field** in the **Conditions tab** on both **item** and **header level**.

## Using this Field

Using this field enables you to **redetermine parts of pricing** if a pricing-relevant value changed and pricing wasn’t redetermined automatically.  

**Example:**  
You’re using an enhancement to determine **indirect taxes** with respect to the **Incoterm**. In the SAP standard, pricing isn’t redetermined when changing the Incoterm. Updating pricing with **pricing type G** is necessary.  

As mentioned in **Section 4.3.1**, the billing document contains the determined **tax identification number**.  

Figure 4.37 shows the determined **VAT ID** based on the tax destination country.  

---

## 4.3 Indirect Tax Determination in Sales

The rule for the determination of the **VAT ID** in this case is <blank>, so the VAT ID is determined from partner role **A (Ship-to party)**.  

You learned about VAT ID determination rules in **Chapter 3, Section 3.2.2**.  

### Invoice Numbering

In some countries, such as **Argentina, Brazil, Italy, Portugal, and India**, you’re legally required to follow specific rules regarding **invoice numbering**.  

- Invoice numbers may need to be **consecutive per the date** and without gaps.  
- SAP offers the standard **official document numbering (ODN)** functionality to handle such requirements.  

### 4.3.3 Customization Scenarios

As for the purchasing side, several scenarios on the sales side aren’t covered by the SAP standard.  

In this section, we want to make you aware of some of the most common cases for which we’ve designed **system enhancements**. These are not comprehensive, and other cases may require enhancements based on business transactions or local regulations.  

---

### Place of Supply of Services

In many countries, **services supplied from one business to another business** are generally taxable at the **place of supply**, i.e., the establishment of the customer receiving the service.  

- In SAP standard, the **tax destination country** is determined from the **ship-to address (technical field KUWEV-LAND1)**.  
- This is incorrect for the supply of services in those countries. An **enhancement** is required to adapt the tax destination country.  
- For **business-to-business (B2B) services**, the tax destination country should equal the **country of the customer** (refer to **SAP Note 1406106**).  

One option is to overwrite the tax destination country with the country of the payer if a **service material** is identified.  

**Warning!** There are cases where goods and services are mixed in the sales order and billing document. SAP standard splits the billing document in such cases (refer to **SAP Note 1412947**).  

Place of supply rules for services and application of enhancements for mixed orders should be reviewed on a **country-by-country basis**.  

---

### Transport Responsibility in Tax Determination

In **chain transactions inside the EU** or cross-border transactions where the customer is responsible for export, **transport responsibility** is a factor in determining correct indirect tax.  

- Multiple SAP fields describe transport responsibility (e.g., **Incoterm**, **shipping condition**).  
- These can be included in indirect tax determination via code inside a **user exit** or a new **condition table** added to the access sequence.  

Adding a **transport responsibility indicator** allows differentiation between cases where you or the customer is responsible for export, enabling different tax codes.  

**Example:**  
- Business transactions from Australia where goods are exported or picked up by Australian customers.  
- Transport responsibility identified by Incoterm: deliveries use **DAP**, pickups use **EXW**.  
- Deliveries apply **0% GST**; pickups apply **0% GST** only if customer isn’t registered for GST in Australia.  
- Incoterm is added to indirect tax determination.  

---

### Invoice Texts

Many jurisdictions require taxpayers to describe why **indirect tax** was not applied on a transaction. Rules vary widely between countries.  

- It is recommended to create a **mapping for an invoice text per tax code**.  
- The correct invoice text can be printed automatically on invoices.  
- This is usually done via a **Z-table** accessed during print processing (see Chapter 6).  

**Example:**  
For applying **0% VAT** on an **intra-community supply of goods inside the EU**, the invoice should describe the reason as **“VAT exempt intra-community supply of goods”**.  

---

## 4.4 Special Indirect Tax Transactions

### Location-Based Supplies and Services

In some jurisdictions, supplies and services are taxable at the **location where they are provided**. Examples include **construction services, fairs, or events**.  

- The place of supply and tax destination country must be adapted during pricing.  
- It is generally recommended to create a new **material tax classification** for these cases.  
- Based on the classification, the tax destination country can be changed in the applicable **user exit**.  

**Example:**  
- A construction provider specializing in resorts won a project to build a large hotel in **France**.  
- They are not registered for VAT in France but need to register because construction services are taxable where provided—in France.  
- The tax departure country for construction services is **France**.  
- Another department advises a French customer on resort management (a standard B2B service). This service is VAT-exempt in Sweden (provider’s residence) and taxable as a reverse charge in France by the French customer.  
- Tax departure country here is **Sweden**.  
- Differentiation in tax determination is needed, possibly via different material tax classifications.  

---

### 4.4 Special Indirect Tax Transactions Overview

This section covers special SAP processes for specific indirect tax requirements, including:

- **Chain transactions**  
- **Internal stock transfers**  
- Special payment processes such as **down payments, billing plans, self-billing**, and **discounts**  

---

### 4.4.1 Chain Transactions / Drop Shipments

Chain transactions, also called **drop shipments**, involve three or more entrepreneurs in a supply chain where goods are delivered directly from the first to the last entrepreneur.  

Figure 4.38 shows a schematic overview of such a transaction.  

---

### Positions in Chain Transactions

Your company may act as:

- **First entrepreneur**  
- **Second entrepreneur**  
- **Third entrepreneur**  

Each position has different tax implications.  

---

### Drop Shipments Globally

Jurisdictions apply different rules on drop shipments, such as:  

- Taxing only one of the supplies  
- Taxing only the recipient  
- Applying place of supply rules  

It is crucial to understand jurisdiction-specific requirements.  

---

### First Entrepreneur

The **first entrepreneur** supplies goods to the **second entrepreneur** (customer).

- Awareness that the transaction is a **chain transaction** is important, especially if the goods are exported by another party.  
- Many jurisdictions require that the export be made by the first entrepreneur or that the receiving party is not resident in the departure country to apply **zero percent indirect tax**.  

**Example:**  
- An **Australian company** (first entrepreneur) selling goods to a customer outside Australia (second entrepreneur) can apply **0% indirect tax** if:  
  - The Australian company is responsible for export, or  
  - The party responsible for export isn’t a resident of Australia.  
- If a third entrepreneur resident in Australia is responsible for export, domestic indirect tax applies.  

---

### EU Chain Transaction Rules

The EU applies special rules to **chain transactions**:  

- In a **cross-border chain transaction**, 0% VAT can only be applied to one supply in the chain.  
- The exemption applies to the party responsible for transport.  
- If the first entrepreneur is responsible for transport, they apply the exemption.  
- If the second entrepreneur is responsible, they choose which transaction to apply the exemption to.  

**Example:**  
- A **German company** (second entrepreneur) receives an order from a **French customer** (third entrepreneur) but does not stock goods and orders from another German company (first entrepreneur).  
- If the first entrepreneur is responsible for transport:  
  - First entrepreneur has a **VAT-exempt intra-community supply**.  
  - Second entrepreneur must register for VAT in France as an intra-community acquisition.  
- If the second entrepreneur is responsible for transport and uses its German VAT ID when purchasing:  
  - Domestic purchase in Germany, followed by an exempt intra-community supply.  
- If the second entrepreneur is a French company, it may be more practical to attribute the exemption to the transaction between first and second entrepreneur, with an intra-community acquisition in France, then domestic supply.  
- If the third entrepreneur is responsible for transport, the first entrepreneur applies **domestic VAT** because the exemption is attributed to the third entrepreneur.

---

### SAP S/4HANA Chain Transaction Handling

As the **first entrepreneur** in a chain transaction:  

- Your **sold-to party** and **ship-to party** are different entities.  
- The sold-to party is the **second entrepreneur** (middleman).  
- The ship-to party is the **third entrepreneur** (final recipient).  

Figure 4.39 shows an example of a **sales order** with different sold-to and ship-to parties.  

Refer to **Section 4.3.1** for more information on the sales order.

## 4 Indirect Tax Determination in SAP S/4HANA

### Second Entrepreneur

As the **middle party** in a **chain transaction**, you’re usually aware that you’re inside a chain transaction. For **multiple-country chain transactions**, this is usually a beneficial situation: oftentimes, no reporting requirements or tax are applicable to your transaction, as the goods never reach your country of residence. The **first entrepreneur** can be responsible for the **export** from the country of departure, and the **third entrepreneur** can be responsible for the **import** (and corresponding duties and import taxes) in the country of destination.

We want to bring your attention to two special cases:

- **Drop shipments inside the United States**  
  In a **drop shipment transaction** within the United States, the second entrepreneur (**retailer**) is generally responsible for the collection of **sales and use tax**. However, if the second entrepreneur isn’t a resident (i.e., doesn’t have a **nexus**) in the third entrepreneur’s (**buyer’s**) state, it’s possible that the **first entrepreneur** (**supplier**) may be held liable for the sales and use tax collection if they have a nexus in that state.  
  Alternatively, a nexus may be asserted to the second entrepreneur who isn’t from the state of destination under the **flash title theory**: as the retailer is owner of the product sold for a brief moment before it’s sold on to the final customer, they have a presence in the state.

- **Triangulation simplification (ABC transactions) inside the EU**  
  This special case involves three entrepreneurs in an **EU triangular deal**. Conditions include:  
  - Three legal entities enter a contract considering one supply.  
  - The supply is directly from the **first supplier** (first entrepreneur) to the **last customer** (third entrepreneur).  
  - The legal entities are registered for **VAT** in three different EU member states.  
  - **Goods are transported** from one EU member state to another.  
  - Goods are transported by either the first supplier (first entrepreneur) or the first customer (second entrepreneur).  
  Under this simplification, the second entrepreneur is not obligated to register for VAT in the state of departure or destination.

The process of booking chain transactions in **SAP S/4HANA** isn’t straightforward due to the separation of the **sales and distribution module** and the **materials management module**, requiring an interface between the two.

The **third-party process** (delivery of goods not by the sales organization but by an external vendor) steps are:

1. Customer orders goods and creates a sales order.  
2. A purchase requisition is automatically created when the sales order is saved.  
3. A purchase order to the vendor is created based on the purchase requisition.  
4. The vendor delivers the goods to the customer.  
5. An invoice is created (received from the vendor).  
6. Finally, an invoice to the customer is created (the order-related invoice).

---

### 4.4 Special Indirect Tax Transactions

#### Steps to create a third-party order:

1. **Create a sales order**  
   The third-party process is triggered by creating a sales order with a **third-party item**. This item has a special **item category** that can be assigned manually or determined automatically via the **material master data** and item category determination.

   - To access material master data settings, see Chapter 3, Section 3.2.1.
   - In the **Sales: sales org. 2** tab (Figure 4.40), the **Item Category Group** is used for item category determination.

   Item category determination for third-party items should be predefined in SAP:

   - Use Transaction **SPRO** and follow menu path:  
     *Sales and Distribution > Sales > Sales Documents > Sales Document Item > Assign Item Categories*  
   - You arrive at the screen showing sales order type (SaTy), item category group (ItCGr), and possible item categories (Figure 4.41).

   When entering an item with item category group **BANS** in sales order using Transaction **VA01**, item category **TAS** will automatically be determined (Figure 4.42).

2. **A purchase requisition is automatically created**  
   After saving the sales order, a purchase requisition is created automatically.  
   - To see the purchase requisition number, double-click the sales order item (item category TAS) and go to the **Schedule lines** tab (Figure 4.43).  
   - The purchase requisition number appears in the **Purchase** column.

   **Schedule Line Categories**:  
   - The purchase requisition is created through **schedule line category CS**.  
   - Schedule line defines info to copy from sales order and actions to take.  
   - Category CS contains info about the purchase requisition to be created.  
   - For maintenance, go to:  
     *Sales and Distribution > Sales > Sales Documents > Schedule Lines > Define Schedule Line Categories*.

3. **Create a purchase order**  
   Using the purchase requisition, create a purchase order. This connects sales and materials management.  
   - The purchase order can be created automatically by using standard item category group **ALES** instead of BANS.  
   - Go to Transaction **ME21N**.  
   - On the left, select **Purchase Requisitions** (Figure 4.44).  
   - Enter your Purchase Requisition Number, and click **Execute** (Figure 4.45).  
   - The purchase requisition appears in the document overview; click **Adopt** icon to copy data into the purchase order (Figure 4.46).  
   - Enter the vendor if not pre-maintained, and save your purchase order.

4. **The vendor delivers the goods to the customer**  
   This happens **outside** your SAP system. Optionally, link this to a goods receipt in SAP via inbound delivery with Transaction **VL31N**.

5. **Invoice receipt**  
   After receiving an invoice from your vendor, enter it in the system with reference to the purchase order.  
   - Use Transaction **MIRO** for manual entry.  
   - In **Basic Data** tab, enter the purchase order under **PO Reference** to copy relevant data including the tax code (Figure 4.47).  
   - Automatic incoming invoice processing with **OCR** may also be implemented.

6. **Create and issue a customer invoice**  
   Only after posting the incoming invoice can you issue a customer invoice based on the sales order.  
   - Use Transaction **VF01**.  
   - Enter the sales order number in the **Document** field (Figure 4.48).  
   - Save the invoice.

---

### Third Entrepreneur

As the **third entrepreneur**, a chain transaction is treated as a **normal input transaction**. This includes owning documents related to:

- Purchase order  
- Goods receipt  
- Incoming invoice

---

### Best Practices for Indirect Tax

There are significant differences in **indirect tax determination** with chain transactions depending on jurisdiction. Always gather requirements before implementation.

Common practical issues relate to **transport responsibility** and **residence of the parties involved**.

#### Transport responsibility

- SAP does not provide a set indicator for determining which party has transport responsibility.
- Common practice uses **Incoterms** as an indicator.  
- **Incoterms (international commercial terms)** are standards describing terms and transport obligations related to goods sales.  
- Incoterms can be included in a custom condition table for tax determination.

#### Residence of involved parties

- Residence of the **exporting party** (if it’s not the first entrepreneur responsible for export) is often relevant.
- SAP standard indirect tax only considers countries of **departure** and **destination**.  
- To include the residence of parties, extend communication structures for indirect tax determination (**KOMKAZ**, **KOMPAZ**) with the new field and populate it during pricing.
- This allows use of residence in tax determination, e.g., via a custom condition table or **Z-table** that overwrites the tax destination country with the tax departure country for applying domestic indirect tax.  
- See Section 4.1.4 and Chapter 6, Section 6.2.1 for more information.

#### Distribution channel

- Some clients use separate distribution channels to differentiate **drop shipments** from **direct sales**.
- Although distribution channels come from Sales and Distribution and not specifically for tax, they can be leveraged for indirect tax determination.
- For example, a specific distribution channel can indicate a chain transaction combined with transport responsibility.

#### Proof of delivery

- In many countries, a **proof of delivery document** is required to confirm that a supply was delivered.
- In some jurisdictions such as the EU, this document is relevant for the **tax exemption** of a transaction.

## Cross-Company Sales

You may also get an **automatic verification and confirmation** via the **SAP standard proof of delivery process**. This requires integration with the **ship-to-party via IDocs** (other SAP system) or an **internet interface**.

A **cross-company transaction** in **SAP S/4HANA** involves more than one **company code**. This usually occurs when more than one **legal entity** exists in the SAP organizational structure. Several cross-company transactions are possible, but we’ll focus on **cross-company sales**.

As an example, consider an organization where:
- A **sales entity in the United States** (selling entity) purchases goods from a **production entity in China** (fulfilling entity).
- Goods are delivered to a **customer in Europe** (end customer), as illustrated in Figure 4.49.

The process:
- The order from the end customer in Europe is received by the system; the customer orders goods from the U.S. entity.
- The U.S. entity doesn’t have the product in stock and forwards the order to the production entity in China.
- This is achieved by entering the **plant of a different company code** in the order—the plant in China is the fulfilling plant.
- After delivery, the U.S. entity invoices the end customer.
- The Chinese company invoices the U.S. company via **intercompany billing**.
- In a cross-company sale, there are always **two invoices**.

> **Warning!** The standard pricing procedure for **intercompany billing** is different. Be sure to include the **condition types for tax** in the intercompany pricing procedure.

### Tax Determination in Cross-Company Sales

As learned in Section 4.1, **tax determination** usually uses:
- The country of the **plant** as the **tax departure country**.
- The country of the **ship-to address** as the **tax destination country**.

This is **not the case in intercompany billing**. Instead:
- The **tax departure country** is the address of the **supplying company code**.
- The **tax destination country** is the country of the **maintained internal customer’s address**.

This can lead to incorrect tax determination. For example:
- The standard setting might determine **import tax in the United States**, even though goods never enter the U.S.
- Within the EU, the standard setting causes more complex issues.

SAP offers guidance in **SAP Note 10560**, suggesting:
- Use the **country of the plant** instead of the country of the company code as the tax destination country.
- Use the **country of the ship-to party from the order** instead of the country of the internal customer.

Note: This SAP Note is a consulting note requiring **technical implementation**.

---

## 4.4.2 Internal Stock Transfers

Companies with multiple plants often need to shift stock between them. A **stock transfer** refers to the **logistical movement** of the company's own goods within the company. From a tax perspective, these straightforward transactions can present complex tax pitfalls involving:
- **Transfer pricing**
- **Corporate income tax**
- **VAT/sales tax**

The fundamental question is the **taxability of transactions** within a **consolidated group** (i.e., the legal entity).

### Prerequisites for Internal Stock Transfers

In **SAP S/4HANA**, the **stock transport order (STO)** process manages these transfers. Two types must be distinguished:

- **Intracompany stock transfers:** Transfer stock between plants within the **same company code**.
- **Intercompany stock transfers:** Transfer stock between plants in **different company codes**.

Both require defining the **supplying** and **receiving plant** as **vendor** and **customer**, respectively, in the **business partner master data**. A **business partner** must be defined first.

In Figure 4.50, a business partner is shown in Transaction **BP**, usually named referencing the company code or plant. For this business partner:
- A **customer** and a **supplier** must be created.

---

The newly created **internal customer** must be assigned to the receiving plant in purchasing configuration via:

`Material Management • Purchasing • Purchase Order • Set Up Stock Transport Order • Define Shipping Data for Plants`

In the overview, select the plant to assign customer data. In Figure 4.51, the **internal customer number** of the plant is entered into the **Customer No. Plant** field.

Similarly, the **internal vendor** must be linked to the dispatching plant. This is done in the **Vendor: General Data** tab under the **Supplier role** in business partner settings, as shown in Figure 4.52.

---

### Purchase Order Document Types

SAP standard delivers two purchase order document types for stock transfers:
- **UB** for **intracompany STOs**
- **NB** for standard purchase orders for **intercompany STOs**

Purchase order document types are maintained in:

`Materials Management • Purchasing • Purchase Order • Define Document Types`

Here, you can view, edit, or create document types (Figure 4.53). A custom STO document type can be set up by clicking **New Entries**.

- Item intervals can be in steps of 10.
- Number range assignments can remain within the standard number range.

---

### Item Categories

Allowed **item categories** control the **billing relevancy** of purchase orders.

- For **intracompany stock transfers** in the same country, item category **U** is assigned.
- This follows the **single entity principle**, meaning goods remain within the same legal entity and are **nonbilling-relevant** logistics processes.

This differentiation is crucial for **indirect tax**:
- Stock transfers within the same legal entity are generally **non-taxable**, only potentially subject to import procedures.
- Stock transfers between different legal entities (different company codes) must be treated like **sales to external customers**.

---

### Delivery Types and Availability Check

In configuration path:

`Material Management • Purchasing • Purchase Order • Set Up Stock Transport Order • Configure Delivery Type & Availability Check Procedure by Plant`

You assign **delivery types** and **checking rules** to combinations of purchasing document types and supplying plants.

Figure 4.55 shows:
- Purchase order type **NB** for standard orders
- **NB2** for returns
- **UB** for STOs with delivery type **NL** (Replenishment Delivery) for internal stock transfers

Delivery types include:
- **NL** for stock transfers within the same company code
- **NLCC** (Replen.Cross-Company) for billing-relevant stock transfers between company codes

Delivery type **NL** typically pairs with document type **UB** and checking rule **B (SD Delivery)**.

Delivery type **NLCC** pairs with document type **NB**.

Default document types for supply/receive plant pairs are maintained under:

`Material Management • Purchasing • Purchase Order • Set Up Stock Transport Order • Assign Document Type, OneStep Procedure, Underdelivery Tolerance`

Figure 4.56 shows document type **UB** assigned for supply plant **1010** and receiving plant **2010** within the same company code.

> Be careful **not to assign UB** for **cross-company stock transfers**.

---

### One-Step Option

The **One-Step** option generates both the **goods issue** and **goods receipt** documents simultaneously.

- If the One-Step checkbox is deselected, goods issue and receipt must be posted manually.
- Manual posting allows tracking goods **in transit**.

---

### Stock Transfers in SAP S/4HANA Cloud

SAP S/4HANA Cloud has **no standard procedure** for intercompany stock transfers. To enable this:
- **Scope item 1P9** must be activated.

To create an outbound delivery from your STO, configure **copying control** at:

`Logistics Execution • Shipping • Copying Control • Specify Copying Control for Deliveries`

Figure 4.57 shows copying control automatically set for **DL (Order Type Sched.Ag.) (STO)** to delivery types **NL** and **NLCC**.

---

### Creating a Stock Transport Order (STO)

To post an internal stock transfer:
1. Start by creating a **STO** via Transaction **ME21N**.
2. Choose **Stock Transp. Order** as document type.
3. The view changes; for example, **Vendor** becomes **Supplying Plant**.

Figure 4.58 shows the minimum required fields:
- **Supplying Plant** (e.g., 1010 Plant 1 DE)
- Purchasing and material data (like a normal purchase order)
- **Receiving Plant** on the bottom-right (e.g., Plant 1 CZ)
  
Depending on system settings, entering supplying and receiving **storage locations** may be useful.

---

### Shipping Tab

To create an outbound delivery in the next step, ensure your configuration is correct, and the **Shipping tab** appears on your STO.

- If the Shipping tab is missing, check the **assignment of customer and vendor to plants** and previously mentioned settings.

---

### Creating an Outbound Delivery

After saving the STO:
- Use Transaction **VL10B** to create an outbound delivery from this STO in materials management.
- The transaction offers several filtering options (Figure 4.59).
- If your STO does not appear, verify the **delivery date** and the **CalcRuleDefltDlvCrDt** field.
  - The default setting is 2 (**Due for Shipment Today and Tomorrow**).

## 4.4 Special Indirect Tax Transactions

If your **delivery date** isn’t within these dates, you can set it to blank.

### Creating Delivery from STO

- Click **Execute** (not shown) to reach the list in Figure 4.60.
- Select your **STO** and click **Background** to execute.

After processing finishes, you will see a second line with a green light, indicating success.

You can find the number of your created **outbound delivery** by going back to your **STO** and checking the **Purchase Order History** tab. The outbound delivery number appears above the yellow banner; here, it’s **8000017**.

From here, follow your standard **outbound delivery process** with a **goods issue** and your standard **inbound delivery process** with a **goods receipt**. You can see your stock changes for the supplying and receiving plants via Transaction **MMBE**.

---

### Billing-Relevant Scenarios

After the **goods receipt**, some scenarios may be **billing relevant**, especially if you have stock transfers:

- Inside the **EU** with plants abroad activated
- Between different **company codes**

A **delivery-related billing document** can be created for these cases. This is a complex process often underestimated in importance for indirect tax.

- The billing document for **internal stock transfers** has billing type **WIA**.
- The billing document for **intercompany stock transfers** is an intercompany billing with billing type **IV**.

---

### Intra-Community Stock Transfers within the EU

Since 1993, **intra-community stock transfers** inside the EU are equated with an **intra-community supply**. This results in a definitive charge of **VAT** in the country of destination.

In **SAP S/4HANA**, there is a specific pricing procedure **RVWIA1**, relevant once you activate plants abroad.

- The pricing procedure contains three condition types relevant for tax: **WIA1**, **WIA2**, and **WIA3**.

#### Condition Types Purposes:

- **WIA2**: Output tax in the country of departure (VAT-exempt movement outside departure country)
- **WIA3**: Intra-community acquisition of goods (input side)
- **WIA1**: Deduction of the input VAT on the intra-community acquisition

These input condition types usually have the same percentage and tax code, negating each other.

---

## 4.4.3 Special Payment Processes

Several **special payment processes** can affect indirect tax in SAP S/4HANA:

- **Down payments**
- **Billing plan**
- **Self-billing**
- **Rebates and discounts**
- **Kit supplies**

---

### Down Payments

The recipient of goods **does not always pay in full** after receipt. For large investments, **down payments** or **prepayments** are often agreed.

- From an **indirect tax perspective**, the **tax point** is usually when the prepayment is received.
- Regulatory tax point requirements must be validated **country-by-country**.

---

#### Down Payment Process in SAP

1. **Sales order** is created based on customer's order.
2. A **down payment request** is issued based on the sales order and agreed amount.
3. After payment by customer, a **down payment receipt** is created.

---

#### Configuration

- Check if your system has a **general ledger account** for down payments using Transaction **OBXR** or through menu path:  
  *Financial Accounting > Accounts Receivables and Accounts Payable > Business Transactions > Down Payment Received > Define Reconciliation Accounts for Customer Down Payments*.
- If unavailable, create an alternative reconciliation account via **Transaction FS00**.

---

##### Important Settings for Down Payment Account

- Set **Recon. Account for Acct Type** to **Customers**.
- The account must have Tax Category of **+B (Output tax – down payments managed gross)**.
- This allows **output tax only** and signals that the document is a down payment to be processed specially.
- Enable **Posting without tax allowed** checkbox to avoid errors in countries where down payments may not be taxed.

---

#### Linking Accounts

- Link the **standard reconciliation account** and the new account in Transaction **OBXR**.
- For example, standard account **12100000** is used for normal invoice payments.
- Special general ledger account **21190000** used for down payments (special G/L indicator A).

---

#### Creating Down Payment Request

- Use **Transaction F-37**.
- Set:  
  - Document Type to **DZ (Customer Payment)**  
  - Special general ledger indicator (**Trg.Sp.G/L Ind.**) to **F (Down Payment Request)**
- Enter **Amount**, **Tax Code** and select **Calculate Tax** checkbox to auto-calculate tax.
- Enter the **due date**.
- You can link the request to an existing sales document via the **Assignment** field.

---

> **Warning!**  
> This process is manual and prone to errors. Checking the **Calculate Tax** checkbox is essential for correct tax calculation. Also, tax code determination can be linked to sales order as **condition-based down payments** (see SAP Note 1788841).

---

#### Posting Down Payment Receipt

- Use **Transaction F-29**.
- Same document Type used.
- Special G/L Indicator **A (Down Payment)**.
- Enter company code, currency, document date, customer account, down payment amount, and GL account for bank posting.
- Link to existing down payment request using the **Requests** button at the top instead of pressing Enter.

---

#### Viewing Down Payment Receipt Details

- Double-click on items for more info or post directly.
- Posted financial accounting document shows lines for down payment request, related settlement, and output tax on receipt.

---

> **Warning!**  
> In some countries, a **tax-relevant invoice** must be issued at the time of down payment receipt. Since the down payment process is in financial accounting, challenges arise:
> - Creation of sales and distribution documents from financial accounting.
> - Managing invoice numbering ranges to avoid duplicate invoice numbers.
>
> Usually, a **delivery block** is placed until the down payment is received, then removed to continue outbound processing.

---

### Billing Plan

Another special payment process is **milestone billing** or **periodic billing**, using a functionality called **billing plan** in sales and distribution.

- The **billing plan** is a schedule of **delivery-independent billing dates** for goods and services.
- **Milestone billing** schedules the total amount to be billed in predefined intervals (common in large, long-term projects like construction).
- **Periodic billing** invoices a certain amount at regular time intervals (e.g., rent, licenses, leasing).

## 4.4 Special Indirect Tax Transactions

Important settings for the **billing plan** include the following:

### Billing Plan Type

The **billing plan type** includes the control data for the billing plan, such as the determination of **start** and **end dates** for the billing schedule.

### Date Category

The **date category** defines additional data for the billing date of the billing plan, for example, the **billing rule**, **date description**, **billing block**, whether a billing date is fixed, and the **billing type**.

The **SAP standard** contains the two billing plan types mentioned previously: **milestone** and **periodic billing**.

To reach the setting for the billing plan, use **Transaction SPRO** followed by path:

- Sales and Distribution • Billing • Billing Plan • Define Billing Plan Types.

In the popup, choose the billing plan type you want to edit. In the example, we’re looking at the billing plan type for **milestone billing**. Double-click the billing plan type you want to maintain or create a new billing type by clicking the **New Entries** button.

On the screen shown in **Figure 4.74**, you can check the settings of billing plan types, date descriptions, date categories, and date proposals, as well as define rules for date determination. You can also create a new billing plan from this screen by clicking **New Entries**.

### Milestone Billing Plan Types

For **milestone billing plan types**, you can define the **Start date of the creation**, as shown in Figure 4.74. In this case, it’s defined as **01 (Today’s Date)**. You can also define:

- A reference billing plan number (**RefBillPlanNo.**), if you have one
- A billing plan usage category (**BillingPlanUCat**), such as **2 (Time and Expenses)** or **1 (Fixed Price)**

### Periodic Billing Plan Types

For **periodic billing plan types**, you have a few more options in comparison to milestone billing plans (see Figure 4.75). The periodic billing plan type defined here has:

- A start date and date-from equal to the **contract start date**
- An end date equal to the **contract end date** and **date-until**
- A horizon of **one year**

Additionally, the **next billing date** is the **first of the month, each month**.

### Assignment of Billing Plan Types

Note that you can create an assignment of billing plan types to:

- **Document types**
- **Item categories**

if needed—for example, if there are only certain products that are sold via a billing plan.

This is possible in:

- Sales and Distribution • Billing • Billing Plan • Assign Billing Plan Types to Sales Document Types
- Sales and Distribution • Billing • Billing Plan • Assign Billing Plan Types to Item Categories

The SAP standard **item category** for milestone billing is **TAO**. Also note the billing relevance **I (Order-Relevant Billing – Billing Plan)** of the line shown in Figure 4.76.

### Example: Billing Plan for Milestone Billing

A billing plan for milestone billing could look like **Figure 4.77**. To reach this view:

- Create a sales order with a material and sales order type that are relevant for a billing plan.
- On the header level, choose the **Billing Plan** tab.

There are three dates for this exemplary milestone billing:

- The **down payment date**
- The **delivery date**
- A date for a **closing invoice**

This could be, for example, a billing plan for the commissioning of a **custom product**, which is produced after the down payment receipt and installed on-site after the delivery. This means that for one sales order over a certain value, several billing documents will be created.

In the example, you can see that the value of the item is **20,000.00 EUR** (**Net value field**). Below that, you’ll find details about the billing plan:

- The type (**BillingPlanType**)
- **Start date**
- The percentage (**InvoicePercentg**)
- The value to be billed over the course of the billing plan (**Billing Value**)

In the Dates table, you can see the individual billing dates of the billing plan. The field **DtDs (date descriptions)** contains the information about what the milestone billing date means, shown two columns to the right.

A billing value is defined based on percentage.

We’ve also defined the **Billing Type** for each milestone. Billing type **FAZ** is the SAP standard billing type for down payments, while billing type **F2** is the standard billing type for invoices. Using different billing types makes it possible to have different settings for:

- Printing invoices
- Forwarding the invoice into financial accounting
- Creation of the billing document itself

### Pricing in Down Payments or Partial Payments

For the pricing in down payments or partial payments of a billing plan, SAP offers the standard **condition type**:

- **YZWR** or **AZWR** (down payment/settlement), depending on the version of SAP you’re using.
- **AZWR** is the condition for **SAP S/4HANA**, while **YZWR** is the condition for **SAP S/4HANA Cloud**.

When creating invoices based on a sales order with a billing plan, the system automatically determines an additional position for the received down payment.

In **Figure 4.78**, you can see that the net value of **EUR 6,000** was automatically determined.

This is due to the previously mentioned condition type (**YZWR**) that was populated as defined in the billing plan in Figure 4.77. You can see the condition type in the pricing procedure in **Figure 4.79**.

---

## Cross-Border Supply of Goods

For **cross-border supplies of goods**, oftentimes the actual movement of the good is a prerequisite for a taxable transaction such as an **intra-community supply of goods** or an **export**.

At the time of receipt of a down payment, the movement of the goods is often not complete. Therefore, determining an **export tax code** during tax determination would be incorrect.

We recommend creating a **condition table** that includes the **billing type** in addition to the fields that are generally used for tax determination. That way, a new tax code for down payments on cross-border transactions can be found.

Refer to **Section 4.1.4** for details on how to create a condition table and add it to the access sequence.

---

## Periodic Billing

So far, we’ve taken a deeper look into an example for **milestone billing**. Let’s now have a look at **periodic billing** in the next step.

To create a billing plan for periodic billing, usually there will be a **contract** in place.

A **value contract** can be created via **Transaction VA41**. On this screen:

- Enter the contract type you want to create a contract for and press **Enter**.

The example in **Figure 4.80** is a **Value Contract** to represent a **rental contract for a period of one year**.

After entering the **customer number** into the **Sold-To Party** field and entering the start and end dates, the system will create billing dates as defined by the dates and the frequency (here, **monthly on first of month**).

From this value contract, you’ll be able to create a sales order via **Transaction VA01**.

Click on:

- Sales Document • Create with Reference, and enter the contract number, as shown in **Figure 4.81**.

Afterwards, you can follow your standard sales process. Every order and invoice you create will have reference to your rental contract and the associated value.

---

## Self-Billing

Usually, the entity fulfilling a supply of goods or services is the one issuing the invoice.

However, this isn’t always the case. In some industries, it’s common to pay for goods or services via **self-billing**: the **recipient** of the goods or service issues the **invoice to themselves** in agreement with the supplier.

Usually, these **self-billing invoices** must fulfill the same requirements as a **standard invoice**.

In some jurisdictions, the issuance of self-billing invoices isn’t permitted.

Self-billing in SAP usually requires an integration to the supplier via **EDI** (see **Section 4.2.1** for more information on EDI).

The self-billing process then begins with an **order of the customer**.

- The tax code on the transaction is determined over the standard process in the **purchase order**.
- This order is confirmed by the supplier, followed by a **delivery** of the goods.
- The customer then creates a **goods receipt** and consequently issues the **self-billing invoice**.
- This invoice is transmitted to the supplier, who processes it with their internal open invoice.
- If the values match, a **financial accounting document** is created.

---

## Rebates and Discounts

From an **indirect tax** perspective, **rebates and discounts** reduce the **taxable base amount**.

The rules differ between jurisdictions:

- Sometimes the amount prior to the rebate or discount is to be taxed
- Sometimes the amount after is to be taxed

### SAP Processes for Rebates and Discounts

There are two processes from an SAP perspective:

- **Pricing discounts**
- **Rebate agreements**

**Pricing discounts** are **condition types** that form part of the pricing procedure and are determined directly at the same time as the rest of the sales order, billing document, or purchase order.

In **Figure 4.82**, you can see that SAP standard pricing Procedure **RVAA01** already contains several of those discounts, for example:

- A predefined customer discount determined via a **condition table**
- A manual discount from the gross or net price that can be added directly in the document

In the pricing procedures of the sales order, invoice, or purchase order, such discounts will look as shown in **Figure 4.83**.

### Rebate Agreements

Rebate agreements serve the purpose of providing **condition-based discounts**.

For example, a company may want to offer a discount after the fact at the **end of the quarter** for customers who have bought a certain **volume of product** during the quarter.

At a process level, there are several invoices that accumulate a rebate. At the end of the quarter, the rebate agreement is fulfilled, and a **credit memo** is issued to the customer.

Technically—very briefly put—the **condition contract** is updated for each transaction that contains the defined business volume selection criteria.

From an indirect tax perspective, it’s important to note that:

- Issuing of a conditional rebate after the fact **does reduce the taxable base amount** of all previous transactions but **doesn’t lead to the necessity of correcting all prior invoices**.
- In most jurisdictions, there are two important requirements:
  - A reference to previously issued documents must be made (i.e., the **document number and date**)
  - The credit note must apply the **same indirect tax** as the original document
- All types of **settlement offered by SAP**—whether partial or final—are **taxable transactions**.
- The **tax point** can be different between jurisdictions; for example, the issuance of credit note may be the tax point or the payment of the rebate.

For details on customizing rebate agreements and condition contracts, refer to **SAP Note 2481672**.

---

## Example

Company **Z** sells goods to a customer **Y**. Customer Y has those goods delivered to warehouses in multiple countries, some inside the country of company Z and some outside.

Therefore, customer Y receives invoices both with **indirect tax for domestic supplies** and **without indirect tax for cross-border supplies**.

## Kit Supplies

When issuing a **credit note** over a certain time frame, different applied **indirect tax rates** on the previously issued documents must be taken into account. This is very important because incorrectly reducing the **taxable base** and **indirect tax liability** may lead to **tax evasion**.

**Kit Supplies SAP** offers functionality to create materials that consist of multiple products, called **kits** or **bundles**, via **Transaction CS01**. You can choose a **header material** — in the example shown in Figure 4.84, **TG16_KIT**—to which you assign a **bill of materials (BOM)** in the **Alternative BOM** field. In the example, the kit consists of the service **TG16**, a **nonstock item**, and the product **TG15**, a **standard trading material**.

### Kit Material BOM

When entering the header material in a sales order, the kit items are automatically added to the order as well. Different **item categories** are assigned automatically based on the settings in the SAP configuration menu path:  
**Sales and Distribution • Sales • Sales Document Item • Assign Item Categories** (see Figure 4.85).

The **header material** receives an item category **TAQ** (_Pric. At Header Level_) based on the **item category group ERLA** assigned in the material master data. The items of the kit are assigned an item category **TAE** (_Explanation_) related to the item category of the high-level item.

As shown in the **HL Itm** column of Figure 4.86, items **20** and **30** refer to item **10** as the high-level item, which corresponds to the **TAE** item category.

In the sales order, this means that **pricing** is only done for the high-level item, i.e., cumulatively for all items of the kit. For the BOM items, all **condition types**—including the **indirect tax condition**—are deactivated.

If you have a **service** as part of a kit, it will be treated as an **ancillary service** to the main supply within the kit and will share the same indirect tax treatment.

---

## 4.5 Summary

After reading this chapter, you should be able to customize your **SAP S/4HANA system** for automatic **indirect tax determination** based on **condition logic** according to your business needs.

You’ve also learned about alternative ways of indirect tax determination on the purchasing side and **procurement tools** that produce tax-relevant data. Finally, you should be aware of special processes such as **chain transactions**, **stock transfers**, and **special payment processes** that produce tax-relevant data and may have different requirements depending on the **jurisdiction**.

In the next chapter, we’ll explore alternative ways for indirect tax determination outside of **SAP S/4HANA**.

---

# Chapter 5  
## Indirect Tax Engines and Add-Ons

So far, we’ve focused on **indirect tax determination** based solely on **SAP-native condition logic** within **SAP core processes**, without external functionality.

Now, let’s broaden our scope to consider other engines and add-ons. This chapter provides an overview of alternative **SAP tax determination solutions** and discusses key decision points for tax functions during **SAP S/4HANA projects** or in terms of tax technology solutions.

Figure 5.1 shows the core indirect tax determination solution approaches we will discuss:

- **SAP’s native condition logic**
- **Tax determination add-ons**
- **SAP’s tax service**
- **External tax determination engines**

---

## 5.1 SAP Condition Logic

The **SAP condition logic** is part of the **SAP S/4HANA core functionalities** and provides a tax determination solution without the need for additional software.

The **SAP standard tax determination** supports basic settings covering indirect tax requirements for a limited catalog of business transactions, including:

- **Taxable local supplies** (goods, services) at standard and reduced rates
- **Tax-exempt local supplies**
- **Tax-exempt intra-community supply of goods** (only applies to two-party transactions; SAP standard insufficient for three-party transactions)
- **Tax-exempt exports** (also only applies to two-party transactions)
- Supplies to **special territories** with existing **ISO country codes**
- Exception handling of intra-community supplies of goods to customers **without VAT ID** (local supply with VAT)

Figure 5.2 shows the SAP-native tax determination logic based on the process flow from the **pricing procedure** → **condition type** → **access sequence** → **condition tables**. These concepts were introduced in Chapter 4.

Examples of transactions not covered by SAP standard and requiring further investigation include:

- **Chain supplies**
- **Nontaxable supplies** within a VAT group
- Supply of **services with deviating place of supply** from country of residence of service supplier
- **Work supplies**
- **Intra-community movement of goods**
- **Consignment stock rules**

For a detailed understanding of SAP tax determination based on condition logic and tax-related adjustments, refer to Chapter 4.

---

## 5.2 Add-Ons

Add-on suppliers address the limitations of SAP standard, which doesn’t cover many indirect tax rules. With predefined setups, typical indirect rules are covered directly in SAP based on extended SAP-native setup and condition logic.

User experience with add-ons varies from:

- Solutions with **VAT cockpits** as customer frontends (where customers maintain rules and master data)
- Step-by-step solutions without customer frontends, instead using extended customizing tables with multiple additional tax determination parameters to handle complex transactions

Figure 5.3 shows how the add-on extends tax logic to enhance SAP standard pricing procedures.

---

## 5.3 Tax Service with SAP

SAP offers a **tax service** (previously known as **SAP Localization Hub**), which allows determination and calculation of indirect taxes considering **country-/region-specific issues**.

Note: The tax service has been in **maintenance mode since December 2021**.

Using **SAP Cloud Integration** for data services, you can connect **SAP S/4HANA** and **SAP S/4HANA Cloud** with external tax engines.

The tax service acts as an **external tax calculation engine** in an SAP environment, such as:

- **SAP S/4HANA**
- **SAP Business Technology Platform (SAP BTP)**
- Connection to SAP partner solutions and platforms

The **SAP Integration Suite** connects the SAP customer’s SAP S/4HANA environment with the partner platform (see Figure 5.4).

---

### Tax Service Prerequisite

To use the tax service productively in SAP S/4HANA, you must have an **enterprise global account** on **SAP BTP**.

SAP provides a solution to enable integration between **SAP S/4HANA Cloud** or **SAP S/4HANA** and external tax calculation engines. This ensures a standardized integration template at the SAP BTP level and SAP infrastructure for data exchange.

Tax determination takes place in the solution provided and integrated by the SAP partner.

### Steps for SAP Partner to Join SAP BTP Partner Offerings

1. Integrate the **tax service** and **partner service APIs**
2. Certify the integration of the APIs
3. Become part of the **SAP PartnerEdge program**
4. Get a **support cooperation agreement**
5. Include the **endpoint domain** in the allowed domains list
6. Publish the partner service in the **SAP Store**

Figure 5.4 illustrates the new integration approach based on the **SAP Integration Suite**, the default SAP tool for integration. This suite acts as the router between SAP S/4HANA Cloud or SAP S/4HANA and external partners.

SAP provides **generic integration templates** that must be deployed and configured in SAP Integration Suite. Routes to specific partners or combinations of routes and partners can be defined using parameters in business transaction data from SAP S/4HANA systems.

---

### Tax Service Integration Details

- Standard templates and customizing options exist for enriching data used in tax calculation.
- You can determine combinations of routes and partners based on tax requirements.
- Different business processes can be routed to different partners.

### Audit Log

The **audit log** for tax requests and responses is accessible within **SAP S/4HANA Cloud** and **SAP S/4HANA** for auditing tax service results.

---

### Extending the Tax Service Integration

To handle more sophisticated tax determination setups, extend the tax service integration by following these steps:

1. **Find Extendable Interfaces**  
   Identify interfaces where you can extend the tax service integration and implement the **IF_TXS_CLASS_ENHANCEMENT** interface. Use **Transaction SE80**, select **Repository Browser**, choose **Class/Interface**, and enter `IF_TXS_CLASS_ENHANCEMENT` as the object name. (See Figure 5.5)  
   Read interface documentation to determine which interface suits your extension needs and align with your SAP partner.

2. **Create a New Class**  
   Create a new class and inherit methods from the determined standard class via **Transaction SE24**. Set the superclass in the Properties tab by entering the location of the class implementing your interface. Activate the new class after implementing your tax rule.

3. **Set Your Own Class**  
   Configure the system to use your new class for tax determination.

---

© 2022 by Rheinwerk Publishing Inc., Boston (MA)

## Customizing for Integration with Other SAP Components

Under **Customizing for Integration with Other SAP Components**, choose **SAP Localization Hub, Tax Service • Enhance Localization Interfaces for Tax Service**. Now you can set your own class to replace a **tax service standard localization class** for a determined interface. 

**SAP’s tax service** is available for **SAP S/4HANA Cloud** and **SAP S/4HANA** to be connected to external tax engines via **SAP BTP**. Therefore, **API** and connection configurations are available. 

- Predefined **indirect tax content** isn’t supported, except existing scenarios for **Canada (maintenance mode)** and new functional content/scenarios released for **Brazil**.
- Former tax service configurations, standard integration templates, or connections can be used for tax automation projects in SAP S/4HANA Cloud or SAP S/4HANA.
- No maintenance beyond **Brazil** is supported anymore.

Details for **SAP S/4HANA Cloud** can be found in **SAP Notes 258016 and 2598888**, and for **SAP S/4HANA Cloud, private edition**, refer to **SAP Note 1497956**.

---

## 5.4 External Tax Engines

In comparison to SAP’s tax service, **external tax engines** work with connectors to transfer data between the SAP S/4HANA system and the tax engine. 

- Solution providers offer both **on-premise** and **cloud-based** versions of tax engines.
- On-premise versions are increasingly disappearing due to centralized management by solution providers.
- The only on-premise part that remains is the **cloud connector** and the **SAP add-in for data transfer**.
- Providers offer **private tenants** for tax determination deployment to ensure data protection and security comparable to on-premise alternatives.

From an architectural perspective, a tax engine consists of different layers to be implemented and maintained (see Figure 5.6):

### Data Layer: SAP S/4HANA

- Processes tax calculation parameters recorded in SAP S/4HANA operational modules.
- The data source is mapped based on client-specific requirements and can be extended.
- Solution providers use additional tools for transaction mapping.

### Integration Layer: Tax Connector

- Manages the data interface between SAP S/4HANA and the tax engine.
- The connector is an **ABAP-based add-on** installed on-premise to collect data.
- Tax determination results are written to SAP documents.
- Results can be monitored on an additional layer (e.g., a web-based frontend) or handled as a black box.

### Processing Layer: Tax Rules

- Hosted on the solution provider’s technology platform.
- Serves as a data processing engine.
- Includes configurable rule sets to identify, classify, and declare indirect tax-relevant transactions in operational processes.

### Examples of Indirect Tax Engines

- **Vertex Indirect Tax O Series**: Manages global indirect taxes including sales, use, VAT, communications, and leasing taxes.
- **Thomson Reuters ONESOURCE Determination**: Calculates sales and use tax, GST, VAT, and excise tax globally.
- **Avalara AvaTax**: Engine for sales/use tax, customs and duties, VAT, GST, communications tax, excise tax, consumer use tax, lodging tax, and beverage alcohol tax.

---

## 5.5 Decision Support for Vendor Selection

There are different ways to handle **tax determination in SAP S/4HANA**. It is recommended to perform an assessment across categories relevant to the tax function and corporation.

### Categories to Consider

- **Implementation**

  Implementing indirect tax rules natively in SAP requires effort. Gaps in the SAP standard may require customizing or enhancements (e.g., chain supplies). Add-ons or external engines reduce effort by embedding predefined logic. All options require some implementation effort.

- **Documentation**

  Comprehensive documentation tailored to individual needs is crucial. It serves as the reference for maintenance and change requests, whether from solution providers or in-house.

- **Indirect Tax Content and Maintenance**

  Predefined content from external tax engines competes with manual maintenance of tax rates, rules, and master data in SAP S/4HANA. Maintenance extent varies widely between solutions and contracts. Native SAP setups include time-dependent tax functionality but require recoding and testing for VAT rate and rule changes due to hard-coded tax condition records.

- **IT Resources and User Roles Concept**

  Solution providers maintain tax determination solutions. SAP-native setups require internal IT and maintenance processes. User role concepts must be established for SAP system access.

- **Scalability**

  Consider scalability for global rollouts. Economies of scale can save costs on multi-regional licenses.

  - Add-ons and native SAP setups typically cover regional tax requirements (EU, Asia Pacific, selected jurisdictions).
  - Large tax engines offer global tax rule coverage and easier extension to new jurisdictions via licenses or APIs.
  - Add-ons require embedding in new entities’ systems or integrating acquired companies into one ERP system.
  - SAP-native setups can reuse template setups for EU countries sharing tax requirements to leverage implementation cost savings.

- **In Control**

  Tax engines and VAT add-ons embed VAT controls and flags for noncompliant transactions. Native SAP requires customization for controls.

- **Multiplatform**

  Tax engine licenses can connect multiple ERPs and other financial systems (procurement, expenses, e-commerce). Add-ons embed within SAP S/4HANA systems needing multiple installations. Native SAP requires VAT coding per ERP or financial system.

- **Reporting**

  Native reporting is compatible with all solutions. Reporting processes become aligned with IT processes. Tax monitoring is key for exception management.

- **Version Control**

  Tax engine providers continuously maintain solutions, licensing the latest versions. Add-ons require manual uploads and implementations of updates. Native SAP maintains documentation for updates and change requests.

- **Tax Classifications**

  Material, vendor, and customer classifications impact taxability in both native SAP and add-ons. Tax engines can bypass incorrect classifications to reach correct tax determination.

- **Tax Code Availability**

  Native SAP supports time-dependent tax functionality. However, tax codes are limited by a two-character structure in multi-country systems. Tax engines provide validity date assignment, eliminating the need for new tax codes upon VAT changes.

- **Pricing**

  Choosing between tax engines, add-ons, or native SAP is a trade-off between **maintenance costs** and **license fees**. Early business case calculations are recommended to maximize economies of scale and select the best tax determination solution.

---

## 5.6 Summary

This chapter covered four alternative ways of **tax determination in SAP**, each with pros and cons. Criteria were proposed to select a tax determination solution matching requirements in an SAP S/4HANA environment.

The next chapter discusses **custom coding options for indirect taxes**.

---

## Chapter 6  
Custom Coding for Indirect Taxes

Now that you understand how to configure SAP S/4HANA for indirect tax, this chapter explores options when requirements cannot be met with standard configuration.

It provides examples of **custom coding** using typical RICEFWs (reports, interfaces, conversions, enhancements, forms, and workflows) to enhance SAP standard functionality on both the sales and purchasing sides.

## Key Terms Breakdown

### Reports
**Reports** are executable programs that read data and provide output in a structured way. An example from indirect tax is **report RFUMSV00**, the SAP standard report for tax on sales and purchases (see Chapter 9 for more details).

### Interfaces
**Interfaces** are connections between two or more distinct systems, for example, between the SAP S/4HANA system and an online shop. An interface defines which data is shared between the two systems and in what way. The type and method of transferred data vary strongly, depending on the **manufacturer**, **type of system**, and **purpose** of the external system.

### Conversions
During a **conversion**, data is converted from one format to another, or from one system to another. Think about a migration from a legacy system to **SAP S/4HANA**. You usually migrate data from your old system, mapping your legacy data to the new system.

### Enhancements
In this chapter, we’ll mostly talk about **enhancements**. Enhancements describe any way in which you add your own functionality into the SAP standard. There are several ways to implement enhancements, for example, through **user exits**, **customer exits**, or **business add-ins (BAdIs)**. You’ll find several examples related to indirect tax in the following sections.

### Workflows
**Workflows** describe sequential tasks that process a set of data or document. For indirect tax, you may have an approval workflow for incoming invoices: The invoice is validated against open purchase orders, and then approved by procurement and the tax department.

---

# 6 Custom Coding for Indirect Taxes

To set the stage, we’ll start with a brief discussion of important **ABAP** terms that are relevant for indirect tax. We’ll then explore the key user exits for tax determination in sales and purchasing.

## 6.1 ABAP Basics for Indirect Tax

To begin, we’ll cover basic terminology and coding logic in the SAP programming language **ABAP** that will enable you to understand the example code snippets in the following sections.

**Note on ABAP**  
Be aware that the explanations and examples offered here aren’t a complete overview of ABAP and the enhancement functionality of SAP and don’t replace the involvement of an **ABAP developer** on a project.

### 6.1.1 User Exits, Customer Exits, and Business Add-Ins

In **SAP S/4HANA**, there are several enhancement points you can use to implement your own functionality:

- **Customer exits**  
  These are predefined spaces in an SAP standard program, screen, or menu, where customers can implement their own functionality. We’ll cover how to identify and create customer exits based on **EXIT_SAPLMEKO_001** and **EXIT_SAPLMEKO_002** in Section 6.3.

- **User exits**  
  These serve basically the same purpose as customer exits, but they are only available for the **sales and distribution** functionality. They generally don’t contain a lot of code, but rather call a **function module**. For details on function modules, refer to Section 6.1.3.  

  Classic examples for user exits relevant for indirect tax are **PRICING_PREPARE_TKOMK** or **PRICING_PREPARE_TKOMP** for the sales order and invoice. For more information on these user exits and their purpose, refer to Section 6.2.

- **BAdIs (Business Add-Ins)**  
  These are also predefined locations where a customer’s own functionality can be implemented. However, in comparison to the other two approaches, they can be **reused** within several projects. The appropriate **BAdI** can be identified in the SAP Customizing menu.

  - For BAdIs in **materials management**, go to path:  
    Materials Management • Purchasing • Business Add-Ins for Purchasing.  
    Examples include BAdIs for enhancements to price determination or adopting the tax jurisdiction code from the delivery address.

  - For BAdIs in **sales and distribution**, there are several locations in the SAP Customizing menu, such as:  
    Sales and Distribution • Sales • Sales Documents • Business Add-Ins (BAdIs)  
    Sales and Distribution • Billing • Business Add-Ins (BAdIs)

### 6.1.2 Objects

**Objects**—structures that hold a certain type of data in a specified format—in ABAP are declared with **declarative statements**. They begin with a keyword, usually **DATA**. This tells the program which type (i.e., format) a certain object has.

For example, the net value of a billing document is of type **VBRK-NETWR** (net value), as follows:  
`DATA lv_netwr TYPE vbrk-netwr.`

You can also use less specific data types and directly assign a value to your variable. In the following example, we define a packed number **lv_netwr** with value 100, length 10, and 2 decimal places:  
`DATA: lv_netwr TYPE p VALUE 100 LENGTH 10 DECIMALS 2.`  
The output of printing this variable would be **100.00** with seven preceding spaces. Be aware of this, as it may influence your operations and output of the variable.

#### ABAP Isn’t Case Sensitive

In ABAP, keywords like `DATA` and `TYPE` can be written in uppercase or lowercase letters, and variable names can also be written in any case without affecting the code.

#### Declaring Structures

It’s possible to declare **structures** consisting of multiple variables. For example:

```abap
TYPES: BEGIN OF st_kna1,
  kunnr TYPE kunnr,
  land1 TYPE land1,
END OF st_kna1.
```
*Listing 6.1 Declaration of a Structure*

##### Usage of the Structure

To use the declared structure, you need to declare a variable of this structure type:

```abap
DATA wa_kna1 TYPE st_kna1.
```

Or an internal table of your structure:

```abap
DATA it_kna1 TYPE STANDARD TABLE OF st_kna1.
```

#### Inline Declaration Example

It’s possible to declare variables inline during a `SELECT` statement as well. Example:

```abap
SELECT taxk1, inco1
  FROM vbrk
  WHERE vbeln = lv_vbeln
  INTO @DATA(lt_vbrk).
ENDSELECT.
```
*Listing 6.2 In-Line Declaration of Local Table*

### 6.1.3 Includes, Functions, Subroutines, and Programs

SAP uses several different methods of structuring functionality:

- **Includes**  
  Reusable objects that are included as source code inside the general program code during runtime. All variables of the overall program are available inside an include without needing explicit import/export. An example in indirect tax is include **V05EZZRG**, used for determination of the tax ID and customer tax classification based on the payer business partner role.

- **Function modules**  
  Subprograms of reusable statements that import and export parameters. Example: **READ_EXCHANGE_RATE**—reads the exchange rate between two currencies for a certain date and exchange rate type.

  Use transaction **SE37** to view function modules. You may create custom function modules to enclose modifications inside a certain user exit.

- **Subroutines**  
  Similar to function modules, but only usable inside a certain ABAP program and cannot be reused outside it. Defined using the **FORM** statement.

- **ABAP programs**  
  Units that process data. Executable programs are defined by the keyword **REPORT** and may contain includes, function modules, or subroutines.

### 6.1.4 ABAP Statements

Every statement in ABAP begins with a keyword (e.g., **DATA**, **TYPES**, **SELECT**, **FORM**, **REPORT**) and ends with a full stop (`.`). Statements can span multiple lines.

- **Spaces**  
  Spaces are generally ignored except when significant for the code (e.g., `var = 1+2.` is incorrect, but `var = 1 + 2.` is correct).

Types of statements:

- **Declarative statements**  
  Declare objects, e.g., `DATA` or `TYPES`.

- **Modularization statements**  
  Define processing blocks, e.g., `FUNCTION...ENDFUNCTION` or `FORM...ENDFORM`.

- **Calling and exiting program units**  
  Used to call or exit processing blocks. For example:  
  - `CALL FUNCTION` calls a function module  
  - `EXIT` exits a processing block

- **Program flow logic**  
  Control structures like `IF...ELSEIF...ELSE...ENDIF` and `CASE...WHEN...ENDCASE` help distinguish between cases and apply conditions.

- **Data processing statements**  
  Open SQL statements like `SELECT...ENDSELECT` and `JOIN` are used to access and process data from tables.

---

## 6.2 User Exits for Tax Determination in Sales

In **sales and distribution**, we focus on **user exits** compared to customer exits covered later in Section 6.3.

### Use Enhancement Implementations

It is best practice to create the implementation within a user exit via an **enhancement implementation**. Enhancements can be created in programs such as **MV45AFZZ** or **RV60AFZZ**.

Using enhancements is especially beneficial during system upgrades because they are **transport objects** and not direct modifications. During an SAP upgrade, all SAP objects including modifications are replaced by a newer version.

### Relevant Structures

Focus on the relevant structures and how they are filled. You will encounter:

- **TKOMK**: Communication header for pricing structure  
- **TKOMP**: Communication item for pricing structure

## Pricing Data Structures and Tables

They contain all relevant data for **pricing**, for example, the **tax departure country**, **tax destination country**, **material tax classification**, and **customer tax classification**, and should contain any **custom fields** you’ve created (appended via structures **KOMKAZ** or **KOMPAZ**) that are to be used during pricing.

They are based on structures **KOMK** and **KOMP** and are populated during pricing from different tables, such as table **VBAK** and table **VBAP** for the sales order, and table **VBRK** and table **VBRP** for the billing document. Additionally, some fields are populated from other tables, such as the business data for the sales document, including the **Incoterm**, which is populated by table **VBKD**, or the partner information for the sales document, which is populated by table **VBPA**.

During the pricing processing, the information for certain fields, structures, and tables may be changed. To track these changes, there are different prefixes you should be aware of:

- **X prefix:** A table or structure with prefix **X**, such as table **XVBAK**, contains the **current status** of the object. These are the changes that are written into the database table when the object is saved.
- **Y prefix:** A table or structure with prefix **Y**, such as table **YVBAK**, contains the status of the object **before any changes** were made. This is the object as it was read from the database.

---

## 6.2 User Exits for Tax Determination in Sales

We will cover custom coding for **indirect taxes** using **user exits** for sales orders and invoices. Other relevant system enhancements will also be considered.

### 6.2.1 Sales Orders

The program containing the most relevant **user exits** for indirect tax is program **MV45AFZZ**. It contains a number of user exits for sales document processing. We’ll focus on these user exits in the program:

- **PRICING_PREPARE_TKOMK**
- **PRICING_PREPARE_TKOMP**
- **MOVE_FIELD_TO_VBAK**
- **MOVE_FIELD_TO_VBAP**

Later, we will also look at program **MV45AFZB**.

---

### USEREXIT_PRICING_PREPARE_TKOMK

This user exit is used to **read, write, or modify** further information into the **communication header** for the pricing structure. If you’re using a custom header-level field in a condition table, this is where you pass the value to pricing.

In many countries, the export of goods is **exempt from indirect tax** or valued as **zero-rated** under certain conditions, usually when the goods leave the country. This is easy to prove when your company is responsible for the export. However, if the customer is responsible for the export and is a resident of the country of departure, the exemption cannot be applied.

#### Example:

Consider **Company A in Australia** which sells goods to **Customer B**, a resident of Australia. Customer B picks up the goods at Company A’s warehouse (**Incoterm EXW**) and exports them to their customer C. Under Australian indirect tax law, **Company A must charge GST** on the transaction to B, even though the goods leave the country.

In such cases, you may want to create a condition table including:

- The **residence country** of the customer
- The **Incoterm**

The **Incoterm** is part of structure **KOMK** — the communication header for pricing (**field KOMK-INCO1**). However, the **residence country of the customer** isn’t part of structure KOMK within SAP standard.

After adding the field to structure **KOMKAZ** (refer to Chapter 4, Section 4.1.4), the new field must be filled using information from **TKOMK** — the temporary structure at the time when the user exit is processed.

#### Code Example (Listing 6.3):

```abap
SELECT SINGLE LAND1
FROM KNA1
INTO TKOMK-ZZLANDPY
WHERE KUNNR = TKOMK-KNRZE.
```

This code assigns the **payer country** to a custom field in pricing structure **KOMK**. Business partner roles available in **KOMK** include:

- **KNRZE** - payer
- **KUNNR** - sold-to party
- **KUNRE** - bill-to party
- **KUNWE** - ship-to party

---

### USEREXIT_PRICING_PREPARE_TKOMP

This user exit is used to **read, write, or modify** further information into the **communication item** for the pricing structure. If you create a custom condition table with a custom field on the item level, this is the point you pass the value to pricing.

If your company acts globally, tax determination for all transactions in sales and distribution is based on the **condition technique**.

Using a standard condition table **A011**, you may have combinations of each tax destination country worldwide for each tax departure country. This can result in thousands of condition records to maintain when implementing tax determination across multiple countries or company codes.

Often, it’s useful to **simplify** maintenance to avoid overhead.

---

#### Special Taxation Relationships in Customs Unions

Not all supplies between countries are the same. Countries within the **European Union (EU)** or other tax and customs unions like the **Gulf Cooperation Council** may have:

- Special taxation requirements
- Special reporting requirements such as the **EC Sales List** and **Intrastat**

Transactions must be distinguished not only by tax departure and destination country but also by the relationship between those countries.

---

#### Example: Company P in Poland

- Supplies goods to the **EU countries** (like Slovakia) — qualifies as **indirect tax-exempt intra-community supply**
- Supplies goods to **non-EU countries** (like Switzerland) — qualifies as **indirect tax-exempt export**

These transactions arise different reporting requirements and must be reported in different boxes of the indirect tax return.

---

#### Condition Table with Country Status Indicator

You can create a condition table including an **indicator of the status of the country**. For example, a custom data element **ZXEGLD** appended to structure **KOMPAZ** with values:

- **E** for EU country
- **N** for non-EU country

This uses SAP standard field **XEGLD** from country table **T005** as the EU membership indicator.

---

#### Code Example (Listing 6.4):

```abap
DATA: LV_XEGLD TYPE XEGLD.

IF TKOMK-ALAND NE TKOMK-LAND1.
  SELECT SINGLE XEGLD
  FROM T005
  WHERE LAND1 = TKOMK-LAND1
  INTO @LV_XEGLD.

  IF LV_XEGLD IS INITIAL.
    TKOMP-ZXEGLD = 'N'.
  ELSEIF LV_XEGLD IS NOT INITIAL.
    TKOMP-ZXEGLD = 'E'.
  ENDIF.
ENDIF.
```

This code assigns the **EU indicator** to a field in pricing structure **KOMP**.

---

#### Notes on Customs Union Identification

If you want to identify membership in another customs union (e.g., **Gulf Cooperation Council**) there is no standard SAP indicator. In that case, it is recommended to create a **custom Z table** to manage affiliation, for example:

| Country | Customs Union Indicator |
|---------|------------------------|
| AE      | GCC                    | G |
| SA      | GCC                    | G |
| QA      | GCC                    | G |
| ...     | ...                    | ... |

**Table 6.1** — Example for affiliation to a customs union.

---

### Z Tables

- A **Z table** is a custom table to aggregate information and is **not part of SAP standard**.
- You create a Z table using transaction **SE11**.
- For customs union affiliation detection, define fields like **LV_XEGLD** as simple character type and select from your custom table accordingly.

---

### Example for USEREXIT_PRICING_PREPARE_TKOMP: Place of Supply for Services

Many countries have special rules for the **place of supply** that differ from the country of the ship-to address.

One such example is **business-to-business (B2B) services in the EU**. Per **Article 44 of the European VAT Code**, the place of supply for these services is where the **recipient has established its business**.

---

#### Implementation Example (Listing 6.5):

```abap
IF TKOMP-TAXM1 = 'S'.
  SELECT SINGLE LAND1
  FROM KNA1
  WHERE KUNNR = TKOMK-KNZRE
  INTO TKOMK-LAND1.
ENDIF.
```

This code changes the **tax destination country** for services by assigning the residence country of the payer for materials classified as services (**TAXM1 = 'S'**).

---

#### Important Notes:

- This applies the rule **globally** for all countries.
- For country-specific implementation, create a table listing countries where this rule applies.
- This solution works only if the **sales order contains exclusively services**.
- For mixed goods and services orders, SAP recommends **splitting the sales document**.
- Reference: **SAP Note 1412947** for mixed orders.

---

### USEREXIT_MOVE_FIELD_TO_VBAK / USEREXIT_MOVE_FIELD_TO_VBAP

- **USEREXIT_MOVE_FIELD_TO_VBAK** is used to read, write, or modify information in the standard table **VBAK** (sales order header).
- **USEREXIT_MOVE_FIELD_TO_VBAP** works the same way on the **item level**.
- Many actions possible via these exits can also be done via **PRICING_PREPARE_TKOMK** or **PRICING_PREPARE_TKOMP**.
- The values influence each other: **TKOMK reads from VBAK and writes back to VBAK** at the end of the transaction.

## Using User Exits for Indirect Tax Determination

**USEREXIT_MOVE_FIELD_TO_VBAK** or **USEREXIT_MOVE_FIELD_TO_VBAK** instead of **PRICING_PREPARE_TKOMK** or **PRICING_PREPARE_TKOMP** is especially relevant for information that isn’t available in structures **KOMK** and **KOMP**.

Let’s consider an example involving the **domestic reverse charge mechanism** in most countries of the EU. This mechanism applies when a **provider of construction services** provides services to another provider within the same country. It was created to reduce fraud in an industry heavily characterized by sub-subcontracting.

- **Reverse charge** in the indirect tax context means the **recipient** of goods or services is liable to report and pay the indirect tax, rather than the supplier.

In the customer master data under the **customer business partner role**, SAP enables you to insert a **license** (see Figure 6.1).

### Tax Exemption Licenses

This is one way SAP offers **tax exemption license functionality**. For example, several countries have a **condition type** for tax exemption licenses (e.g., **LCIT**) maintained on a condition-level basis. This license is stored in table **KNVL** and can mean anything specific depending on use.

If you’re a construction provider serving other providers, you can store the license identifying your customer as a construction provider for a specified period.

### Example: Changing Customer Tax Classification for License

In Listing 6.6, we use license data to overwrite the customer tax classification. We do **not change customer tax classification directly** because it’s linked to license validity, reducing risks of incorrect tax determination.

```abap
DATA: ls_knvl TYPE knvl.

SELECT * FROM knvl
  INTO ls_knvl
  WHERE kunnr = xvbak-kunnr AND aland = xvbak-aland AND tatyp = 'TTX1'.
ENDSELECT.

IF ls_knvl-datab <= xvbak-audat AND
   ls_knvl-datbi >= xvbak-audat AND
   ls_knvl-licnr IS NOT INITIAL AND
   ls_knvl-belic NE ' '.
  xvbak-taxk1 = 'C'.
ENDIF.
```

Explanation:

- Declares local variable **ls_knvl** of type **KNVL**.
- Selects matching **tax type TTX1** for customer and tax departure country from **XVBAK**.
- Checks if order date falls within license validity (**DATAB**, **DATBI**) and confirms license number and indicator fields are filled.
- Overwrites **tax classification TAXK1** to 'C' (construction provider) if criteria met.

> This tax classification must be created earlier (see Chapter 3, Section 3.2.1).

If multiple license entries exist per customer, adapt selection by date or loop over entries.

---

## 6.2 User Exits for Tax Determination in Sales

### USEREXIT_NEW_PRICING

**Program MV45AFZB** contains two user exits:

- **USEREXIT_NEW_PRICING_VBAP**
- **USEREXIT_NEW_PRICING_VBKD**

These user exits enable automatic re-pricing when specific item fields change that do not trigger new pricing by standard SAP.

Parameters relevant for indirect tax redetermination include:

- **Supplying plant**
- **Country of ship-to party**
- **Incoterm** (indicates party responsible for transport)

Changing the **Incoterm** in a sales order should re-trigger indirect tax determination to comply with regulations and redetermine the tax code.

Example scenario:

- A customer in the country of departure picks up goods and exports them using Incoterm EXW.
- Domestic supply of goods is relevant under indirect tax.
- Transport to a third country may trigger export of goods in most jurisdictions.

> This example shows an indicator relevant for tax scenarios; not indirect tax advice.

### Example Code: Trigger New Pricing on Incoterm Change

```abap
IF vbkd-inco1 NE *vbkd-inco1.
  new_pricing = 'G'.
ENDIF.
```

---

### 6.2.2 Invoices

**Program RV60AFZZ** contains relevant user exits for indirect tax during sales document processing, including:

- **USEREXIT_PRICING_PREPARE_TKOMK**
- **USEREXIT_NUMBER_RANGE**

These are important for billing documents.

#### USEREXIT_PRICING_PREPARE_TKOMK for Billing Documents

This user exit is called when a **billing document** is created and allows modification of pricing communication structures.

Example:

- In some jurisdictions, **credit notes** or document reversals must be reported differently on indirect tax returns.
- Credit notes have billing type **G2** in SAP standard.

Two options for tax determination:

1. Create condition table including billing type
2. Overwrite primary tax determination parameters (e.g., material or customer tax classification)

Option 2 often preferred for simpler maintenance when one access sequence serves multiple company codes.

#### Z Table Example for Overwriting Material Tax Classification

- Defines combinations of **Sales Organization**, **Billing Type**, and **Material Tax Classification**.

| Sales Organization | Billing Type | Material Tax Classification |
|--------------------|--------------|-----------------------------|
| 0001               | G2           | C                           |

**C** stands for **correction**, a material tax classification created previously (see Chapter 3, Section 3.2.1).

#### Listing 6.8: Code Snippet to Change Material Tax Classification

```abap
DATA: lv_taxm1 TYPE taxm1.

SELECT SINGLE taxm1 INTO lv_taxm1
  FROM zztaxm1
  WHERE vkorg = tkomk-vkorg AND fkart = tkomk-fkart.

IF lv_taxm1 IS NOT INITIAL.
  tkomk-taxm1 = lv_taxm1.
ENDIF.
```

Explanation:

- Declares local variable **lv_taxm1** to hold material tax classification.
- Selects classification matching **sales organization** and **billing type** from Z table.
- Overwrites **tkomk-taxm1** if a classification is found.

---

#### USEREXIT_NUMBER_RANGE for Billing Documents

- Controls internal number range on billing document creation.
- Number ranges in SAP standard are assigned at billing type level.
- Consecutive numbering without gaps is often required per jurisdiction.

##### Z Table for Number Range Assignment

| VKORG | LANDTX | FKART | NUMKI |
|-------|--------|-------|-------|
| 0001  | PT     | F2    | 10    |
| 0001  | PT     | G2    | 11    |

##### Listing 6.9: Change Number Range Code Snippet

```abap
SELECT SINGLE numki
  INTO us_range_intern
  FROM zznumberrange
  WHERE vkorg = vbrk-vkorg AND fkart = vbrk-fkart AND landtx = vbrk-landtx.
```

---

### 6.2.3 Additional Enhancements

Besides sales order and billing document user exits, other enhancements exist for indirect tax implementation in SAP S/4HANA.

#### Access Requirements

- Control access to condition tables within an access sequence.
- Consist of a piece of code within a **routine**.

To access pricing routines:

- Use Transaction **VOFM**
- Choose **Requirements** > **Pricing**

This leads to an overview of pricing routines (see Figure 6.2).

The most relevant indirect tax routines in SAP standard are:

- **Routine 7:** Domestic business
- **Routine 8:** Export business

## Activating and Modifying Routines

To reach the **source code**, simply **double-click the routine**.

### Activating the Routines

- If you change or create a new routine, run **report RV80GHEN** to activate the routines.
- This must be done in all following systems after transport of the routine.
- Best practice: leave **SAP standard routines** unchanged, and copy them into a new routine within the **customer namespace**.
- Use a **number starting with 9** for customer routines, e.g., 907 for the copy of routine 7.

### Example: Routine 8 (Export Business)

The code in **Listing 6.10** is mostly **SAP standard**. To extend it, it’s necessary to understand its structure.

- The routine is structured as a **subroutine** called during execution of a higher program.
- The program starts by setting **sy-subrc = 4 (error)**, meaning the access to a condition table is not fulfilled.
- There are **three lines** introduced by keyword **check**: these ensure three conditions are met, else the subroutine terminates.
  
Conditions checked:
- **Tax departure country** (komk-aland) and **tax destination country** (komk-land1) must not be empty and must be different.
- The routine applies **only for tax conditions** (condition class `D` in field komt1-koaid).
  
Inside the IF condition:
- There is a cumulative IF condition with three **AND** statements followed by an **exit** statement.

Two options fulfill the access requirement (not exit):
1. Both countries are in the **EU**, and the customer has a **European VAT ID number** available (field komk-stceg).
2. One or both countries are not in the EU, making the VAT ID irrelevant.

If both countries are in the EU and VAT ID is empty, the program **exits with return value 4 (error)**.

### Code Sample: Listing 6.10 Access Requirement 08 for Tax Conditions

```abap
form kobed_008.
  sy-subrc = 4.
  check: komk-aland ne space.
  check: komk-land1 ne space.
  check: komk-aland ne komk-land1.
  if (komt1-koaid eq 'D'). "Only for tax conditions
    if (at005-xegld eq 'X') and "participant european community
       (et005-xegld eq 'X') and
       (komk-stceg is initial).
      exit.
    endif.
  endif.
  sy-subrc = 0.
endform.

* Prestep
form kobev_008.
  sy-subrc = 4.
  check: komk-aland ne space.
  check: komk-land1 ne space.
  check: komk-aland ne komk-land1.
  sy-subrc = 0.
endform.
```

### Extension Scenarios for Routine 8

- Check for **special territories** like Northern Ireland or Canary Islands.
- Check for the **country of the VAT ID**.
- Since **2020**, EU rule: If the middleman uses the VAT ID of the departure country, the middleman is responsible for transport.
  - The first transaction is subject to **domestic VAT**.
  - The second transaction may be **VAT exempt**.

---

## 6.2 User Exits for Tax Determination in Sales

### Tax ID Determination

- If using **rule A (sold-to party)** or **rule B (payer)** for tax ID determination, you can enhance the process via **includes**.
- Includes:
  - Sold-to party: **V05EZZAG**
  - Payer: **V05EZZRG**
- The logic is the same; suffix depends on partner role.

### Example: Using Include V05EZZAG

- Check if the **tax ID of the sold-to party (kuagv-stceg)** is empty.
- Check if the **tax ID of the customer’s residence country (lkna1-stceg)** is not empty and different from tax departure country (vtcom-lland).
- Load country data from **table T005** if not already loaded.
- If tax destination country is an **EU country** (`t005-xegld`), fill tax ID from **customer master data**.

#### Code Sample: Listing 6.11 Tax ID Determination in Include V05EZZAG

```abap
IF kuagv-stceg IS INITIAL.
  IF NOT lkna1-stceg IS initial AND vtcom-lland NE lkna1-land1.
    IF t005-land1 NE vtcom-lland.
      SELECT SINGLE * FROM t005 WHERE land1 = vtcom-lland.
    ENDIF.
    IF t005-land1 = vtcom-lland AND NOT t005-xegld IS INITIAL.
      kuagv-stceg = lkna1-stceg.
      kuagv-stceg_l = lkna1-land1.
    ENDIF.
  ENDIF.
ENDIF.
```

### Additional Notes on User Exits

- There are many more **user exits and enhancement points** for indirect tax requirements.
- Always assess **individual legal and functional requirements** during SAP S/4HANA implementation.

---

## Standard Tax Determination Parameters in Sales and Distribution

All information required for **tax determination** should be available in best quality.

| Data Description                | Data Field          | Source                                 |
|-------------------------------|---------------------|--------------------------------------|
| Tax departure country          | KOMK-ALAND          | Supplying plant (VBAP-WERKS/VBRP-WERKS) |
| Tax destination country        | KOMK-LAND1          | Ship-to address (KNA1-LAND1)          |
| VAT ID of the customer         | KOMK-STCEG          | KNA1-STCEG / KNAS-STCEG               |
| Material tax classification    | VBAP-TAXM1          | MLAN-TAXM1 (plant & sales org)        |
| Customer tax classification    | VBAK-TAXK1          | KNVI-TAXKD (sales org, dist. channel, division) |

---

## 6.3 Customer Exits for Tax Determination in Purchasing

### 6.3.1 Identifying Customer Exits

- **User exits** require knowing the program containing them.
- **Customer exits** are identified via **Transaction SMOD**:
  - Enter enhancement name, or use **F4 help** with keywords (e.g., "pricing").
  
- Enhancement names for communications structure KOMK and KOMP:
  - **LMEKO001** and **LMEKO002** respectively.
- View the **components** (function exits) by selecting Components radio button and clicking Display.

### Using Transaction CMOD for Customer Exit Projects

- Use **Transaction CMOD** to view, create, and modify customer projects where enhancements are implemented.
- Steps to create a project:
  1. Enter project name (e.g., `"Z_ITX_AP"`).
  2. Select **Enhancement Assignment** under Subobjects.
  3. Click **Create**.
  4. Enter a short text description for the project.
  5. Click **Enhancement assignments** button.
  6. Enter desired enhancements to activate them.

- Note: If you add code but don’t assign the enhancement to a project, it will **not be called** during pricing execution.

### Accessing Source Code and Implementing Exits

- In **Transaction SMOD** overview, double-click function module name to access source code.
- Cannot change function module source code directly (e.g., EXIT_SAPLMEKO_001).
- Instead, create an **implementation of the include** inside the exit by double-clicking on it.

## 6.3 Customer Exits for Tax Determination in Purchasing

### Warning and Implementation

There will be a **warning** on the bottom of the screen (see Figure 6.8). Simply press **(Enter)** to continue.

---

### Figure 6.8 Warning for Implementation of Include

In the popup shown in **Figure 6.9**, click **Yes**, and enter the **transport** and **package**.

---

### Figure 6.9 Create Include for EXIT SAPLMEKO

---

### These Exits Aren’t Includes but Functions

The enhancements in Transaction **SMOD** aren’t includes like we’ve used with the user exits in **sales and distribution**, but rather **functions**. 

This means that in contrast to sales and distribution user exits where you can see, use, and modify all relevant data, in **materials management**, you can only use the **tables** and **variables** explicitly imported in the declaration of the function.

---

### Relevant Information for Indirect Tax Determination

The most relevant information for **indirect tax determination** on the purchase order is stored in:

- Structures **KOMK** and **KOMP**
- Tables **EKKO**, **EKPO**, and **LFA1**

If you’re using **purchase info records**, tables **EINA** and **EINE** will also be relevant.

---

### Pricing Structures KOMK and KOMP

Similarly to tax determination in sales and distribution, structures **KOMK** and **KOMP** store information for **pricing** at the header and item level. However, they are **filled differently** between the sales order and billing document.

- **KOMK** contains a field for **billing type**, which is empty in pricing for purchase orders.
- Some fields used in sales and distribution pricing aren’t filled for materials management pricing.

For example, the **customer tax classification** is part of **customer master data** (**KNA1**) not **supplier master data** (**LFA1**), so it isn’t filled despite connection in **SAP S/4HANA** between customer and vendor master data via the **business partner**.

---

### Purchase Order Standard Tables

- **EKKO** and **EKPO** contain purchase order data.
- **LFA1** contains supplier master data.

---

### Purchase Info Records and Indirect Tax Determination

Purchase info record information is stored in:

- **EINA** — general info for purchase info record
- **EINE** — purchase info record applied to purchasing documents, including info record number, purchasing organization, and tax code

---

## 6.3.2 EXIT_SAPLMEKO_001

---

### Purpose

The exit **EXIT_SAPLMEKO_001** allows you to change the values in structure **KOMK**, the communication header for pricing.

This exit is a **function** (not an include), meaning you can only use **tables** and **variables** explicitly imported in the function declaration.

---

### Imported and Exported Variables and Tables for EXIT_SAPLMEKO_001

```abap
*" Lokale Schnittstelle:
*" IMPORTING
*" VALUE(I_EKKO) LIKE EKKO STRUCTURE EKKO OPTIONAL
*" VALUE(I_EKPO) LIKE EKPO STRUCTURE EKPO OPTIONAL
*" VALUE(I_LFA1) LIKE LFA1 STRUCTURE LFA1 OPTIONAL
*" VALUE(I_T001) LIKE T001 STRUCTURE T001 OPTIONAL
*" VALUE(I_LFM1) LIKE LFM1 STRUCTURE LFM1 OPTIONAL
*" VALUE(I_T001W) LIKE T001W STRUCTURE T001W OPTIONAL
*" VALUE(I_KOMK) LIKE KOMK STRUCTURE KOMK
*" VALUE(I_EINA) LIKE EINA STRUCTURE EINA OPTIONAL
*" VALUE(I_EINE) LIKE EINE STRUCTURE EINE OPTIONAL
*" VALUE(I_WEDATEN) LIKE MEPRWE STRUCTURE MEPRWE OPTIONAL
*" TABLES
*" T_EKPA STRUCTURE EKPA OPTIONAL
*" CHANGING
*" VALUE(E_KOMK) LIKE KOMK STRUCTURE KOMK
```

---

### Imported Tables Explanation

- **EKKO**, **EKPO** — purchasing document header and item
- **LFA1** — vendor master data
- **T001** — company code data
- **LFM1** — vendor info by purchasing organization
- **T001W** — plant data
- **KOMK** — structure for pricing header
- **EINA**, **EINE** — purchasing record information
- **MEPRWE** — pricing date information
- **EKPA** — partner role table

---

### Include ZXM06U14 Usage

**EXIT_SAPLMEKO_001** contains only the **include ZXM06U14**, implemented earlier. An include means the code is inserted into the program at runtime.

---

### Example Scenario

Company **C**, a car dealer, buys cars from other businesses and private individuals. In most jurisdictions, transactions by **private individuals** aren’t subject to indirect tax.

- On the output side, the **customer tax classification** is used for tax determination.
- On the input side, this indicator doesn’t exist in materials management pricing.

Our goal is to use the **customer tax classification** in materials management pricing.

---

### Table 6.5 Customer Tax Classification for Materials Management Tax Determination

| LLAND (Tax Destination Country) | TAXIM (Tax Indicator Material) | TAXIL (Tax Indicator Import) | TAXIW (Tax Indicator Plant) | TAXK1 (Tax Classification) |
|---------------------------------|-------------------------------|-----------------------------|----------------------------|-----------------------------|
| DE                              | 1                             | 0                           | 1                          | 1                           |
| DE                              | 1                             | 0                           | 1                          | N                           |

---

### Solution 1: Determine Customer Tax Classification by Vendor Tax ID

- Check if the **tax ID** exists in the vendor master data (field **STCEG** for European VAT ID).
- If **tax ID exists**, set **TAXK1 = 1** (taxable).
- If **tax ID does not exist**, set **TAXK1 = 'N'** (natural person).

**Note:** 'N' is not a standard SAP customer tax classification; must be created first (see Chapter 3, Section 3.2.1).

---

#### Listing 6.13 Code Example

```abap
IF I_LFA1-STCEG IS NOT INITIAL.
  E_KOMK-TAXK1 = 1.
ELSEIF I_LFA1-STCEG IS INITIAL.
  E_KOMK-TAXK1 = 'N'.
ENDIF.
```

---

### Solution 2: Determine Customer Tax Classification via Business Partner Link

Use the connection between **vendor** and **customer master data** via the **business partner** in SAP S/4HANA.

- Select **TAXKD** from table **KNVI** into **KOMK** where:
  - Customer number = Supplier linked via business partner
  - Country of tax classification = Tax destination country

---

#### Listing 6.14 Code Example

```abap
SELECT SINGLE TAXKD
  FROM KNVI
  LEFT JOIN I_BUSINESSPARTNERSUPPLIER AS IBPS ON IBPS~SUPPLIER = @I_LFA1-LIFNR
  LEFT JOIN I_BUSINESSPARTNERCUSTOMER AS IBPC ON IBPS~BUSINESSPARTNER = IBPC~BUSINESSPARTNER
  WHERE KUNNR = IBPC~CUSTOMER AND ALAND = @I_KOMK-LAND1
  INTO @E_KOMK-TAXK1.
```

---

### Figure 6.11 Business Partner to Supplier and Customer Relation Tables

- Supplier number, business partner number, and customer number can be the same (best practice but not always true).
- Use **CDS views** (e.g., **IBPSUPPLIER**, **IBUPACUSTOMER**) instead of actual tables for better performance and authorization access.

---

## 6.3.3 EXIT_SAPLMEKO_002

---

### Purpose

The exit **EXIT_SAPLMEKO_002** allows you to change the values in structure **KOMP**, the communication item for pricing.

As with **EXIT_SAPLMEKO_001**, this exit is a function with explicitly imported tables and variables only.

---

### Example Scenario: Company P Customs Warehouse

Company **P** has an organizational setup with one plant per country and several storage locations, including a **customs warehouse**.

- **Customs warehouses** receive special indirect tax treatment.
- Goods stored physically but not fully imported, they cannot be sold before import.

---

### Figure 6.12 Organizational Setup of Company P

| Company Code | Plant India | Plant China | Plant Bulgaria |
|--------------|-------------|-------------|----------------|
| Warehouse 1  | TAXIW 1     |             |                |
| Warehouse 2  | TAXIW 1     |             |                |
| Customs Warehouse | TAXIW 0 |             |                |

---

### Tax Indicator Plant (TAXIW)

- Assigned only on the **plant level**.
- Creating a new plant is possible but requires high master data maintenance.
- Alternative: change **tax indicator plant** on the **item level** for the customs warehouse storage location.

---

### Custom Table ZZS_CustomsWarehouse

Create a **Z table** named **ZZS_CustomsWarehouse** containing:

- **BUKRS** — company code
- **WERKS** — plant
- **LGORT** — storage location
- **TAXIW** — tax indicator plant

---

### Code to Change Tax Indicator Plant

---

#### Listing 6.15 Example Code

```abap
DATA: LV_TAXIW TYPE KOMP-TAXIW.
SELECT SINGLE TAXIW
  FROM ZZS_CUSTOMSWAREHOUSE
  WHERE BUKRS = I_KOMK-BUKRS AND WERKS = I_EKPO-WERKS AND LGORT = I_EKPO-LGORT
  INTO E_KOMP-TAXIW.
IF LV_TAXIW IS NOT INITIAL.
  E_KOMP-TAXIW = LV_TAXIW.
ENDIF.
```

---

## 6.3.4 Additional Enhancements

Many relevant system enhancements in the **procure-to-pay** process (also in **order-to-cash** and **logistics**) include interfaces to **external systems**.

## Examples of External Systems in Materials Management

A few examples of **presystems** for **order management** include **SAP Ariba** or alternatives mentioned in *Chapter 4, Section 4.2.5*, other supplier online shops, or purchase management systems. Additionally, **optical character recognition (OCR)** software is commonly used.

These **external systems** usually have an interface to the system. A large portion of the **materials management system enhancements** involves the mapping and integration of external systems into the **SAP S/4HANA system**.

| BUKRS (Company Code) | WERKS (Plant) | LGORT (Storage Location) | TAXIW (Tax Indicator Plant) |
|---------------------|--------------|--------------------------|-----------------------------|
| 1000                | 0001         | 0003                     | 0                           |

*Table 6.6 Example Table ZZS_CustomsWarehouse*

---

## 6.4 Summary

Generally, **SAP** offers a number of **technologies to realize interfaces**, such as:

- **Remote function call (RFC)**
- **Business Application Programming Interface (BAPI)**
- **Intermediate document (IDoc)**
- **Simple Object Access Protocol (SOAP)**
- **Representational state transfer (REST)**

The best technology for a certain interface must be **decided on a case-by-case basis**.

From an **indirect tax perspective**, the most important point is that **all information required for tax determination is available in the best possible quality**. For example, a **pre-system** may enable users to enter information into **text fields** that are mapped to SAP. Classic examples are **Incoterms** or **delivery addresses**, which are often entered manually and can lead to **indirect tax risks**.

Adequate **controls and validations** should be in place for **interface data**.

---

## Standard Tax Determination Parameters for Materials Management

Table 6.7 shows the **standard parameters** that must always be available and their usual sources.

| Data Description         | Data Field       | Source                                         |
|------------------------|-----------------|----------------------------------------------|
| **Tax departure country** | KOMK-ALAND      | LFA1-LAND1 (derived from country of supplier) |
| **Tax destination country**| KOMK-LAND1     | T001W-LAND1 (derived from country of receiving plant) |
| **Tax indicator import**   | KOMP-TAXIL     | Populated from ME_FILL_KOMP_PO. If departure country equals destination country → TAXIL = 0 <br> If departure country isn’t equal to destination country: <br>&nbsp;&nbsp;- If departure country is non-EU → TAXIL = 1 <br>&nbsp;&nbsp;- If departure country is EU → TAXIL = 2 |
| **Tax indicator plant**    | KOMP-TAXIW     | T001W-TAXIW                                    |
| **Tax indicator material** | KOMP-TAXIM     | MLAN-TAXIM                                     |
| **Material group**         | EKPO-MATKL     | EKPO-MATKL                                     |

*Table 6.7 SAP Standard Tax Determination Parameters in Materials Management*

---

Enhancements like this should always be **tested in detail** with positive and negative tests.

- **Positive tests** are test cases that include the **expected result**. For example, if you overwrite the **tax destination country** for the supply of service, you would test a supply of service with a delivery address different from the customer's country of residence.
- **Negative tests** ensure that there are **no interferences in other areas** and that the enhancement can handle **invalid or unexpected input**. For example, test a normal supply of goods and ensure that the tax destination country isn’t overwritten.

Despite coding examples, it is recommended to involve an **implementation partner** who has the **technical knowledge** to understand, implement, and test these requirements.

---

# Chapter 7  
## Direct Taxes in SAP S/4HANA

In this chapter, we explain the **capabilities SAP S/4HANA offers in direct taxes** and how to implement them. This provides the basis for **highly automated income tax reporting and compliance processes**, contributing significantly to a **data-driven tax function**.

**Direct taxes** include all types of taxes assessed and levied at the level of the taxpayer, based on the **profit or taxable income of the company**. This particularly covers income taxes (e.g., **corporate income tax**), including **withholding tax (WHT)** as a special form of levy.

---

We begin with the **essential tasks of the direct tax function** and then explain how to support the **end-to-end tax process** through organizational and master data structures available in **SAP S/4HANA** and SAP-integrated applications.

Examples include:

- **SAP Tax Compliance**
- **SAP S/4HANA embedded analytics**
- **SAP Profitability and Performance Management**
- **SAP Analytics Cloud**

This chapter will help you understand the major areas in SAP S/4HANA from a **direct tax perspective**, allowing you to incorporate tax requirements into your SAP S/4HANA implementation project structurally.

---

## 7.1 Direct Tax Basics

The **tax function** in direct taxes has a wide range of tasks, mainly including:

- Supporting the preparation of **annual financial statements** in the context of **tax reporting**. This ensures the correct presentation of the **net assets, financial position, and results of operations** in individual and consolidated financial statements from an income tax perspective.
- Fulfillment of **tax filing obligations**, including submission of **corporate income tax and trade tax returns**, along with tax balance sheets in electronic form.
- Management of **tax audits**, including evaluation of tax audit findings and their impact on financial statements and assessments.
- Monitoring **income tax risks** as part of **tax compliance management** through process-integrated/ERP-based controls.
- Fulfillment of **tax withholding obligations** in current business operations.

**SAP S/4HANA** supports the tax function in all these areas with comprehensive capabilities.

---

It is crucial to see the implementation of **SAP S/4HANA for direct taxes** as an **opportunity** and take advantage of its potential.

---

### 7.1.1 End-to-End Process for Direct Taxes

The **tasks of the tax function** tie in with the **record-to-report scenario** of the company. For **direct taxes**, these tasks extend to **tax-owned end-to-end processes**, illustrated in Figure 7.1.

---

### Figure 7.1 End-to-End Process for Direct Taxes

| Phase                         | Key Elements                                                         |
|-------------------------------|----------------------------------------------------------------------|
| **Data Capturing + Tax Determination** | Create **Customizing structures** to capture **tax-relevant data** with quality, granularity, and availability for operational business. Enable **automated tax determination**. SAP S/4HANA serves as a **data provider** for tax functions. |
| **Tax Data Analytics + Monitoring**      | Analyze **tax data quality** using SAP Tax Compliance, SAP S/4HANA embedded analytics, and **core data services (CDS)**. Operationalize tax control frameworks. |
| **Tax Filing + Reporting**               | Connect tax data in SAP S/4HANA with software solutions for **tax filing and reporting**. Use **SAP Document and Reporting Compliance** for WHT and compliance with **Standard Audit File for Tax (SAF-T)** requirements. |
| **Tax Planning**                         | Use **SAP Analytics Cloud** for direct tax planning, suitable for various planning scenarios. |

---

In the **first phase**, you address important questions such as:

- How to map **tax balance sheets** along the tax lifecycle using **parallel ledgers**?
- Which **organizational structures** need to be created to map tax-related corporate structures properly?
- How to define **master data structures** to support recording of tax-relevant facts?
- How to design **fixed asset accounting** to meet parallel tax valuation areas?
- How to set up the **chart of accounts** to enable automated transfer of balances for tax reporting and e-balance sheet purposes?
- What alternatives to the chart of accounts exist, such as the use of **tax tagging concepts**?
- How to set up the **basic or extended WHT function** to efficiently support tax determination and reporting?

---

After familiarizing yourself with these questions and Customizing settings in Sections 7.2 and 7.3, you will be closer to efficiently setting up your **tax function for direct taxes**.

---

The **second phase**, **tax data analytics and monitoring**, focuses on whether the tax data meets **quality requirements**. Section 7.5 covers **SAP Tax Compliance**, **SAP S/4HANA embedded analytics**, and suggestions for analyzing and monitoring direct taxes.

---

In the **third phase**, **tax filing and reporting**, connect tax data in SAP S/4HANA with **third-party applications** and use **standard SAP reports** for tax preparations.

**SAP Document and Reporting Compliance** is significant here, especially for **WHT** and compliance with **SAF-T** requirements from the **OECD** for companies with extensive foreign activities. This is covered in Section 7.6.

---

In the **fourth phase**, **tax planning** is enabled by **SAP Analytics Cloud**. Section 7.7 explains its functions, planning scenarios, and initial implementation use cases.

---

### 7.1.2 Direct Tax Organizational Structures and Master Data

Setting up **organizational structures** and **master data** is a core task in SAP S/4HANA implementation. These form the central basis for the system's subsequent **operational use**.

## Direct Taxes in SAP S/4HANA

Because it’s often no longer possible to change settings at a later point in time, or only with a disproportionate amount of effort, the **tax requirements for organizational structures and master data** must be incorporated into the implementation project at an early stage. In this section, we’ll provide you with the essential requirements for the area of **direct taxes** and recommendations on how to implement them with regard to your daily work.

In detail, we’ll take a closer look at the **organizational structures** and **master data** that have a tax significance.

### Tax Organizational Structures in Direct Taxes

Tax organizational structures in direct taxes include the following:

- **Ledger**
- **Company code** / further organizational units
- **Chart of accounts**
- **Balance sheet structures** / e-balance sheet
- **WHT** (Withholding Tax)

### Master Data in the Area of Direct Taxes

Master data in the area of direct taxes include the following:

- **Personal account master data** (business partner: debtors, creditors)
- **General ledger account master data**
- **Asset master data**

Even though you’ll be working as a **tax function** at the user level in SAP S/4HANA and **customizing** will be left to your IT team, we’ll provide insights into customizing so you can better understand and address tax requirements in **organizational structures** and **master data**.

You may have the option of accessing a **sandbox** or **test environment** as a tax function, which gives you the opportunity to familiarize yourself with the settings in **Customizing**.

Provided you have the necessary authorizations, you can access Customizing by selecting **Transaction SPRO** in the SAP Easy Access menu. After executing Transaction SPRO, you can display the **SAP Reference Implementation Guide (IMG)**. In the IMG, most settings relevant for direct taxes can be found under **Enterprise Structure** or **Financial Accounting**.

© 2022 by Rheinwerk Publishing Inc., Boston (MA)

---

## 7.2 Organizational Structures

After you’ve opened the **IMG**, you can jump from here to the respective customizing activities. You’ll learn about the most important **customizing settings** for direct taxes in the following sections.

First, we’ll introduce you to the basic **tax-relevant organizational structures** in SAP S/4HANA. As already mentioned, these include in particular the customizing settings for:

- **Ledger**
- **Company code**
- **Chart of accounts**
- **WHT**

### 7.2.1 Ledger

Pursuant to local tax provisions, taxpayers usually must prepare a **tax balance sheet** that differs from generally accepted accounting principles under commercial law.

Due to special **tax accounting rules**, there are often significant differences between the **commercial** and the **tax balance sheet** across the enterprise.

Compared to the presentation of a commercial balance sheet, the tax balance sheet poses the particular challenge of anticipating different phases in the preparation of the tax balance sheet, the **tax lifecycle**:

1. **Preparation of the annual financial statements**  
   In this first phase, the **tax balance sheet** serves as the basis for determining **current tax provisions** and **deferred taxes** by comparing commercial and tax balance sheet positions. As a rule, not all detailed information from a tax perspective is available at this point.

2. **Preparation of annual tax returns**  
   In the second phase, the completeness and correctness of the tax balance sheet prepared in the annual financial statements phase are checked. This serves as the basis for preparing the **annual tax returns**. Any necessary adjustments are made via **return-to-provision adjustments** before the tax balance sheet is submitted to the tax authorities as an enclosure to the tax return.

3. **Tax audit**  
   In the third phase, the tax returns submitted for the tax audit period, including tax balance sheets, are taken up by the tax authorities. In the event of differing legal opinions, **tax audit findings** are often assessed regarding tax accounting. As **audit-to-return adjustments**, these may lead to further differences between the commercial and tax balance sheets (**tax auditor’s balance sheet**). These must be developed further in subsequent years based on the tax audit period.

During these phases, the **tax balance sheet** is subject to ongoing changes called **true-ups**.

---

Due to its scope and complexity, this process of tax balance sheet preparation should be supported by your **ERP system** as much as possible. This applies all the more as the profit effects from differences between the commercial and tax balance sheets, together with off-balance sheet items such as **nondeductible business expenses** and **tax-exempt income**, form the main reconciliation items between the annual net profit under commercial law and the determination of the tax base.

We'll walk through key **direct tax ledger concepts** in the following sections, including the framework in SAP S/4HANA, and its setup and use in business operations.

### Account Solution versus Ledger Solution

Until the introduction of **SAP S/4HANA**, the picture in practice regarding the process of tax balance sheet preparation was very heterogeneous.

For the majority, the tax balance sheet was prepared **outside the ERP system** based on Microsoft Excel or with help from a **tax balance sheet tool**, if necessary, combined with the **account solution** in SAP, limited to the time of preparation of the annual financial statements, as shown in **Figure 7.2**.

Within the framework of the **account solution**, parallel accounting principles are represented by **parallel accounts within a single general ledger**.

All postings are initially made according to the accounting principles of a previously defined **leading general ledger** (e.g., **US GAAP** or **IFRS**). Deviating valuations in the tax balance sheet are then taken into account via **delta postings**.

For this purpose, **adjustment accounts (L-accounts)** are created for the respective account of the leading general ledger, and the delta postings are recorded in the adjustment accounts. The actual tax balance sheet value of a balance sheet item is the balance of the underlying commercial balance sheet value and the adjustment entry to the tax balance sheet valuation.

**Figure 7.2 Account Solution**

However, because the account solution directly uses the commercial general ledger which is closed with the preparation of the annual financial statements, the account solution isn’t suitable for implementing the complete **tax lifecycle** from the preparation of the annual financial statements to the incorporation of tax audit findings.

The other phases of the tax lifecycle can only be mapped **outside SAP**. As a result, the preparation of the tax balance sheet according to this approach is characterized to a large extent by **media disruptions** and **manual activities**.

The desire for a **single point of truth** for the tax balance sheet is therefore largely missed.

---

In **SAP S/4HANA**, the aim should be a **complete mapping** of the tax balance sheet in the ERP system along the entire tax lifecycle.

Full posting of the tax balance sheet in the ERP system creates a central prerequisite for automated processing of the tax lifecycle from the creation of tax reporting to the preparation of tax returns or ERP-based tax planning.

You can use the implementation of SAP S/4HANA to establish the **tax balance sheet as a single source of truth** for the tax function.

SAP S/4HANA offers new possibilities to achieve this goal by combining different **ledger functions**.

The concept of mapping parallel accounting rules via ledgers isn’t fundamentally new and is probably already familiar to those who have worked in SAP before.

In contrast to the account solution, parallel accounting principles can be represented using **parallel general ledgers** in the **ledger solution**. Here, a separate general ledger is available for each relevant accounting principle of the company.

**Figure 7.3 Overview of the Ledger Solution**

---

In SAP S/4HANA, general ledgers managed in parallel are taken into account via **standard ledgers (fixed ledgers)**.

Companies that want to prepare **consolidated financial statements** in accordance with **international accounting standards** and at the same time use separate general ledgers for commercial and tax purposes generally create three parallel standard ledgers:

- **Standard ledger: Consolidated financial statements** (e.g., IFRS)
- **Standard ledger: Individual financial statements under commercial law**
- **Standard ledger: Tax balance sheet**

With this procedure, a **standard ledger** must be defined as the **leading ledger** in Customizing, which inherits all business transactions to the other standard ledgers.

This enables you to efficiently derive your tax balance sheet from the leading standard ledger in SAP S/4HANA.

---

### Ledger Solution

**Parallel Accounting through Parallel Ledgers**

| IFRS Ledger X | Local Tax Ledger Y | Ledger Z |

Because an additional **standard ledger** for the tax balance sheet inherits all business transactions of the leading ledger, it’s only necessary to intervene if there are differences in accounting, for example, between the commercial and tax balance sheets.

#### Example

Let’s consider an example of using parallel ledgers.

In the commercial balance sheet, a **provision of EUR 1,000** is recognized; in the tax balance sheet, this provision may not be recognized and the carrying amount in the tax balance sheet is **EUR 0**.

This results in the following different balance sheet approaches in the parallel ledgers for the commercial and tax balance sheets:

- **Standard ledger (commercial balance sheet): EUR 1,000** Carrying amount commercial balance sheet
- **Standard ledger (tax balance sheet): EUR 0** Carrying amount tax balance sheet

The deviating posting in the tax balance sheet can be represented either by using a **ledger group** that disregards a posting in the standard ledger (tax balance sheet) or by reversing the posting made in the standard ledger (commercial balance sheet).

---

In addition to the option of reconciling differences between accounting principles using a parallel standard ledger, you can also use an **extension ledger**.

Compared to the standard ledger, an extension ledger records the variances between different accounting principles as **delta postings** to a standard ledger.

In this case, the representation of a different accounting principle results from the joint consideration of the **standard** and **extension ledger**.

Delta postings in the extension ledger are always made at the **account level**.

#### Example

Let’s consider an example of using extension ledgers.

In the commercial balance sheet, a provision of **EUR 1,000** is recognized. In the tax balance sheet, this provision may not be recognized; the carrying amount in the tax balance sheet is **EUR 0**.

The resulting differences between the commercial balance sheet and the tax balance sheet are converted using a delta posting in the extension ledger:

- **Standard ledger (commercial balance sheet): EUR 1,000** Carrying amount commercial balance sheet
- **Extension ledger: -1,000 EUR** delta posting
- **Tax balance sheet: EUR 0** Carrying amount tax balance sheet

An extension ledger is always assigned to a **base ledger** and inherits all posting documents of the standard ledger for reporting.

You can assign any number of extension ledgers to each standard ledger.

Postings explicitly posted to an extension ledger are visible in this extension ledger, but **not in the underlying base ledger**.

## 7.2 Organizational Structures

You can use the **extended ledger functions** in **SAP S/4HANA** as an **integrated ledger concept** to reflect the **tax balance sheet** across the **tax lifecycle**. We recommend a **1+2 approach**, which combines:

- A **standard ledger** for preparing the tax balance sheet in the consolidated financial statements
- An **extension ledger** for the status of the tax balance sheet at the time of the tax returns
- Another **extension ledger** for the status of the tax balance sheet at the time of the tax audit (tax auditor’s balance sheet)

(see Figure 7.4).

This approach allows you to reflect the different phases of the **tax lifecycle** separately.

### Set Up the Tax Ledger

In this section, you’ll learn which settings to make in **Customizing** to reflect the ledger approach in **SAP S/4HANA**.

You can access the settings in Customizing via **Transaction SPRO** and menu path:

```
Financial Accounting • Financial Accounting Global Settings • Ledgers • Ledger • Define Settings for Ledgers and Currency Types
```

You'll get an overview of the **standard** and **extension ledgers** already created in the system and can extend them for your purposes.

As shown in Figure 7.5, various ledgers have already been created in Customizing. For example:

- **Standard ledger 0L** defined as the **leading ledger** covers the **IFRS accounting standard**
- **Standard ledger 2L** reflects **local accounting standards**

To create a new ledger, click **New Entries** in the header and assign a ledger and other relevant attributes.

#### Figure 7.4 1+2 Approach for Reflecting the Tax Balance Sheet in SAP

_Fiscal Year 202X - Tax Reporting_  
_Fiscal Year 202X - Tax Return_  
_Fiscal Year 202X - Tax Audit_

| App             | Tax Ledger Workflow | Standard Ledger | Extension Ledger |
|-----------------|---------------------|-----------------|------------------|
| IFRS            |                     | X               | X                |
| Local GAAP      |                     | X               | X                |
| Tax             |                     | X               | X                |

- Fiscal Year
- Carryforward Tax Postings
- Parallel Accounting
- Tax Ledger Hub
- Delta Posting
- Audit-to-Return
- Return to Provision

---

#### Figure 7.5 Overview of Created Ledgers

As illustrated, the **1+2 approach**—combining a **tax ledger as a standard ledger** with two **extension ledgers** for tax purposes (e.g., standard ledger **TX** and extension ledgers **TE** and **TL**)—is also preconfigured in the SAP standard system.

### Deactivate Ledgers

Ledger settings are defined **per client** and initially apply to all company codes.

If you do not want to use the defined ledgers for all company codes, you must **deactivate a ledger** for specific company codes in Customizing via:

```
Financial Accounting • Financial Accounting Global Settings • Ledgers • Ledger
```

To deactivate a ledger:

1. Choose the relevant ledger by determining the work area in the popup window
2. Select the specific company code (**CoCd**)
3. Enter the fiscal year period the deactivation should apply to in the **From FYear** and **To FYear** fields

#### Figure 7.6 Deactivation of Ledgers per Company Code

---

### Ledger Groups / Document Entry

To make specific postings to the created ledgers, **ledger groups** with matching names are created automatically for each ledger.

When posting in **SAP S/4HANA**, you can select a ledger group for each posting. The posting will then only be made to the ledgers covered by that ledger group.

As shown in Figure 7.7, ledger groups exist for the tax ledgers defined:

- **TE** for Tax Return 1
- **TL** for Tax Audit 1
- **TX** for Tax Ledger Reporting

#### Figure 7.7 Overview of Created Ledger Groups

---

#### Example

- Posting a difference between the **commercial** and **tax balance sheet** at the time of **financial statement preparation** can be done by selecting the **TX ledger**.
- To enter a **delta posting** due to a **return-to-provision adjustment**, use ledger group **TE**.
- For a delta posting due to an **audit-to-return adjustment**, use ledger group **TL**.

When posting general ledger account entries via **Transaction FB50L**, you can enter the desired ledger group (**Ledger Grp**) in the upper-left corner (e.g., **TX**). If no ledger group is selected, the posting applies to **all defined parallel ledgers**.

#### Figure 7.8 Selection of Ledger Group in Transaction FB50L

---

### Fiscal Year Variant

A common question is whether **tax ledgers can have their own fiscal year and posting period variants**.

- The **fiscal year variant** determines if the fiscal year corresponds to the **calendar year**.
- It defines the **number of posting** and **special periods** for the fiscal year.

You can view fiscal year variants in Customizing via:

```
Financial Accounting • Financial Accounting Global Settings • Ledgers • Fiscal Year and Posting Periods • Maintain Fiscal Year Variant
```

To create a new fiscal year variant:

- Click **New Entries**
- Assign a **fiscal year variant (FV)** and relevant attributes such as **calendar year**, number of **postings**, or **special periods**

The fiscal year variant is always determined **per company code**.

You can view which fiscal year variant is assigned to a company code via:

```
Financial Accounting • Financial Accounting Global Settings • Ledgers • Fiscal Year and Posting Periods • Assign Company Code to a Fiscal Year Variant
```

For example, fiscal year variant **K4** (fiscal year same as calendar year with 12 posting periods and 4 special periods) is assigned to **Company Code 1010** (see Figure 7.10).

---

#### Figure 7.9 Overview of Created Fiscal Year Variants

#### Figure 7.10 Assigning a Fiscal Year Variant to a Company Code

An **extension ledger** always inherits the **fiscal year variant** of the underlying **standard ledger**.

In this case, the fiscal year variant of extension ledgers **TE** and **TL** matches the fiscal year variant of standard ledger **TX**.

Example shown for ledger **TL** in Figure 7.11 where the **Fiscal Year Variant** field automatically shows **K4**.

#### Figure 7.11 Inheritance of the Fiscal Year Variant to the Extension Ledger

---

### Posting Period Variant

Unlike fiscal year variants, the **posting period variant** can be defined **flexibly**.

- The posting period variant determines if you can **open and close a ledger independently** from other ledgers for accounting purposes.
- This allows postings to extension ledgers even if the tax general ledger is closed.

As shown in Figure 7.12, the posting period variant for extension ledger **TL** is set to **Tax Reporting**.

#### Figure 7.12 Selection of Posting Period Variant

You can post to created tax ledgers as long as the posting period remains **open** for that ledger.

---

### Balance Carryforward

The **balance carryforward** to a new posting period is linked to the posting period variant.

- Balance carryforward can be executed **independently** for each ledger, carrying forward balances separately.

To set balance carryforward:

1. Run **Transaction FAGLGVTR**
2. Select parameters such as **Ledger** and **Company Code**
3. Click **Execute**

#### Figure 7.13 Balance Carryforward per Ledger

---

### Tax Ledgers in Regular Business Operations

The **ledger functionality** in **SAP S/4HANA** offers companies a new starting point for **ERP-integrated tax accounting**, establishing a **single source of truth** for all accounting standards.

To maximize the transformation project, consider further automation for **efficiency gains**.

A common enhancement is a **custom-built tax ledger application** featuring:

- Integration into **SAP S/4HANA**
- Rule-based determination of **statutory-to-tax adjustments**
- Posting statutory-to-tax adjustments to the tax ledger at the push of a button
- Determination of statutory-to-tax adjustments per **account** or **line item**
- Predefinable templates for statutory-to-tax adjustments
- Freely definable **tax lifecycle phases**
- Lifecycle phase and fiscal year carryforward of statutory-to-tax rules
- Workflow-based **approval process** and status overview
- Document and notification service
- Responsive SAP **Fiori-based** web design

---

### Benefits of a Custom Tax Ledger Application

- **Single source of truth** for the tax balance sheet
- Real-time availability of **tax balance sheet data**
- Accelerated preparation of the tax balance sheet through **automation**
- Reduced interface to peripheral tax balance sheet solutions
- Relief of finance and tax teams from routine tasks
- Improved governance between **finance** and **tax**
- Increased **data quality** through IT-sided mapping of the tax balance sheet
- Operationalization of **tax compliance** via integrated approval workflows
- Enhanced documentation through central **document storage**
- Unified **SAP user experience (UX)**

---

### Demonstration of a Custom Application

This custom application in **SAP Fiori** focuses on three key capabilities:

- Setting up a **full direct tax lifecycle management system** covering major lifecycle phases such as **tax reporting**, **tax return preparation**, and **tax audit management**
- Creating **rule-based statutory-to-tax adjustments** based on statutory GAAP values
- Enabling **automated posting** of all statutory-to-tax adjustments at the push of a button, including workflow-based approval processes

This application provides the tax function a powerful tool to:

- Develop the tax ledger concept toward a **highly automated process**
- Create a **single source of truth** for all parallel accounting standards

---

### Lifecycle Phases Setup

Before creating statutory-to-tax adjustments, it's required to create different **lifecycle phases** covering the full tax lifecycle.

Use the **Lifecycle Maintenance tile** in the **Tax Ledger Hub app** (see Figure 7.14) to manage lifecycle phases.

#### Figure 7.14 Tax Ledger Hub App: Tiles

Figure 7.15 shows how to set up a full tax lifecycle for capturing adjustments.

You can add new lifecycle phases easily by clicking the button in the lower-right corner.

## Figure 7.15 Direct Tax Lifecycle Management

Once you’ve finalized this preparatory work, you can start to create **rule-based statutory-to-tax adjustments** by uploading posted local and tax GAAP values. Navigate to the **App tile** in the **Tax Ledger Hub app** (refer to Figure 7.14), and select the relevant **Company Code**, **Fiscal Year**, and created **tax Lifecycle Phase** (see Figure 7.16).

Click the **Go** button, and you’ll see real-time **Local GAAP** and **Tax GAAP** numbers as posted in your ledgers in **SAP S/4HANA**.

### Figure 7.16 Posted Local GAAP and Tax GAAP Values at a Glance

In the next step, you can select **balance sheet items** that are subject to statutory-to-tax adjustments and create new adjustments (see Figure 7.17). To do so, just click on one line item (account line), and then if a statutory-to-tax adjustment needs to be created, click on the **Add Deviations** button on the right-hand side.

### Figure 7.17 Creation of New Statutory-to-Tax Adjustments

When creating a new statutory-to-tax adjustment, you can decide whether you want to define the adjustment on the **account** or **line-item level** by turning the **Account Split** button to **ON** or **OFF** (see Figure 7.18).

- If you choose **OFF**, the entire account balance will be taken into account for the statutory-to-tax adjustment.
- If you choose **ON**, you’re able to select the checkboxes of specific transactions posted to this account and then proceed with the next step by clicking **Next Step**.

### Figure 7.18 Statutory-to-Tax Adjustments on the Account or Line Item Level

Once you’ve selected the relevant items for your statutory-to-tax adjustment, you’re able to create a **rule** to automate the determination of the respective **tax GAAP values** based on available templates.

You can either choose between:

- **Predefined rules** provided with a standard content package  
- Define your **own templates** based on your specific needs

© 2022 by Rheinwerk Publishing Inc., Boston (MA)

---

## 7.2 Organizational Structures

Tax rules that can be defined, for example, are:

- **Depreciation plans**  
- Correction of **valuation allowances** in percent for assets  
- **Discounting of accruals or liabilities**  
- **Tax-specific valuation plans** for accruals and liabilities  
- Rules for **nonrecognition of assets and liabilities** for tax purposes

For our example, navigate to the **Choose Template** step, as shown in Figure 7.19, where you can see the catalog of available templates for tax rules.

### Figure 7.19 Application of Tax Rules for Statutory-to-Tax Adjustments

Once you’ve created the statutory-to-tax adjustments, you can directly see the **profit impact on your tax base** by navigating to the **Review Template** step (see Figure 7.20).

### Figure 7.20 Profit Impact of Statutory-to-Tax Adjustments at a Glance

In the last step, the user (**approver**) can select the created statutory-to-tax adjustments and submit these for posting at the click of a button to the underlying ledger.

To do so, the user opens the **Approve and Post** tile within the **Tax Ledger Hub app** to get to the page shown in Figure 7.21.

In the case at hand, all postings for the tax reporting lifecycle phase will be made to an underlying **parallel tax ledger**.

Postings can alternatively be made to, for example, extension ledgers for the tax return or tax audit lifecycle phases depending on the underlying ledger concept. The postings can be triggered by:

- Checking items in the list of available postings  
- Selecting **Simulate** (optional step)  
- Selecting **Posting**

### Figure 7.21 Posting of Statutory-to-Tax Adjustments at the Click of a Button

To keep the work across all lifecycle phases and fiscal years as efficient as possible, all statutory-to-tax adjustments can be **carried forward** from phase to phase or to the next fiscal year.

As you can see, the standard ledger functionality in **SAP S/4HANA** can be enhanced to a significant extent if you make use of additional capabilities such as **SAP Fiori**. We strongly recommend that you make use of these capabilities to leverage the full potential of SAP S/4HANA for your tax function.

---

## 7.2.2 Company Code

In **SAP S/4HANA**, **company codes** represent **organizational units of accounting** that subdivide the company from a **financial accounting perspective**. You can use company codes to reflect several **self-contained accounting records** at the same time.

A separation is usually based on **commercial or tax considerations**, but it can also take into account other reporting requirements. By default, each company code corresponds to a **legal entity** in the company.

You can make **company code settings** in SAP S/4HANA using the Customizing menu path:  
**Enterprise Structure • Definition • Financial Accounting • Edit, Copy, Delete, Check Company Code**.

The company code exists as an organizational unit of accounting alongside other organizational units and accounting objects that support different objectives in SAP S/4HANA.

For the tax function, the implementation of SAP S/4HANA results in the task of analyzing **tax requirements** to be reflected via appropriate organizational units and accounting objects.

In particular, you need to assess whether the existing implementation of company codes already meets tax requirements with regard to reporting or needs to be expanded in the future.

The following **organizational units or accounting objects** are available:

- **Company code**  
- **Profit center**  
- **Plants abroad**  
- **Cost centers**  
- **Internal orders**  
- **Projects** (work breakdown structure [WBS] elements)

### Functions of Organizational Units/Accounting Objects

In particular, include the following functions in your considerations, which individually or in combination determine the requirements for the respective organizational unit:

- Preparation of **balance sheet** and **profit and loss (P&L) account**  
- Allocate **revenues and costs**  
- Apply **parallel accounting**  
- Create **asset accounting**  
- Assign **individual roles and permissions**  
- Define your own **currency types**  
- Deposit **tax master/tax registration data**  
- Implement **tax reporting requirements** (e.g., SAF-T, e-balance sheet)

From a tax perspective, the following issues arise in the area of **direct taxes** that require a decision to be made when choosing the right organizational unit:

#### Legal entities as tax subjects

As a rule, **tax law**, for example, for **corporate income tax** purposes, bases taxation on **legal entities** as tax subjects (e.g., limited liability companies).

Because configuration in SAP S/4HANA provides for the creation of company codes as standard for this case anyway, this doesn’t result in any special tax requirement.

As a recommendation, it can be stated that from a direct tax perspective, a **company code should be created for each legal entity**.

#### Permanent establishments abroad of domestic legal entities

The situation is different if legally dependent organizational units are also taken into account for tax purposes. In practice, this is usually the case with **permanent establishments abroad**.

From a tax perspective, the **result of the permanent establishment abroad** is exempt from domestic taxation by applying the **exemption method**. In contrast, the results of the permanent establishment abroad are subject to taxation, and a **tax return** must generally be filed for the permanent establishment abroad.

To fulfill the tax filing obligations, a **separate P&L calculation** must be prepared for the permanent establishment. On one hand, this serves to distinguish the expenses and income attributable to the permanent establishment from the domestic determination of profits; on the other hand, separate accounting rules may have to be applied.

If the previously mentioned functions are taken as a basis, only one company code can fulfill the tax requirements for organizational units because a **separate balance sheet and income statement** must be prepared for the determination of permanent establishment profits.

In addition, there may be a need to resort to **parallel accounting**, including separate valuation areas in asset accounting, due to differing accounting regulations under domestic and foreign tax law.

#### Special features for partnerships

Insofar as **partnerships** are part of the company, requirements may arise from the **transparent taxation of partnerships**. This can, for example, lead to:

- Special and supplementary **balance sheets of the partners**  
- Additional **tax documentation and filing obligations**

As you can see, the decision should generally be made in favor of creating a **separate company code**, regardless of the circumstances.

Even if existing filing and reporting obligations can currently be reflected via alternative organizational units, the creation of a company code is more **future-proof** in any case. 

At the same time, you still have the option of combining the created company code with other organizational units or accounting objects to derive additional reporting options, for example, in the area of **transfer pricing**.

### Transaction for Displaying Created Company Codes

If you want to get an overview of the company codes created in Customizing, you can do this by using **Transaction SE16** in the SAP GUI and then selecting table **T001**.

---

## 7.2.3 Chart of Accounts

As already explained, the requirements of companies for the design of a **uniform (consolidated) chart of accounts** are complex.

In addition to the requirements of **parallel accounting** in the consolidated and separate financial statements under commercial law, country-specific features in the respective jurisdictions must be taken into account for local accounting.

In addition, the **tax function** needs to reflect **tax requirements**, especially in the area of **direct taxes**, in a country-specific manner and as granularly as possible in the chart of accounts.

The reason for this is that the chart of accounts and the account balances behind it provide the **numerical framework** for the existing filing and reporting obligations to a considerable extent.

All in all, these requirements lead to a considerable **administrative effort** with regard to the maintenance of the chart of accounts, which understandably should be avoided in practice on a regular basis by specifying a **lean chart of accounts**.

For the tax function in SAP S/4HANA, you must, on one hand, take into account the **guardrails** for the chart of accounts defined in the overall project, and on the other hand, you should take advantage of the tax opportunities of a **granular chart of accounts** with regard to your processes as much as possible.

If we look in detail at the requirements for the area of direct taxes, the following questions, for example, arise in practice that you must consider when designing your chart of accounts:

- What are the **commercial law obligations** in the consolidated and individual financial statements that require a more granular design of the chart of accounts?  
- What are the **tax obligations** that must be taken into account when implementing the chart of accounts?  
- What are the requirements with regard to the desired **automation of tax reporting and filing processes**?

During the project, it will likely turn out that the need to create an account often doesn’t only arise from a single requirement, but from a combination of these areas.

There is regularly a need for discussion in the overall project in cases where the creation of a tax-relevant account is solely based on the objective of **tax-efficient processes**.

In this case, the need for creating additional accounts must be justified in more detail by means of a **business case**.

## Documentation of Tax Requirements in the Chart of Accounts

In practice, it’s advisable to mark the **tax requirements** that arise for the chart of accounts in separate columns in the project template and to explain them via comments. Given the large number of requirements, it will then be easier for you to understand the reason for creating a **tax-relevant account** retrospectively.

Before you begin with the actual design of the chart of accounts from a **tax perspective**, it’s important to further narrow the scope of accounts to be evaluated by the **tax function**. In our experience, the conception of the chart of accounts from a **direct tax** point of view includes two account areas in the balance sheet and the income statement: **tax position accounts** and **tax-sensitized accounts**.

### Tax Position Accounts

**Tax position accounts** form the actual tax accounts in the chart of accounts and record the accounting of **current taxes**, including **tax fringe benefits** and **deferred taxes** in the balance sheet and the income statement.

---

## Consideration of Additional Tax Requirements

Under **international accounting standards**, additional tax position accounts may be required, but these won’t be discussed in detail here. These serve, for example, to show:

- The maturity of **deferred taxes**
- The presentation of deferred taxes in **equity**
- The distinction between **tax provisions** and **tax liabilities**

To automate monitoring of tax movements in real time (see Section 7.5), it’s important that you use other available **account assignment features** in **SAP S/4HANA** in addition to the chart of accounts. This relates in particular to the use of **movement types**, which allows you to clearly differentiate between account movements, for example, in:

- Additions
- Reversals
- Consumption

In this way, you can significantly reduce the effort required for the manual creation of a **tax receivable** and a **tax accrual statement**.

### Tax-Sensitized Accounts

**Tax-sensitized accounts** carry items in the balance sheet and income statement that are:

- Assessed differently under **commercial** and **tax law**
- Have to meet additional **tax reporting requirements**

Important accounts with relevance for tax-sensitization include:

#### Balance Sheet Accounts

- **Financial assets** and related receivables that can be sensitized by differentiating between **legal form**, **participation quota**, and **foreign/domestic investments** to determine tax-exempt sale of assets or impairments to be disallowed for tax purposes.
- **Equity accounts** that can be sensitized through the use of **movement types** and **partner IDs** to manage tax equity accounts more efficiently, e.g., to track movements in tax equity automatically by shareholder.
- **Liabilities and accruals** that can be structured by **item**, **maturity**, **movement**, and **intercompany relevance**.

#### P&L Accounts

- Income from **revaluation** or **disposal of investments** that can be sensitized by differentiating between **legal form**, **foreign/domestic income**, and **participation quota** to track tax-exempt income.
- Other income from investments to be differentiated by **type of payment**, **legal form**, **foreign/domestic income**, and **participation quota** to structure tax-exempt income.
- Income and expenses from realized and unrealized **currency gains/losses** that may be subject to specific tax provisions.
- Income and expenses from **special tax reserves** relevant only for tax accounting purposes.
- **Nondeductible business expenses** by type of expense; thresholds; availability of tax certificates/vouchers; intercompany relevance such as **entertainment expenses**, **gifts**, or **donations**; **supervisory board compensation**; and rental, lease, and license expenses.

---

### Additional Account Assignment Features

In particular in the area of tax accounts, the concept of the chart of accounts should be supplemented with additional **account assignment features**. This concerns, for example, the use of **transaction types** for the presentation of the:

- Fixed asset schedule
- Tax movement schedule
- Movements in equity

If the chart of accounts doesn’t provide all relevant **taxation characteristics** as information, you can alternatively consider using the concept of **tax tagging** for the area of direct taxes. This may be due to either:

- The limitation of the chart of accounts by project specifications
- The design via accounts can’t technically provide all the required taxation attributes

Tax tagging will be discussed in more detail in Section 7.4.

---

## Choice of Tax Account Descriptions

Because the uniform (group) chart of accounts is used **globally**, you should ensure that the **tax account descriptions** are generally understandable and applicable. A specific account description should be applied only if **country-specific requirements** can be reflected via additional **subaccounts**.

### Examples

- An account description **“nontax-deductible entertainment expenses”** is generally more appropriate than **“entertainment expenses under Section 4(5)(2) of the EStG.”**
- An account description **“nontax-deductible interest”** is preferable to describing it as **“nontax deductible interest under Section 233a AO.”**

---

### Chart of Accounts Overview

Via **Transaction SE16** and the subsequent selection of table **T004**, you can get an overview of which charts of accounts are currently set up in the system (see Figure 7.22).

---

To analyze which accounts are currently already provided in a chart of accounts, an overview is available via **Transaction SE16N** in connection with the selection of table **SKA1** (see Figure 7.23).

---

## 7.2.4 Withholding Tax

For various business transactions, **globally operating corporations** are taxed at the **service recipient’s place of business**, i.e., subject to **limited corporate taxation**. In this case, the respective **service recipient** is obliged to withhold a portion of the **remuneration** agreed on with the service provider to pay it directly to the **tax authorities**.

Typical cases for withholding tax include — among others — license payments, construction services, or sporting, artistic, and similar performances.

Due to these obligations, there are numerous implications at the level of:

- The service recipient
- The service provider

that need to be reflected in **SAP S/4HANA**. This includes the entire **end-to-end tax scenario** from **data collection** and **tax determination** to **reporting** and **filing**.

---

### Focus on Input Side of WHT

The main activities processed in the area of **WHT** on the input side (essentially in the **procure-to-pay** scenario) are, for example:

- Assessment of facts for any potential WHT obligations
- Derivation of the correct tax determination in SAP S/4HANA based on available **WHT types** and **WHT codes**
- Extraction of WHT-relevant data in SAP S/4HANA
- Preparation, checking, and finalization of **WHT filings** for transmission to tax authorities
- Issuance of **tax certificates** to the vendor

---

### WHT Functionality in SAP S/4HANA

SAP S/4HANA offers two alternative ways to support the **WHT process**:

- **Basic WHT function**
- **Extended WHT function**

Which option should be implemented depends on the complexity of the WHT case, determined by the interaction of:

- The **tax base**
- **Taxable portion** of the remuneration
- Prescribed **tax calculation methods**
- Applicable **WHT rate**
- **WHT exemption certificates**

It must also be decided whether both the input and output sides will be covered by the WHT function.

- The **basic WHT function** supports management of WHT at the **service recipient** (input side).
- The **extended WHT function** provides the possibility to manage WHT at the **service provider** (output side).

Step-by-step configuration for WHT (basic and extended) is covered in Chapter 3, Section 3.1.1.

---

## 7.3 Master Data

In addition to the setup of **organizational structures**, **master data governance** is highly relevant for the area of **direct taxes**. It represents basic information created in the system that can be accessed centrally when processing **business transactions**.

Besides **tax determination**, master data enables the accurate and uniform handling of **tax cases** in the area of **direct taxes**, divided thematically into:

- **Business partner**
- **General ledger account**
- **Asset master data**

Each area will be explored in the following sections.

---

## 7.3.1 Business Partner

**Business partner master data** comprises all basic information relating to the **debtors/creditors** of your company. Although these are of minor importance for the **direct tax** area as a whole, they are highly relevant for **WHT determination**.

Important information such as **WHT types**, **WHT codes**, and **WHT exemptions** is assigned to the business partners (see Figure 7.25). Correct system-supported tax determination can only be ensured if the **master data** is carefully maintained and updated.

---

### Risks of Incorrect Master Data

Incorrect **personal account master data** can lead to:

- A WHT case being incorrectly posted by the system
- A WHT deduction being made in the wrong amount

Given the considerable amount of master data in the company, neglecting **master data governance** can have material consequences.

---

### Monitoring Data Quality for Person Account Master Data

To enable you to monitor data quality in the area of **business partner master data**, it is recommended to:

- Establish a **detective check** (see Section 7.5)
- Continuously monitor the master data as part of your **tax control framework**

Vendor: Withholding Tax data is detailed in Chapter 3, Section 3.2.1.

---

## 7.3.2 General Ledger Account

You can use **general ledger account master data** to include important information when posting **tax-relevant transactions** to general ledger accounts.

An important starting point for this is the definition of **field status variants** and **field status groups**:

- These define which input fields are **visible** when performing document entries
- Determine whether the filling is **optional** or **mandatory**

Customizing settings and created field status variants can be accessed in the IMG via:

`Financial Accounting • Financial Accounting Global Settings • Ledgers • Fields • Define Field Status Variants`  
(see Figure 7.26)

By selecting a **field status variant (FStV)** and clicking on **Field status groups**, you can see the list of available field status groups (see Figure 7.27).

For the area of **direct taxes**, you could define separate field status groups, e.g., **TX01** for your **tax receivables** and **tax accruals**.

This enables users to add further information to a posting via **mandatory fields**, which is required for tax reporting but can’t be provided via the definition of the chart of accounts.

## 7 Direct Taxes in SAP S/4HANA

### 7.28 Field Status Groups and Subgroups

Subgroups for the **field status group**, where you can make settings for individual input fields during document entry, can be accessed by selecting the **field status group** (e.g., **TX01**) and double-clicking on the line item. The subgroups are shown in Figure 7.28.

For example, you can specify:
- **Cost centers** in the **Additional account assignments subgroup**
- **Tax codes** in the **Taxes subgroup**
- **Transaction types** in the **Consolidation subgroup** (not shown) as account assignment attributes

One attribute to always include in your **general ledger account master data** when implementing SAP S/4HANA in the area of **direct taxes** is **transaction types**. 

You can view available transaction types in the IMG under:
- **Financial Accounting** • **Financial Accounting Global Settings** • **Books** • **Fields** • **Standard Fields** • **Maintain Consolidation Transaction Types**

**Transaction types** allow the option of displaying horizontal changes in balance sheet items such as **additions, reversals, and consumption**. In **direct taxes**, this supports the automated derivation of a **tax movement schedule** or presentation of changes in **tax equity** in SAP S/4HANA.

SAP provides many predefined **transaction types** in the standard system (see Figure 7.29), which you can customize by adding your own transaction types via the **New Entries** button.

### Customer-Specific Fields in General Ledger Account Master Data

Besides **field status variants**, you can use **customer-specific fields** in the general ledger account master data area. For example, these fields can **use a tax code for direct taxes** to implement the concept of **tax tagging** (see Figure 7.33 later in this chapter).

---

### 7.3.3 Asset Master Data

The management of **asset master data** significantly impacts **direct taxes**. Asset master data in SAP S/4HANA can reflect various accounting regulations, including **tax accounting**. 

The basis for managing asset master data is the **chart of depreciation**, defined on a **country-specific level**, combining various **parallel valuation areas** as a single unit. 

Define a chart of depreciation using the path:
- **Financial Accounting** • **Asset Accounting** • **General Valuation** • **Depreciation Areas** • **Define Depreciation Areas**

As seen in Figure 7.30, a **tax depreciation area** (e.g., **10 [Tax – Local Currency]** and **15 [Tax – Group Currency]**) can be part of the chart.

The **tax valuation area** carries valuation parameters needed for fixed asset valuation according to tax regulations. This automates **asset accounting** and the preparation of the **tax balance sheet**.

You can link the **tax depreciation area** to the **tax ledger** you defined by double-clicking the tax depreciation area in the list and selecting the **Target Ledger Group** (e.g., **TX**) in the **Define Depreciation Areas** section (Figure 7.31).

We recommend integrating **asset accounting conception from a tax perspective** from the start to fully align **tax valuation** and **tax balance sheet representation**.

---

## 7.4 Tax Tagging

Business transactions in SAP often lack information required for **tax reporting** and **compliance**. Even if transactions carry some tax-related data, businesses struggle with appropriate **tax determination** due to multiple potential tax effects.

**Tax tagging** supports businesses by guiding and enabling tax decisions and additional data capture in SAP. Tax tagging uses various technical enhancements, flexible to specific use cases.

### Tax Tagging Workflow (Figure 7.32)

1. A new **tax-relevant business transaction** occurs in SAP S/4HANA (purchase order, incoming invoice, or posting). The system checks if additional tax information (**tax tag**) needs to be added based on validations.

2. If tax-relevant, a **user exit** triggers a **tax decision engine**, which returns the required tax tag via a **guided or automated Decision Model and Notation (DMN)** process.

3. The tax tag is routed back and stored in the relevant SAP S/4HANA transaction (e.g., via a **customer-specific field**) or stored externally referenced by document number.

4. Based on the tags and other data, **journal entries** are evaluated, and relevant **tax reports** processing the tax tags are built.

### Examples of Tax Tagging Support

- In Transaction **FB50L**, tax tagging standardizes postings to **tax accounts** like **tax receivables**, **tax accruals and liabilities**, and **tax expense**. Additional columns capture details like **Tax period**, **Tax author**, **Tax type**, and **Tax PL mov (tax P&L movement) type** (Figure 7.33). This is based on a user exit not available in standard SAP.

- The additional information can be used to automatically derive **tax movement schedules**, **tax expense**, or **tax cashflow reports**.

### Withholding Tax (WHT) Tax Tagging

Tax tagging detects **WHT-relevant transactions** and triggers a guided process for **WHT tax consequences** in Transaction **FB60** (Figures 7.34 & 7.35).

- A **user exit (popup window)** appears if a foreign vendor (without WHT maintained) and specific material are selected.
- The user can **Post Anyway** (not recommended) or **Open Tool** to access the decision engine.
- The decision engine supports **WHT determination** by a guided or automated process.
- The user selects the appropriate **WHT type and code** and receives instructions to correct WHT settings in SAP S/4HANA.

This example enriches transactions with tax info and supports the **tax control framework** by detecting tax-relevant transactions via **ERP-integrated controls**.

---

## 7.5 Direct Tax Data Analytics and Monitoring

The quality of **tax-relevant data** in SAP S/4HANA depends heavily on **organizational structures** and **business processes** behind data collection, including **master data** and **transactional data**.

Due to complex processes, user interaction, and changing tax laws, ensuring perfect **data quality** for tax compliance at all times is nearly impossible.

You must establish a **tax control framework** to ensure correct fulfillment of **tax filing** and **reporting obligations** and reduce risks of tax evasion or fraud.

Key components:
- Setup of **guidelines**
- Documentation of **tax processes** and **tax risks**
- Introduction of **process- and system-integrated controls**

### Analysis and Monitoring in Direct Taxes

This phase supports you in:

#### Master Data Screening

Ensures the completeness and accuracy of **tax-relevant master data**.

- Poorly maintained or incorrect master data can cause **incorrect tax determination**.

**Example:**  
A company paying royalties abroad has wrongly maintained WHT types and codes in **vendor master data**. Payments thus escape proper WHT classification or rates, potentially causing liability for WHT shortfalls upon tax audits.

Master data screening can identify business partners missing relevant **WHT types and codes**, improving data quality.

#### Transaction Data Screening or Account Screening

Analyzes if **tax-relevant facts** are completely and correctly recorded in the appropriate accounts.

- Incorrect booking may arise from unrecognized facts, wrong assessment, or incorrect accounts.
- Incorrectly recorded transactions can lead to omission of **tax corrections** and understatement of the **tax base**.

## Example

A company thanks a business partner with monthly gifts of **EUR 30**, which are erroneously recorded in the account **“Gifts to business partners up to EUR 35 per fiscal year.”** In the tax return, there is no addition of the account balance totaling **EUR 360**, because only the account **“Gifts to business partners in excess of EUR 35 per fiscal year”** is taken into account in the context of off-balance sheet additions. The company’s tax department relies on the correct posting of the facts in the accounting system.

Transaction data screening reveals that the total amount of gifts to a business partner exceeds the permissible sum of **EUR 35**. Accordingly, the incorrectly recorded amount can still be corrected in the tax return.

## Analytical Review

The purpose of an **analytical review** is to uncover **tax-relevant transactions** by means of analytical test routines or to derive technical conclusions for tax reporting. Corresponding review routines are based on, among other things, **inter-periodic correlations**, **changes to master data**, or the targeted analysis of **account movements**.

Failure to recognize tax-relevant facts may result in them being disregarded when preparing tax returns or in tax benefits not being appropriately used.

### Example

A company realizes a **tax loss of EUR 100** in the current fiscal year and a **tax profit of EUR 100** in the previous fiscal year. When preparing the tax return for the current fiscal year, a **tax loss carryforward of EUR 100** is taken into account. The tax return is to be submitted to the tax office after a rough review by the head of the tax department.

Through an analytical check routine that compares the tax result determined via the use of the **tax ledger** over the past few fiscal years, it becomes apparent that a **tax loss carryback** would have been more advantageous. By identifying the error, the tax return can still be corrected in time before submission to the tax office.

As you can see, implementing a solution that enables the analysis of tax-relevant data according to the aforementioned criteria can create a significant value proposition for ensuring **tax compliance** in the company.

## Tax Compliance and SAP S/4HANA Embedded Analytics

In this section, we’ll present you with **SAP Tax Compliance** and **SAP S/4HANA embedded analytics**, two alternatives with which you can effectively implement tax data analytics and monitoring in **SAP S/4HANA**.

### Core Data Services

**Core data services (CDS)** in SAP S/4HANA underlie both solutions as an approach to data modeling. Check routines that you want to define and use in SAP Tax Compliance and SAP embedded analytics must be mapped using **CDS**.

**CDS** has gained a lot of importance with the introduction of SAP S/4HANA. Compared to previously existing concepts for data modeling, data models with **CDS** can be defined directly in the database (**SAP HANA**) located in the core. This leads to a significant performance improvement in data analysis.

For detailed information on **CDS in SAP S/4HANA**, including the basics of CDS data modeling and modeling analytical and transactional applications, see _Core Data Services for ABAP_ by Renzo Colle, Ralf Dentzer, and Jan Hrastnik (SAP PRESS, 2022, www.sap-press.com/5294).

### 7.5.1 SAP Tax Compliance

**SAP Tax Compliance** provides you with a standard solution for **tax data analysis** that you can license as an extension in SAP S/4HANA. In addition to the actual execution of compliance checks using predefined check routines, SAP Tax Compliance also provides functions for investigating identified anomalies.

Using predefined tasks, errors can be tracked in a **system-supported** and **process-integrated** manner and remedied in SAP S/4HANA.

#### Check Routines in SAP Tax Compliance

You have the option of extending the existing check routines in SAP Tax Compliance via **tax type-related content packages** or the definition of your own views.

SAP Tax Compliance is thus available not only for compliance checks in the area of **direct taxes** but also for carrying out audits in the area of **indirect tax**.

Before you can use SAP Tax Compliance via your **SAP Fiori interface**, you must first set up the solution in Customizing as part of an implementation project. You can access the settings in the **IMG** via the Customizing function **Set Up SAP Tax Compliance**.

In this section, we’ll focus on the user perspective. You can access the SAP Tax Compliance applications in your **SAP Fiori interface** via the **Detection** and **Investigation** areas. There you’ll find SAP Fiori apps that cover all important tasks in SAP Tax Compliance, as shown in Figure 7.36.

![Figure 7.36 Overview of SAP Tax Compliance](image-placeholder)

To get an overview of the check routines available in SAP Tax Compliance, first open the **Manage Compliance Checks** app. You can then add further compliance checks (check routines) and combine them into **compliance scenarios** that can be managed using the **Manage Compliance Scenarios** app and run using the **Run Compliance Scenario** app.

You can view the status of your compliance scenario runs using the **Monitor Compliance Scenario Runs** app and check your results using the **My Compliance Check Results** app.

For detailed information on monitoring with SAP Tax Compliance, see Chapter 10.

### 7.5.2 Embedded Analytics

Now that we’ve provided you with an initial overview of SAP Tax Compliance from a user perspective, we’ll introduce you to **SAP S/4HANA embedded analytics** as a possible alternative for performing tax analyses in the area of **direct taxes**.

With SAP S/4HANA embedded analytics, SAP offers a way to perform analyses of your tax data in SAP S/4HANA without the additional use of a **business intelligence (BI) solution**.

**SAP S/4HANA embedded analytics** is integrated as a component in your SAP S/4HANA system, making the additional licensing of a solution unnecessary.

Embedded analytics builds on **CDS** and uses the **virtual data model** in SAP S/4HANA. This enables **real-time tax data analytics** and doesn’t require data to be extracted for analytics purposes.

For using embedded analytics, in addition to standardized views based on CDS, extensive templates for analytics applications are available via the **SAP Fiori apps reference library** (see Figure 7.37), which you can easily adapt to your requirements (see http://sprs.co/v549503). At the same time, you can develop your own analytics apps based on **SAPUI5**.

![Figure 7.37 SAP Fiori Apps Reference Library](image-placeholder)

Embedded analytics is also interesting for analyses in the area of **direct taxes**. Defined check routines based on CDS views can be converted into meaningful lists or dashboard views without major development effort.

For a transaction data screening/account screening, for example, a dashboard might look like the one shown in Figure 7.38.

![Figure 7.38 Example of a Dashboard in Embedded Analytics](image-placeholder)

#### Further Reading

For more information on SAP S/4HANA embedded analytics, see _SAP S/4HANA Embedded Analytics_ by Jürgen Butsmann, Thomas Fleckenstein, and Anirban Kundu (SAP PRESS, 2021, www.sap-press.com/5226).

### 7.5.3 Comparative Review

Whether you’ll use **SAP Tax Compliance** or **SAP S/4HANA embedded analytics** as your solution of choice depends both on your expectations of the functions of the respective solution and on financial considerations.

Table 7.1 summarizes important similarities and differences between the two solutions.

| Features                     | Embedded Analytics | SAP Tax Compliance |
|------------------------------|--------------------|--------------------|
| **License costs**             | No                 | Yes                |
| **Content package**           | Yes                | Yes                |
| **Available on-premise**      | Yes                | Yes                |
| **CDS-based**                 | Yes                | Yes                |
| **Analyses in real time**     | Yes                | Yes                |
| **Flexible dashboards**       | Yes                | No                 |
| **Standardized task management** | No            | Yes                |
| **Workflow-based task processing** | No            | Yes                |

As you can see, the choice of a suitable analytics solution depends primarily on your personal preferences and, among other things, on the weighting of the aforementioned criteria.

#### Use of SAP Analytics Cloud in the Area of Analytics and Monitoring

In this section, we haven’t gone into detail about the use of **SAP Analytics Cloud** as a possible (visualization) alternative. The use of this solution can also be a valuable alternative.

This can be assessed, for example, on the basis of the criteria mentioned in Section 7.7.3.

## 7.6 Direct Tax Filing and Reporting

**Tax filing and reporting** is usually the most labor-intensive phase of the end-to-end tax scenario and encompasses the entire **tax lifecycle** in the area of **direct taxes**—from the preparation of tax reporting as part of **year-end closing activities** to the preparation of **corporate income tax returns** or tax filings in the area of **WHT** (withholding tax).

The actual reporting takes place outside the ERP system, so **SAP S/4HANA** primarily serves as a **data provider**.

In our experience, a very heterogeneous picture emerges with regard to the implemented tax applications in this area. In practice, a large number of historically grown solutions are generally used, which must be taken into account when implementing SAP S/4HANA.

These include, for example, **tax reporting solutions**, applications for the preparation of **tax returns**, **WHT filings**, or **tax audit management solutions**.

In the course of the introduction of SAP S/4HANA, the question arises as to whether the solutions used to date should continue to be used or whether a fundamental realignment of the system landscape should take place.

If the aim is to retain the existing system landscape, in addition to the tax requirements in SAP S/4HANA, further compelling questions arise with regard to the design of **tax data management** as an intermediary between the ERP system and the software solutions used. In addition, comprehensive **interface requirements** must be taken into account.

In any case, the project to implement SAP S/4HANA should be used to establish the ERP system as a **single source of truth for tax data** to reduce previously existing media disruptions and at the same time the number of interfaces to the most possible extent.

As an alternative to retaining the existing system landscape, the introduction of SAP S/4HANA offers interesting starting points for fundamentally rethinking **tax reporting** and integrating it into the SAP system landscape.

**SAP Profitability and Performance Management** and **SAP Document and Reporting Compliance** are the main solutions to consider in this area.

### Future Alignment of Filing and Reporting

Which strategy is ultimately pursued depends to a large extent on the **licensing costs** incurred by existing and future solutions, the **functional scope** of existing and alternative solutions, whether there is the possibility of a **module transfer** between the applications, or what effort can be expected in the future with regard to **tax data management** and the necessary **interfaces**.

**User acceptance** of existing and potential future solutions should not be underestimated as well.

In the following sections, we’ll show how you can use **SAP Profitability and Performance Management** as a solution for tax reporting and **SAP Document and Reporting Compliance** as a solution for tax filing.

Both solutions offer the flexibility to reflect different scenarios for reporting and to implement company-specific requirements.

As part of the SAP system landscape, you as a user benefit, for example, from a **uniform UX**, an existing **role and authorization concept**, a coordinated **IT infrastructure**, an existing **master data model**, and immediate access to tax data via **drilldown**.

## 7.6.1 SAP Profitability and Performance Management

**SAP Profitability and Performance Management** is an application based on **SAP HANA database technology** that enables near real-time **profitability and cost analysis**, as well as complex calculations and simulations. Assuming an available license and setup, you can access the application via the **SAP Fiori launchpad**.

- Example: Tax reporting implementation accessed via the **My Activities – Process Activities** tile.
- The application scenario is modeled as a **process** in SAP Profitability and Performance Management.
- Contains all essential elements familiar from classic tax reporting solutions.

You can access key elements by selecting the line item **Tax Calculation Process**.

In SAP Profitability and Performance Management, you can:

- Enter **master data** and maintain **tax rates**.
- Perform **account mapping**.
- Calculate **current** and **deferred taxes**.
- Document **tax loss carryforwards**.
- Reconcile **expected to actual tax expense**.

Within individual activities, users can:

- Switch between different **company codes**.
- Select **fiscal year** and **posting period**.
- Maintain data **manually**, via **data sources**, or determined by **calculation**.
- Aggregate or consolidate information from different company codes.

This allows a fully-fledged tax reporting application combining essential functions with the advantages of an **SAP-integrated application**.

---

## 7.6.2 SAP Document and Reporting Compliance

**SAP Document and Reporting Compliance** supports a wide variety of reports required for global tax filing in the required file formats from tax authorities. 

- Key reports include the **SAF-T** and the generation of **WHT returns**.
- It is possible to model custom scenarios such as **income tax returns** or **electronic tax balance sheets**.

### Developing Your Own Scenarios

To create a report:

1. Select **Define Compliance Reports** tile in the SAP Fiori launchpad.
2. View or define the structure of existing or new reports.

Report definition follows a uniform structure:

- **Step 1 (Properties):** Select report name and description.
- **Step 2 (Parameters):** Define report selection parameters (e.g., company code, fiscal year, posting period).
- **Step 3 (Queries):** Define queries (usually modeled as **CDS views**) supplemented by calculations based on **Business Rule Framework plus (BRFplus)**.
- **Step 4 (Document Definition):** Create the actual report form, e.g., a **WHT report for Belgium**.

The report form is defined as an **XML document**, assigning data fields from queries to form fields, offering complete flexibility.

SAP Document and Reporting Compliance also allows modeling of forms like **tax certificates**.

### Running Reports

- Reports are run using the **Run Advanced Compliance Reports** app in the SAP Fiori launchpad.
- Filters help select desired reports.
- The app monitors the **status of reporting obligations**.

### Benefits

- Efficiently model report forms in the formats required by tax authorities.
- Enables tax filings based on a **uniform platform** without relying on multiple third-party solutions.
- Solutions may require significant implementation effort but improve **process efficiency** and **automation**.
- SAP Profitability and Performance Management and SAP Document and Reporting Compliance can be extended beyond tax to other areas such as **finance** and **controlling**.
- Leads to simplification of system landscape and more economical app usage.

---

## 7.7 Direct Tax Planning with SAP Analytics Cloud

The tax function supports business decisions beyond compliance, particularly in **direct taxes**.

- Examples include evaluating whether an investment decision makes sense considering **income tax consequences**.
- Assessing current tax effects on **net assets**, **financial position**, **results of operations**, or **cash flow**.

### 7.7.1 Direct Tax Planning Scenarios

With **SAP S/4HANA**, analytics increasingly shift to business departments supported by:

- **In-memory technology** for real-time analysis and planning.
- Cloud-based applications enabling **location-independent** access.
- Modern UX with **SAPUI5** and **SAP Fiori**.

**SAP Analytics Cloud** provides a cloud-focused analytics solution with self-service components. It supports:

- **Business Intelligence (BI):** Evaluating actual data for detailed analyses via reports or dashboards.
- **Forecast Planning:** Using actual data to create forecasts or budget plans.
- **Predictive Analytics:** Automated analysis of possible scenarios using identified patterns.
- **Application Design:** Free development of complex analytic applications.

All these may be applicable for the tax function depending on **data quality**, **granularity**, and **availability**.

- Basic tax data may be directly available as SAP tables in SAP S/4HANA.
- Complex scenarios require data transformation into **tax data marts**.

Implementing SAP S/4HANA as the **single source of truth** from the tax perspective simplifies data governance and data use.

### 7.7.2 Map Fiscal Application Scenarios

In SAP Analytics Cloud, application scenarios are set up as **stories**, providing user-oriented tax planning or reporting.

- After login, create a story by clicking **Create your first story**.
- Different templates exist for stories depending on audience.
- The **dashboard template** is suitable for many tax function use cases.

---

### Figures Referenced (Descriptions)

- **Figure 7.39:** SAP Profitability and Performance Management Tiles in SAP Fiori Launchpad
- **Figure 7.40:** Process Activities in SAP Profitability and Performance Management
- **Figure 7.41:** Elements of Tax Reporting in Profitability and Performance Management
- **Figure 7.42:** Maintain Tax Rates
- **Figure 7.43:** Calculation of Current Taxes
- **Figure 7.44:** Documentation of Tax Loss Carryforwards
- **Figure 7.45:** Calculation of Deferred Taxes
- **Figure 7.46:** Define Compliance Reports Tile
- **Figure 7.47:** Define Report Parameters
- **Figure 7.48:** Define Queries
- **Figure 7.49:** Define Report (WHT report for Belgium example)
- **Figure 7.50:** Structure Defined Report
- **Figure 7.51:** Overview of Tax Data Model
- **Figure 7.52:** SAP Analytics Cloud Startup Screen
- **Figure 7.53:** Story Template Selection  
- **Figure 7.54:** Dashboard Template Example

## 7.7 Direct Tax Planning with SAP Analytics Cloud

With the help of this template, you’re able to visualize **tax data** in table or list form or via different chart variants.

### 7.7.1 Using SAP Analytics Cloud for Reports and Dashboards

To use **SAP Analytics Cloud** to create reports or dashboards, you need to connect your **tax data**. SAP Analytics Cloud provides comprehensive options for this purpose. They range from the connection of simple files (e.g., **Microsoft Excel files**) from external data sources to the connection of your **SAP system** or the databases behind it (see Figure 7.55).

### 7.7.2 Data Connection in SAP Analytics Cloud

You can model the connected data before using it in SAP Analytics Cloud. However, for efficiency and practicability, it’s advisable to set up the tax data model so that essential transformation steps that precede data usage are already implemented at the **data layer level** if possible. For example, refer to a data layer as defined in a **data warehouse** where the required data is fully transformed and stored in a specific format and table.

### 7.7.3 Implement Tax Scenarios

Before implementing SAP Analytics Cloud, consider which **tax scenarios** are to be implemented under which framework conditions. SAP Analytics Cloud can be useful in the following cases:

- Tax data is comprehensively stored in **SAP S/4HANA**, e.g., master or transaction data.
- A **heterogeneous system landscape** whose data is merged centrally in the cloud.
- Inclusion of **external data** in the analysis.
- Reports are made available to a larger group beyond just the tax function on a **cloud-based basis**.
- Reporting as a **self-service** is handled by the tax function.

#### Supplementary Considerations

If the target group doesn’t require a cloud connection, the scenario limits reporting as a self-service, or the system follows a one-ERP strategy, alternative solutions such as **SAP S/4HANA embedded analytics** may be more appropriate.

### Typical Use Cases in SAP Analytics Cloud

One use case is continuous execution of **mass data analyses** in SAP S/4HANA based on complex **check routines**. These typically serve to operationalize the **tax control framework** or simulate **e-audits**. Typical scenarios where SAP Analytics Cloud provides support include:

- Monitoring **tax receivables** and **tax provisions**.
- Monitoring the **fiscal cash flow**.
- Determining effects from **tax audits**.
- Determining **additional taxes** and **interest** according to tax audit results.
- Planning the **tax base** (tax forecast), e.g., assessing recoverability of **deferred taxes**.

### Examples of SAP Analytics Cloud Scenarios

#### Scenario 1: Simple Tax Forecasting

Used to plan the **tax base** based on the **net income for the year** under commercial law to estimate future tax burden and offsetting of **tax loss carryforwards**.

- Result displayed in **table view** (Figure 7.56) showing quantitative detailed information.
- Also shown in **dashboard view** (Figure 7.57) aggregating KPIs visually.

All data for determining the tax assessment basis can be replicated for the current fiscal year from SAP S/4HANA and updated in subsequent years. The **tax burden** is determined based on predefined formulas in SAP Analytics Cloud. **Tax rates** and **tax loss carryforwards** come from a centrally maintained database.

#### Scenario 2: Monitoring Tax Positions

SAP Analytics Cloud is used to monitor tax receivables, provisions, and expenses. Users can filter by:

- **Company code**
- **Country**
- **Vendor** (tax office)
- **Tax type**
- **Fiscal year** (assessment period)

Results are displayed in:

- **Table view** (Figure 7.58) presenting detailed quantitative information.
- **Dashboard view** (Figure 7.59) summarizing key tax item information visually.

This enables continuous monitoring and eliminates the manual reconciliation of tax positions at fiscal year-end. A standardized recording of **tax postings** is necessary, which can be supported via an **SAP Fiori app** providing automated user support for tax postings or **tax tagging principles**.

## 7.8 Summary

This chapter demonstrated the opportunities SAP S/4HANA offers for **direct taxes**. Key improvements include customizing the:

- **General ledger**
- **Organizational and master data structures**
- **Asset accounting**
- **Chart of accounts**
- **Withholding tax (WHT)**

SAP applications such as **SAP Tax Compliance** and **SAP S/4HANA embedded analytics** support **tax analysis** and **tax compliance management**. 

Solutions like **SAP Profitability and Performance Management** and **SAP Document and Reporting Compliance** aid in **reporting** and **filing**. Along with SAP Analytics Cloud for **tax planning**, these tools facilitate a **data-driven tax function**.

Direct taxes form an important **value driver** in your SAP S/4HANA project. The next chapter discusses **transfer pricing** in SAP S/4HANA.

---

# Chapter 8  
## Managing Transfer Prices in SAP S/4HANA

This chapter provides an overview of **transfer pricing** functionality in SAP S/4HANA, focusing on key tax topics like **price determination**, **monitoring**, and **reporting**.

**Transfer prices** are important in multinational companies due to:

- Scrutiny by **tax authorities** during audits
- Different objectives for **tax** and **management** purposes

For many years, discussions have centered on the relationship between **management** and **legal transfer prices** and the best optimization approaches. Tax objectives focus on **compliance**, **risk management**, and **tax effectiveness**. Management focuses on **coordination**, **performance measurement**, and **incentivization**.

This chapter covers tax and practical requirements for transfer prices, including **operational transfer pricing** and how it is done in SAP S/4HANA. Topics include transfer pricing **determination**, **monitoring**, **reporting**, and **testing**.

### Base Erosion and Profit Shifting (BEPS) Project

The **OECD G20 Base Erosion and Profit Shifting (BEPS 2.0)** project adds complexity to transfer pricing. See www.oecd.org/tax/beps for details.

## 8.1 Operational Transfer Pricing

Definitions and types of **transfer prices** lay the foundation. We introduce **management** and **operational transfer prices**, discuss processes, and outline challenges and requirements.

### 8.1.1 Tax Transfer Prices

A **transfer price** is the price for controlled exchange of tangible and intangible goods and services between related parties.

- Must follow the **arm’s length principle**.
- Should reflect the price independent parties would agree to under similar circumstances.
- The **OECD framework** provides guidance for determining an arm’s length price ([source](https://doi.org/10.1787/0e655865-en)).
- Designed to ensure each company is rewarded according to its **value contribution**.

### 8.1.2 Management Transfer Prices

Defined as **enterprise internal prices** for transfer of goods/services between stages in the value chain.

Important in **decentralized organizations** and both **intercompany** and **intracompany** transactions.

Functions include:

- **Coordination**, including **setting incentives**
- **Measurement of success**
- Calculation basis for **decision-making**
- **Inventory valuation**
- Simplified planning using **normalized values**

Coordination can conflict with tax rules as local managers may be incentivized to maximize profitability, which may not comply with tax regulations. SAP can provide a **single source of truth**, but integrating management and tax transfer prices is beyond this chapter's scope.

### 8.1.3 Operational Transfer Prices

Regardless of purpose (tax or management), transfer prices need to be **operationalized** in practice, often referred to as **operational transfer prices**.

## 8.1 Operational Transfer Pricing

**Operational transfer pricing** summarizes all activities to execute the **transfer pricing policy** effectively and efficiently through processes, systems, and organization. This leads to **compliant financial statements**, including tax reports and effective management reports.

In practice, this requires an **end-to-end transfer pricing process** integrated into different business processes and performed by numerous functions in a company. This process is outlined in the next section.

### 8.1.4 Transfer Pricing Process

The transfer pricing process can be structured in **six different steps**, as shown in **Figure 8.1**.

The operational transfer pricing process typically starts from **price setting** per the transfer pricing policy, then involves continuous **monitoring and adjustments** of prices if necessary. The output of this process is used for **transfer pricing documentation** covering:

- **Country-by-country reporting (CbCR)**
- **Master files**
- **Local files**

We’ll now introduce these steps in more detail, focusing on the operational implications:

### 1. Planning and Policies

This initial step involves the **planning of the transfer pricing model** and defining the **transfer pricing methods** for different types of **intercompany** and **intracompany transactions**.

Specifically for intercompany transactions, there are **five transfer pricing methods** applied to establish whether controlled transactions comply with the **arm’s length principle**:

- **Three traditional transaction methods:**
  - **Comparable uncontrolled price (CUP) method**  
    Compares prices agreed between related parties and prices agreed with third parties.
  - **Resale price method**  
    Applies typically to trading of purchased goods; price is based on sales price minus a normal profit margin.
  - **Cost-plus method**  
    Starts from the cost base of the provider; cost base is increased by a profit markup resulting in the transfer price.

- **Two transactional profit methods:**
  - **Transactional net margin method (TNMM)**  
    Compares the **net-profit margin** from controlled transactions relative to a base (e.g., costs, sales, assets).
  - **Transactional profit split method**  
    Allocates combined operating income or loss based on each party’s relative contribution to overall profits or losses.

The **selected method** determines the **implementation requirements** for each intercompany transaction type.

### 2. Price Setting and Contracts

Based on the transfer pricing planning, prices must be calculated according to one of the five methods.

Multinational groups typically use **multiple different transfer pricing methods** to price different transactions such as:

- Tangible goods
- Services
- Licenses
- Loans
- Transfer of functions

Each method follows different **calculation schemata** and drives different practical implications, which lead to a need for **automation**.

Typical complexities in price setting include distinguishing between:

- **Related parties**
- Granular levels of the **product hierarchy**
- Distinct **cost bases**
- Different **market prices**
- **Customs implications**
- Local **legal regulation**
- **Net profits** from selected intercompany transactions

An additional complexity is whether prices need to comply only with legal requirements or if **parallel valuations** are required for management purposes, especially in **production companies**. For example, a **group valuation** might treat intercompany trading as a stock transfer passing manufacturing costs.

Further implementation considerations based on **SAP S/4HANA** are discussed in Section 8.2.

### 3. Transaction Processing and Journal Entries

Once transfer prices are calculated, **intercompany sales and purchase transactions** are recorded using the maintained transfer prices.

For SAP, these include:

- Intercompany sales orders (e.g., **Transaction VA01**)  
- Purchase orders (e.g., **Transaction ME21N**)  
- Financial accounting invoices (e.g., **Transaction FB01**)

### 4. Monitoring and Adjusting

This step involves various **monitoring activities** required for transfer pricing management to ensure the policy is executed properly.

**Noncompliance** can lead to:

- **Double taxation**
- **Penalties**
- **Reputational risk**

Tax authorities now employ digital tools to detect anomalies. A number of reports and related activities are required, including:

- **Segmented tax margin reporting (tax transfer pricing segmentation)**  
  Important for models requiring margin monitoring, e.g., from contract manufacturing, R&D, or distribution.
- **Segmented business margin reporting (business unit segmentation)**  
  Ensures management-relevant margin reporting. Differences in segmentation require reconciliation between tax and management margins.
- **Transfer pricing risk reporting**  
  Related to risk monitoring of noncompliant profit allocation, transaction execution, and outdated prices.
- **Transfer pricing provisions report**  
  Assesses potential amounts for transfer pricing risk provisions.
- **Transfer pricing alerts**  
  Provides push information for deviations from the transfer pricing model.
- **Transfer pricing adjustments and true-up report**  
  Identifies potential adjustment amounts to ensure tax compliance and matches revenue and expense corrections in the reporting period. Shows deviation between targeted and actual profitability.
- **Multiyear project reports**  
  Used for transfer pricing induced profitability over longer periods.
- **Simulation of reports**  
  Conducts "how-to" and "what-if" analyses of transfer price adjustments or volume changes.
- **Other reports**  
  Includes profit split and reconciliation reports for parallel systems for tax and management transfer pricing.

These monitoring activities require several system capabilities described in Section 8.4.

Monitoring reports can be deployed in SAP applications such as **SAP Analytics Cloud**.

### 5. Testing and Compliance

This step verifies the proper execution of the transfer pricing policy and ensures **tax compliance documentation** is prepared.

Documentation typically follows **OECD standards** and includes:

- **Master file**
- **Local file**
- **Country-by-country reporting**

Required financial information includes:

- **Intercompany transaction volumes** by transaction type
- **Profitability data**
- **Financial data for tax jurisdictions** such as revenues, net profit before tax, taxes, provisions, equity, number of employees, and assets

According to the **BEPS 2.0 pillar one** (Tax Challenges Arising from Digitalization) action plan by the OECD, additional reporting requirements are expected, creating complexities such as:

- Revenue sourcing for distinct revenue categories
- Additional transactional classification
- Gathering of additional data points

### 6. Controversy and Litigation

This final step refers to potential controversies with tax authorities and subsequent litigation.

It includes regular analysis of **historic data** and **time series** to defend specific **fact patterns**.

---

### 8.1.5 Challenges and Requirements

Operational transfer pricing enables numerous processes across an organization but faces challenges in **end-to-end implementation** in large multinationals.

Common practical symptoms with system-related root causes include:

- **Complex international tax regulations** requiring local country-specific considerations  
  Often implemented through semi-manual spreadsheet calculations unsuited for complexity.
- Complex **intercompany transactional models** not fully addressed by current systems.
- Lack of consideration for **interdependencies with other taxes and customs**, leading to uninformed decisions and **cash leakages**.
- Built transfer pricing models are often **static** with limited flexibility for business dynamics or regulatory changes, due to shortcomings of:
  - Built-in solutions (e.g., business warehouse)
  - Difficulties adjusting complex spreadsheet models
- **Compliance issues** from limited functionality in existing transfer pricing solutions, such as:
  - Inability to perform **prospective price adjustments**
  - Resulting in large transfer price adjustments with negative side effects

These challenges highlight the need for improved **system setup** and **automation** to support effective operational transfer pricing.

## Operational Transfer Pricing Processes

The **practical transfer pricing processes** are often not set up end to end, but support only certain subprocesses such as **price setting** or **monitoring**. The interdependencies with upstream (e.g., **planning**) or downstream processes (e.g., monitoring) are frequently not considered. This is especially true for **silo solutions** and **spreadsheet-based processes**.

The **data models** are often neither of source systems nor of the operational transfer pricing system designed to meet various transfer pricing requirements. Standardization across multiple source systems causes additional issues. Source systems frequently cannot provide required raw data such as **classified intercompany transactions** and **financial data on permanent establishments**.

Monitoring requirements for multifunctional **legal entities** are challenging without a **segmented reporting solution** in place.

---

The decision support is often limited due to simplified modeling, such as reliance on spreadsheets. Simulation capabilities for **segmented profitability** considering market dynamics (e.g., material price, sales price, volumes) or transfer pricing parameters are typically missing.

---

### Trade-Off and Challenges

Multiple objectives sometimes lead to trade-offs between **compliance** and **performance management**. Simplified modeling and system enablement are common root causes.

Scattered solutions for single subprocesses result in a lack of **central coordination** and **control** over the end-to-end process. Consequences include:

- Quality issues  
- Inefficiencies  
- Delays  

A lack of a **central and integrated solution** with a **single source of truth** leads to challenges regarding **reconciliation** and **proof of numbers**.

The use of scattered subsolutions or silo solutions causes:

- Limited transparency or insights into calculations  
- Documentation gaps  
- Missing audit trails  
- Challenges during transfer pricing audits  

---

### Technical Requirements for Automation

To realize a robust and automated **operational transfer pricing process**, a solution must address the following components as shown in Figure 8.2:

- **Integration across the operational transfer pricing process**, as process steps are interdependent  
- **Data collection, checks, and validations**, including **master data enrichment**  
- **Rule-based configuration, classifications, calculations, allocations, and reporting**  
- **Traceability, auditing, and user access rights**  

---

Usually, four components are required in the solution:

- A **central data repository** holding raw data and results, including historic data to support audit inquiries  
- A **modeling tool** to reflect the rule base and perform calculations and allocations across multiple dimensions; typically designed for tax/business users for independent operation  
- A **reporting/dashboarding tool** to visualize and analyze outputs  
- A mechanism to prepare and post **journal entries, invoices, or price list updates** back to the ERP system (in this case, **SAP S/4HANA**)  

---

## 8.2 Transfer Price Calculation

According to the **transfer pricing policy**, different **transfer prices** must be calculated for intercompany transaction types such as the sale of **semi-finished goods, finished goods, and services**.

Different **pricing methods** apply in practice; frequently used methods include the **cost-plus method** and the **resale-minus method**.

---

### Cost-Plus Method

The **cost-plus method** requires determining a **cost base**, often following the **full cost approach** structured as:

- **Direct material cost**  
- + **Indirect material cost**  
- = **Total material cost**  
- + **Direct conversion cost**  
- + **Indirect conversion cost**  
- = **Total conversion cost**  
- + **General and administration and other cost**  
- = **Total cost**

This cost base must be marked up according to applicable **tax regulations**.

---

### Resale-Minus Method

The **resale-minus method** uses the **purchase price** of an intercompany transaction product, which is then sold to a third party. The resale price is reduced by an adequate **gross margin**, reflecting the transfer price.

- The **gross margin** can be a percentage determined internally or via **external benchmarking**.  
- The calculation scheme is:

  - **Market Price**  
  - +/- **Market price adjustments**  
  - = **Adjusted market price**  
  - - **Gross margin**  
  - = **Resale-minus price**

---

### Market Price Considerations

An important consideration for **tax** and **management purposes** is determining an adequate **market price** as the starting point.

In some cases, adjustments are needed for comparability—for example, if the **selling entity** serves both intercompany and third-party customers.

Thus, both the cost-plus and resale-minus methods require calculation functionalities that consider:

- Market- and customer-specific adjustments  
- Tax-specific considerations such as the **transfer pricing function**

---

Now, we will review how **costing** occurs in **SAP S/4HANA** for these methods.

---

### 8.2.1 Cost-Plus Method in SAP S/4HANA

Assuming all basic settings for **product costing** exist, the **costing structure** must establish the cost-plus scheme.

The **cost base** must contain different items introduced in the cost-plus calculation.

---

#### Key Elements in Transfer Pricing Context

- The **bill of materials (BOM)** is fundamental, containing raw materials and semifinished products required to produce finished products.  
- The BOM for costing typically equals the BOM for production. It provides quantities and rolls up costs for multi-level BOM structures.  
- The BOM can be reviewed with **Transaction CS03**.  
- Costs related to **routings** and manufacturing steps are part of the cost base and can be reviewed using **Transaction CA03**.  

---

#### Material Master and Costing

- The **material master** must contain accurate costs as they determine the calculation base via the **valuation variant**.  
- For raw materials, costs may be the **moving average price** or **standard cost**.  
- Marking and releasing the cost estimate updates the material master with cost estimates.  
- **Tax prices** can be defined in the Accounting 2 view of the material master with corresponding valuation variants.

---

#### Overhead Costs and Cost Uplifts

- In the full cost approach, overhead costs such as **general and administration costs** must be included via a **cost uplift**.  
- Materials and conversion costs typically form the basis for applying the cost uplift for indirect costs.  
- Indirect cost allocation must consider **transfer pricing requirements**, meaning costs must be attributable to the respective transaction and transfer pricing function.  
- This allocation may differ from management accounting, so multiple **valuation variants** may be necessary.

---

#### Costing Setup in SAP

- Automated determination of dedicated cost uplifts is often needed, calculating uplift per uplift type and plant.  
- Several costing variants exist in SAP, maintained within the **valuation variant** including:

  - Basis line items (e.g., direct raw material costs)  
  - Uplifts based on percentages or quantities, with multiple uplifts possible

- Conditions may be required for managing interdependencies (e.g., plants, overhead types).  
- Detailed uplift settings include:

  - Overhead amount in euros  
  - Reference amount in kilograms  
  - Credit postings setup linking cost centers and cost types  

---

#### Cost Center Allocation

- **Cost center allocations** can be used to allocate indirect costs to production orders or other centers, supporting the full cost approach.  
- This may differ from management accounting where overheads are reported separately in the management **profit and loss (P&L)** statement.

---

#### Additional Considerations

- Complex businesses may require separate cost uplift calculations for tax purposes, possibly using SAP ecosystem applications.  
- Structured reporting as per Section 8.4 is typically needed for compliant indirect cost consideration.

---

#### Cost Estimation Transactions in SAP

- The **plan costing run** can be performed with **Transaction CK40N** (or specific materials with **Transaction CK11N**).  
- This generates cost estimates for each material in the BOM.  
- Cost estimates can be reviewed using **Transaction CK13N** and provide the basis for the inventory valuation of the selling plant.

---

#### Profit Markup for Transfer Pricing

- For transfer pricing, **additional cost elements** not included in inventory valuation per **GAAP** or local tax regulations must be considered.  
- Specifically, a **profit markup** is required to ensure an **arm’s length transaction**.

## Address Different Valuation Requirements

Companies using the **Material Ledger** can make use of the **parallel valuation functionality** to distinguish between **legal valuation** and **group valuation**:

### Legal Valuation

- Allows **intercompany trading at arm’s length**
- Consideration of **additional cost and a profit markup** being charged
- Defined in the **pricing conditions** related to the affiliated companies treated like external business partners

### Group Valuation

- Treats **intercompany transactions as a stock transfer**
- The **cost of goods** is passed between the involved entities
- The price condition **KW00** selects the split of cost components, not only the selling price

---

## 8.2 Transfer Price Calculation

### 8.2.2 Resale-Minus Method in SAP S/4HANA

The **resale-minus method** is supported by the SAP standard. It considers the **purchase price** for a product in an intercompany transaction, which is then sold to a third party. The resale price is reduced by an adequate **gross margin**, reflecting the transfer price. The gross margin can be:

- A **percentage** determined internally or via **external benchmarking**

For each customer, including intercompany customers, **pricing-relevant master data** is defined (e.g., pricing procedure, applicable price list). The pricing procedure follows the information defined in the **sales document type** and the **customer master record**.

The pricing procedure defines a group of **condition types** in a particular sequence. Transaction **V/08** is used to define the intercompany pricing procedure, as shown below:

#### Pricing Procedure (Figure 8.3)

- Intercompany list price  
- +/- Material surcharges/discounts  
- +/- Customer-specific material surcharges/discounts  
- +/- Customer surcharges/discounts  
- - Price group discount  
- + Freight cost (item)  
- + Customs  
- + Taxes  
- = Final price

Pricing conditions are set either in **sales and distribution** or in **profit center accounting**. For legal purposes, the focus is on **sales and distribution** since prices need to meet **tax requirements**.

The condition types require customization to apply **customer/country discounts** and reflect specific **market price levels** via condition tables. Condition types can also consider **material-** and **country-specific markups** and need to be set up per **customer and material** (see Figure 8.4).

The value for required conditions for **tax purposes** can’t always be derived in the SAP standard. Additional calculations outside SAP are often needed for:

- **Market price adjustments**  
- Maintenance of applicable **gross margins** distinct per country and distribution channel

These complex **pricing procedures** require side calculations with correct **access sequences**, which define the search strategy for valid data for particular condition types. Further details on this are in Section 8.3.

Despite SAP standard functionalities supporting key elements of costing and pricing, **limitations** exist relating to transfer pricing dimensions (e.g., transfer pricing function, intercompany transaction types). Introducing these dimensions requires solutions outside SAP standard.

---

### 8.2.3 SAP Profitability and Performance Management

SAP S/4HANA requires additional calculations and side processes to support **cost-plus** and **resale-minus methods** for transfer pricing. Other methods like **profit split** also need extra calculations.

**SAP Profitability and Performance Management** provides key capabilities relevant for transfer pricing:

- Engine for **complex cost modeling and allocation**, optimized for many dimensions  
- Flexible modeling of **transfer price calculation** and its inputs  
- Supports **primary costing** on ERP dimensions in SAP S/4HANA  
- Enables **simulation** of scenarios including transfer pricing models and macro-economic effects  
- Manages a **stepwise process across multiple functions and stakeholders**  
- Generates **transfer pricing-relevant P&Ls** that consider transfer pricing dimensions  
- Supports **write-back** of new prices  
- Ensures **auditability and traceability**

#### SAP Profitability and Performance Management Functionalities (Figure 8.5)

- Databases, SAP Applications, Functions, Other Apps and Data  
- Scenarios, Forecasting, Drilldown, Auditability  
- Allocation, Planning, Support, Enrichment, Calculations  
- Calculation Engine, Simulation Application, Business Data Aggregator  

---

## 8.3 Transfer Price Determination

With **correct transfer prices** in the system, SAP determines the applicable transfer price for each **intercompany transaction** following the transfer pricing policy.

Factors influencing price selection include:

- Involved entities  
- Traded material  
- End customer  

SAP uses **sales and distribution functionality** to identify the applicable **pricing procedure** and **condition types**.

### Pricing Condition Master Data

- **Base price**, discounts, surcharges, etc. are defined in the **conditions master data**
- Use Transaction **VK11** to create material price conditions

### Access Sequence

- Defines the **search strategy** for valid condition records
- Prioritizes condition records when multiple prices exist, such as:  
  - Material on a global basis  
  - Material in a market  
  - Material for a specific customer group  
  - Material for a key account across markets  

The access sequence typically starts with the most specific price to the most general.

The pricing procedure executes according to the identified condition records and determines transfer prices.

---

## 8.4 Transfer Price Monitoring and Adjustments

**Plan-based transfer prices** apply to intercompany transactions. Transfer price **monitoring** ensures correct implementation of the group’s transfer pricing policy in actual transactions.

Despite detailed planning, deviations usually occur, requiring regular monitoring of **transfer price-relevant key performance indicators (KPIs)**, such as **actual intercompany profits**.

This profitability needs calculation at different granularities per relevant **transfer pricing dimensions**. If noncompliance is found, **corrective actions** are necessary.

### Segmented P&L

- Instrument for monitoring and determining transfer prices  
- Requires availability of **standard and enriched data dimensions** to classify intercompany transactions  
- Enrichment involves extending the **Universal Journal** with transfer pricing dimensions  
- Allocation and top-down distribution must consider transfer pricing dimensions  
- Regular monitoring identifies if actual margins remain within expected ranges

If actual margins deviate, **transfer prices** may require adjustment, linking back to the **price setting step** (Section 8.1.4).

#### Complete View Process (Figure 8.6)

- Extend Universal Journal entries in table **ACDOCA** with transfer pricing dimensions like transaction group  
- Perform allocations for **overhead cost accounts**  
- Perform top-down distribution  
- Check if actual margins meet target margins per group’s transfer pricing policy  
- If not, perform corrective actions (e.g., price adjustments)  

---

### 8.4.1 Extend Universal Journal

Two types of dimensions essential for transfer price monitoring:

#### Standard Dimensions

- Typically already support margin analysis, including:  
  - Legal entities  
  - Periods  
  - Customers  
  - Profit center groups/business divisions  
  - Materials  

#### Transfer Pricing Dimensions

Special dimensions necessary for transfer pricing include:

- **Transfer pricing transaction group/transaction tag**: Aggregates various intercompany transaction types like goods and services  
- Goods can be further split into **finished products**, **raw materials**, and **semifinished products**  
- Transactions are grouped/tagged according to granularity required by the **transfer pricing model** and **transfer pricing functions/methods**

## Transfer Price Monitoring and Adjustments

For example, the **intercompany selling entity** is a **contract manufacturer**, and the **transfer pricing method** applied is **cost plus a 10% markup**.

### Vendor/Supplier

- This is needed if monitoring based on transfer pricing methods such as **TNMM** or **resale price**.
- The vendor/supplier for all transactions is usually not filled.
- Usual **P&L statements** generated for financial statements don’t have the dimensions needed from the transfer pricing perspective.
- With the **Universal Journal extensibility options** available in **SAP S/4HANA**, it is possible to include additional information in a journal entry.

### Extensibility Options in SAP S/4HANA

Several extensibility options are available in **SAP S/4HANA**:

#### Classic Coding Block Extension

- Uses **Transaction OXK3** and the **CI_COBL structure**.
- This structure processes coding block information within **ABAP programs**, not storing data like a database table.
- The introduced field is updated in table **ACDOCA** along with the **COBL structure**.
- Features include:
  - Possibility to extend processes that lead to journal entries in table **ACDOCA**.
  - Derivation and validation using **validation/substitution** (**Transactions GGB0/GGB1**).

#### SAP S/4HANA Key User Extensibility Using Custom Fields and Logic App

- Successor of the classic coding block extension in **SAP S/4HANA Cloud**.
- Not recommended for the on-premise version.
- Available only in **SAP Fiori Reuse UI** for many transactions, not in **SAP GUI**.
- Derivation and validation possible using enhancement options **FIN_CODINGBLOCK_SUBSTITUTION/VALIDATION**.

#### Journal Entry Item Business Context Extensibility

- Local extension of journal entries available in **SAP S/4HANA Cloud** and **SAP S/4HANA**.
- Features:
  - Custom fields added via include structure **INCL_EEW_ACDOC** to accounting interface structure **ACCIT** and tables **ACDOCA** and **ACDOCP**.
  - Derivation of custom field values possible with **BAdI BADI_FINS_ACDOC_POSTING_EVENTS** and the **FIN_ACDOC_EXT_SUBSTITUTION** enhancement option in **SAP S/4HANA Cloud**.
  - Note: Derivation affects only table **ACDOCA**.

#### Classic Profitability Analysis Custom Characteristics

- Created using **Transaction KEA5**.
- Features:
  - Extensions of the **profitability analysis operating concern** by custom characteristics.
  - These fields are generated in journal entries in table **ACDOCA**.
  - Derivation of values possible with profitability analysis derivation tool (**Transaction KEDR**).

#### Market Segment Business Context Extensibility

- Recommended approach.
- Features:
  - Extension of profitability analysis operating concern available in **SAP S/4HANA Cloud** and on-premise as of **SAP S/4HANA release 2020**.
  - Derivation of values possible with the profitability analysis derivation tool (**Transaction KEDR**) in **SAP S/4HANA**.
  - In **SAP S/4HANA Cloud**, use the **Manage Substitution and Validation Rules** app.
  - Custom fields available in all relevant **CDS-based cubes and queries**.

### Market Segments via Transaction SE11

- Create your own field in an append to include **INCL_EEW_MARKET_SEGMENT_PS** via **Transaction SE11**.
- Enable this new field for the market segment business context via **Transaction SCFD_EUI**.
- This field becomes available in the **Custom Fields and Logic app**.
- SAP does **not recommend** extending journal entry line items by creating append structures directly in **Transaction SE11**, especially not to table **ACDOCA** due to syntax errors (see **SAP Note 2160045**).
- Restriction: Recommended to keep transfer pricing dimensions as data type **CHAR10**.

### Summary of Custom Fields

- Custom fields created with any of these extensibility options are always added to tables **ACDOCA** (journal entry line items) and **ACDOCP** (plan data line items).
- For more details, refer to **SAP Note 2453614**.

### Extensibility Options Overview

| Extensibility Option                      | SAP S/4HANA         | SAP S/4HANA Cloud          |
|------------------------------------------|---------------------|----------------------------|
| Classic Coding Block                     | Available           | Not Available              |
| Key User Extensibility: Coding Block    | Not Recommended     | Available                  |
| Key User Extensibility: Journal Entry   | Available           | Available                  |
| Classic Profitability Analysis (KEA5)   | Available           | Not Available              |
| Key User Extensibility: Market Segment  | Available as of 2020. Recommended | Available          |

---

## 8.4.2 Derivation and Validation

You can categorize transactional data for transfer pricing to allow distinct reporting of **revenue** and **direct cost**.

### Available Options

- **Validation/substitution tools**  
  - Custom fields added using the classic coding block can be derived from other values and validated using validation/substitution tool (**Transactions GGB0/GGB1**).

- **Enhancement options**  
  - Custom fields added via coding block business context can be derived using **FIN_CODINGBLOCK_SUBSTITUTION** and validated with **FIN_CODINGBLOCK_VALIDATION** (available since **SAP S/4HANA Cloud 1705**).

- **BAdIs**  
  - Custom fields added with journal entry item business context derived using **BADI_FINS_ACDOC_POSTING_EVENTS** (**SAP S/4HANA 1511**) and **FIN_ACDOC_EXT_SUBSTITUTION** (**SAP S/4HANA Cloud 1705**).  
  - Note: Derivation updates only table **ACDOCA**.

- **Manage Substitution and Validation Rules app**  
  - In **SAP S/4HANA**, use the profitability analysis derivation tool (**Transaction KEDR**) for both classic profitability analysis custom characteristics and market segment context.
  - In **SAP S/4HANA Cloud**, rules created with the **Manage Substitution and Validation Rules** app are used for derivation.

---

## 8.4.3 Indirect Cost Allocation Limitations

To allow for full P&L reporting according to **transfer pricing-relevant dimensions**, indirect costs must be allocated.

### Considerations for Application of Custom Fields

- **General ledger assessments and distributions (on-premise only)**  
  - Support custom fields using classic coding block, coding block business context, and journal entry item business context.
  - Transactions **GLGCA1** or **GLGCA6** do not enable a custom field in general ledger assessments or distributions.
  - These transactions are not available in **SAP S/4HANA Cloud**.

- **Overhead cost controlling allocations**  
  - Do not support custom fields and are therefore not an option.

- **Profitability analysis allocations**  
  - Supported if selected, for classic profitability analysis custom characteristics and market segment business context.

- **Coding block: journal entry items**  
  - Custom field values are automatically copied from sender to receiver items by **cost center** and **profit center** allocations.

- **Coding block: market segments**  
  - Custom fields can be used as receivers in allocations for **margin analysis** in both **SAP S/4HANA** and **SAP S/4HANA Cloud**.
  - Similar behavior exists for classic profitability analysis custom characteristics but only on-premise.

Because of these limitations, alternative solutions leveraging custom fields are required.

---

## 8.4.4 SAP Profitability and Performance Management

**SAP S/4HANA** has limitations in complex cost allocation requirements. A better alternative is **SAP Profitability and Performance Management** (introduced in Section 8.2.3).

- Runs on the **in-memory SAP HANA database**.
- Maintains and executes **complex calculations, rules, and simulations**.
- Capable of performing **complex allocations**.
  
### Key Concepts of Allocation

- Distinction between **sender** and **receiver**.
- The sender allocates values to the receiver based on an **allocation function**.
- Characteristics defined for the sender are used to identify matching receiver characteristics (e.g., by name matching).
- Receiver’s **key figures** represent possible distribution bases.

#### Types of Allocation

- **Direct Allocation**  
  - Distributes key figures from sender to receiver dependent on characteristics on both sides and based on the (cost) driver on the receiver site.

- **Indirect Allocation**  
  - Distributes all key figure amounts from sender to receiver leveraging the (cost) driver on the receiver site.

### Setting up Allocation Rules

- The **sender** and **receiver** rules need to be specified.
- Includes definition of the **Rule Type**, e.g., **Direct** or **Indirect allocation**.
- Sender input such as the **posted amount** is required.
- Receiver rule setup is necessary for distribution.

---

## 8.5 Transfer Price Reporting and Simulation

For managing the **transfer pricing system**, additional reports and simulation capabilities are typically required.

- Expected to provide overviews on:
  - **Tax risk** and potentially required **provisions**.
  - Interdependencies with other **taxes** and **customs duties**.
  - Additional relevant **Key Performance Indicators (KPIs)**.

- Common denominators for these reports were discussed in Section 8.4.

## Transfer Pricing Reporting in SAP S/4HANA

In cases where certain **transfer pricing master data** or results such as **net margin** in a certain segment are required, subsequent reporting is limited. This especially applies to the reporting on **transfer pricing risk** and related **provisions**, as well as for **true-up reporting** and any dependent **KPI reporting**.

Let’s first consider some standard reporting options to understand why the native **SAP S/4HANA** functionality may not be sufficient for transfer pricing scenarios:

### Embedded Analytics

**SAP S/4HANA embedded analytics** offers an integrated **transactional and analytical data platform** to process complex analytic queries. It is recommended for **operational reporting**, providing insights into the very latest data coming from business transactions. The purpose is to provide **real-time insights**.

It uses the technology of **ABAP CDS** to create **virtual data models** (representation of operational data). For transfer pricing purposes, the reporting requirement is beyond real time, as the reporting period is typically **12 months or more**. Therefore, embedded analytics doesn’t necessarily meet transfer pricing reporting requirements.

### Simulations

The simulation capabilities relate to **what-if scenarios** when transfer pricing or **transactional parameters** are adjusted. As **SAP S/4HANA** doesn’t offer specific simulation capacity out of the box, **ecosystem applications** become relevant.

Now, we’ll explain two options to allow the combination of real-time and persistent data of an entire group, which typically includes non-SAP S/4HANA transactional systems.

- The first option is **SAP BW/4HANA**, which consolidates data across the entire enterprise using a standardized but fully extensible data model for decision-making. This creates a **redundant (aggregated and harmonized) persistence of data**, resulting in a **single version of truth**.

- This single version of truth can be leveraged for transfer pricing reporting purposes that don’t require complex segmentation but only transactional volume insights.

- The second option is **SAP Profitability and Performance Management**, which supports complex reporting and **simulation requirements** related to transfer pricing, as mentioned in Section 8.4.4.

This solution allows integration with **SAP Analytics Cloud** and **SAP Digital Boardroom**, leveraging SAP Business Warehouse (**SAP BW**) and **SAP HANA**, supporting both live and import data connections.

### Analytics Component of SAP Profitability and Performance Management

SAP Profitability and Performance Management’s analytics component is the standard application to visualize data. It allows **self-service reporting**, where users can display data in data grids and charts, such as the **value flow**.

- The value flow shows the flow of values between dimensions, such as **intercompany business partners** (see Figure 8.9). The diagram supports an **interactive drilldown**.

In addition, it provides **simulation capabilities** that enable execution of **what-if scenarios** for transfer pricing management and maintenance of **assumptions and drivers**.

- Based on the granularity of the financial model, it allows drilldown and provides transparency by offering **traceability** and **auditability** information.

---

## 8.6 Transfer Price Testing and Compliance

A simulation can be accessed via defined processes. You can execute process activities and change parameters and selections at any time during execution. For **transfer pricing purposes**, this can relate to changing parameters such as the **profit markup** or **transaction volumes**.

You can access **reports** defined on top of processes that provide dynamic reporting and what-if simulations including multiple processes. When launching a report, the **Reporting & Simulation app** opens (see Figure 8.10). If included processes are of type **Simulation**, the report can be used for what-if simulation as all parameters are available for changes.

---

## Transfer Pricing Documentation Elements

Transfer pricing documentation entails several elements which drive different requirements:

### Master File and Local File

- A **master file** provides a **high-level overview** of the group, including **global business operations** and **transfer pricing policies**.

- A **local file** provides detailed **transactional transfer pricing information** for each jurisdiction. This includes material-controlled transactions, amounts involved, and transfer pricing analysis of these transactions.

- Preparation of an overview of **intercompany transactions** with related parties is required, also known as a **transaction matrix**.

### Country-by-Country Reporting (CbCR)

- In **CbCR**, different financial data must be provided for each **tax jurisdiction**, such as:
  - Revenues (unrelated party, related party, total)
  - Profit (loss) before income tax
  - Income tax paid (cash basis)
  - Income tax accrued (current year)
  - Stated capital
  - Accumulated earnings
  - Number of employees
  - Tangible assets other than cash and cash equivalents

- This section explores how these requirements can be enabled with **SAP S/4HANA**.

---

## 8.6.1 Transaction Matrix

The **transaction matrix** represents sales volume considering all **intercompany transactions** in a segmented view. Table 8.2 illustrates:

- Sales volume by **selling entity**, **buying entity**, and **transaction groups**.

- Transaction groups represent aggregated transaction types such as goods and services. Goods can be split into finished products, raw materials, semi-finished products, etc.

For generating the transaction matrix, the data dimensions discussed in Section 8.4.1 are required. With **embedded analytics** in **SAP S/4HANA**, you can create your own report for the transaction matrix depending on the availability of required dimensions.

| Buying Entity | Selling Entity | Transaction Group | Sales Volume               |
|---------------|----------------|-------------------|----------------------------|
|               | A              | Finished goods    | € 15,465,400               |
|               | A              | Semifinished goods| € 8,465,400                |
|               | B              | IT services       | € 9,465,400, split as:     |
|               |                |                   | - € 5,679,240              |
|               |                |                   | - € 3,786,160              |
| **Total**     |                |                   | € 33,396,200               |

*Table 8.2 Simplified Transaction Matrix Example*

---

## 8.6.2 Country-by-Country Reporting

**CbCR** requires preparation of two main tables:

- The **first table** requires data per jurisdiction (Table 8.3), aggregating predominantly **local entity financial data**. Financial data such as revenue, profit, and tax paid can be sourced from **financial statements** (balance sheet, income statement) generated in SAP S/4HANA (e.g., report **RFBILA00** or transaction **FC10**).

- The **Balance Sheet/Income Statement – Multidimensional app** from **SAP Fiori** can be leveraged to access account-level financial statements.

- You can also create custom reports using **SAP S/4HANA embedded analytics** capabilities.

| Tax Jurisdiction | Revenues Unrelated Party | Revenues Related Party | Total Revenues | Profit (Loss) before Income Tax | Income Tax Paid (Cash Basis) | Income Tax Accrued (Current Year) | Stated Capital | Accumulated Earnings | Number of Employees | Tangible Assets Other Than Cash and Cash Equivalents |
|------------------|--------------------------|-----------------------|----------------|-------------------------------|-----------------------------|----------------------------------|----------------|----------------------|---------------------|------------------------------------------------------|
|                  |                          |                       |                |                               |                             |                                  |                |                      |                     |                                                      |

*Table 8.3 Overview of Allocation of Income, Taxes, and Business Activities by Tax Jurisdiction*

- The **second table** (Table 8.4) requires **qualitative information** about conducted business activities, which cannot be sourced from SAP S/4HANA standard. This information typically does not require automation as it is static.

- Additional **commentary** can be added if necessary to explain or facilitate understanding of the compulsory CbCR information.

| Tax Jurisdiction | Constituent Entities Resident in Tax Jurisdiction | Tax Jurisdiction of Organization or Incorporation (if different) | Main Business Activities                                               |
|------------------|--------------------------------------------------|-----------------------------------------------------------------|---------------------------------------------------------------------|
|                  | 1. 2. 3. 4. 5.                                  |                                                                 | Research and development, Holding/managing intellectual property, Purchasing or procurement, Manufacturing or production, Sales/marketing/distribution, Administrative/support services, Services to unrelated parties, Internal group finance, Regulated financial services, Insurance, Holding shares or equity instruments, Dormant, Other |

*Table 8.4 All Constituent Entities of the MNE Group Included in Each Aggregation per Tax Jurisdiction*

---

## 8.7 Summary

You have seen that a **robust and efficient operational transfer pricing system** implemented using **SAP** addresses the following challenges:

- Improving **transparency** and **internal controls** on data and calculations, for example, using **exception reporting dashboards**.

- Reducing **year-end adjustments** and implementing **prospective transfer pricing models**, for example, segmented **P&L** with actual and forecast data integration.

- Integrating **service charging** and **royalty models** into the integrated transfer pricing solution.

- Enabling **intelligent transfer price adjustments**, including impacts of price adjustments like **customs, inventory, VAT, pharma specifications,** and **antidumping**.

- Supporting **compliance** and **audit readiness** with **historic data**, **audit trail**, and **policy making** through analytics and simulations.

Multiple operational transfer pricing subprocesses can be enabled with **SAP S/4HANA**, including price setting for certain transfer pricing methods, price determination, and reports such as the **transaction matrix** and **CbCR tables**.

However, because of limitations, it makes sense to consider **SAP ecosystem applications** such as **SAP Profitability and Performance Management**.

- Generation of full segmented **P&Ls**, integrated transfer pricing adjustments, further transfer pricing reporting capabilities, and application of certain transfer pricing methods require flexible modeling applications like **SAP Profitability and Performance Management**.

In the next chapter, tax reporting topics for both direct and indirect taxes will be discussed.

---

## Chapter 9

# Tax Reporting in SAP S/4HANA

Various capabilities and solutions are available for **indirect** and **direct tax reporting** with **SAP S/4HANA**. This chapter discusses key considerations around tax reporting and focuses on key capabilities, especially **SAP Document and Reporting Compliance**.

## 9 Tax Reporting in SAP S/4HANA

In this chapter, we focus on **tax reporting** for **SAP S/4HANA** (**indirect and direct taxes**). We start with general observations in the **tax reporting landscape**, resulting functionalities, and characteristics for **tax reporting solutions**. Then we cover periodic aggregated reporting obligations (e.g., **value-added tax [VAT] return**) and **continuous transaction control (CTC)** requirements (e.g., **e-invoicing**).

This is followed by configuration guidance for **SAP S/4HANA standard solutions** for **VAT returns**, **VAT listings**, and **Intrastat**. In addition, the **SAP reporting solution**—**SAP Document and Reporting Compliance for SAP S/4HANA**—will be discussed in detail in this chapter.

---

## 9.1 Worldwide Tax Reporting Requirements

As described in Chapter 2, more and more jurisdictions are introducing and establishing **digital reporting requirements (DRRs)**, that is, **near real-time tax reporting** and **mandatory e-invoicing regimes**. For taxpayers, especially **multinationals**, this often means paradigm shifts in terms of organizing **tax reporting processes** and implementing and covering local or global **tax reporting solutions** at a very high pace.

This is clearly a **global trend** that doesn’t exclude any region in the world. We see a very heterogenous tax reporting landscape in **South America, Europe, India, China, and Australia** within and throughout the regions and continents.

South American countries such as **Mexico** and **Brazil** were **digital tax reporting pioneers** and developed totally different **transaction-based reporting requirements**, which is a challenge for multinational and local taxpayers as neither **unification** (tax system itself and reporting obligations) could be agreed on nor **standardized**.

**Tax functions** must manage the tax challenges and the opportunities. Traditionally, **tax compliance processes** (i.e., tax reporting obligations and declarations, statutory tax accounting) are based on **financial data** that is requested from accounting and then extracted from the statutory or report-based environment into a separated tax surrounding with very few or missing connections to operational processes and transactional data.

After preparation and submission to the tax authorities, the **tax audit period** begins and often ends after years.

With the raising of **transaction-based, real-time reporting requirements worldwide**, an evolution in **digital tax compliance models** is predicted. The prediction includes live data-based **tax management** in integrated operational system landscapes with connected stakeholders of the tax functions (other areas in the company and their employees, group entities, tax administrations, and banks).

Figure 9.1 shows the **process integration** and the change from separated tasks in the traditional compliance model to **end-to-end tax scenarios** (refer to Chapter 1, Section 1.5, for end-to-end scenarios) that follow operational processes and data in the digital compliance model.

---

### Figure 9.1 Evolution of the Compliance Process (Source: EY)

A further angle is the growing integration between **taxpayers** and **tax authorities**. Resulting from the previously mentioned transition, **tax audits** can also happen based on real-time **business data** of all business partners included in the transactions.

This also has huge **efficiency potential** for both the taxpayers and the tax authorities.

Depending on the development of rising tax reporting obligations, we distinguish between:

- Pure (**real-time**) **tax reporting** without connection to business processes
- **Mandatory e-invoicing**, which is fully integrated in the business (invoicing) process.

In the new **digital compliance model** for worldwide tax reporting, there are several fundamental concepts to consider, which we’ll discuss in the following sections.

---

## 9.1.1 The Five Fingers of Tax Administration

According to **TaxVoice**, the tax requirements raised by tax authorities can be compared to a hand. We can conclude that there are at least **five fingers** that grab after the following:

- **Payments**  
  The taxpayers need to report **single payments** together with tax reporting data. **Split payment** measures change the payment directions from taxpayer (customer) directly to the tax authority. Retailers and e-commerce businesses are especially affected, but the effect isn’t limited to these industries.

- **Logistical data**  
  Tax authorities are tracking **logistical movements**, and taxpayers require live approvals of **cross-border movements of goods**. This directly affects the supply chain of the taxpayer and their customers and is therefore business critical. For example, in Hungary, there are **Electronic Road Transportation Control System (EKAER)** reporting requirements that fall under this category.

- **Purchase orders**  
  The tax authorities are demanding business documents without a direct relevance for the tax burden and tax payment. From the **purchase orders** to the invoice, many things can happen, and changes can lead to heavy investigations to meet such requirements.  
  We see this, for example, in Italy. Since **February 1, 2020**, the **Public Administrations of the National Health System** are obliged to issue purchase orders for goods in electronic format. These purchase orders are passed through the **Nodo Smistamento Ordini (NSO)** platform, affiliated under the **Sistema di Interscambio (SDI)**—effectively an invoice approval portal.  
  An extension of this obligation started in January 2021 and includes purchase orders for services.

- **Invoices/till fiscalization**  
  Invoices are part of real-time tax reporting requirements and mandatory **e-invoicing**.  
  As a result of EU studies, we’ve learned that especially **mandatory e-invoicing** is the most **proportional CTC type** that leads to transparency for the tax authorities and to efficiency gains on the taxpayer side (e.g., **SDI**, mandatory e-invoicing in Italy).  
  **Real-time tax reporting** only causes transparency without efficiency gains for taxpayers (e.g., **Suministro Inmediato de Información [SII]** in Spain).  
  With regard to **business-to-consumer (B2C) store sales**, cash registers are under control of tax authorities under the **till fiscalization** regime.

- **General ledger data on the line item level**  
  Invoices are categorized into **account groups** and need to be reported that way. This is the case in **Greece**.

---

## 9.1.2 Maturity of Tax Reporting Requirements

There are several methods to compare the **maturity of tax reporting requirements** of jurisdictions. We want to introduce a measure that takes into account the relation between the taxpayer and the digital tax administration. Figure 9.2 shows **five levels of tax reporting maturity**, which can be broken down as follows:

1. **Level 1: E-file**  
   Use of standardized **electronic forms** for filing tax returns required or optional; other income data (e.g., payroll, financial) filed electronically and matched annually.

2. **Level 2: E-accounting**  
   Submit accounting or other source data to support filings (e.g., invoices, trial balances) in a defined electronic format on a defined timetable; frequent additions and changes at this level.

**Paradigm Shift 1**  
Between level 2 (**E-accounting**) and level 3 (**E-match**), there is a paradigm shift. In comparison to level 2, the tax authorities can **match documents** on level 3 between customer and supplier and are able to manage these cross-checks in real time.

3. **Level 3: E-match**  
   Submit additional accounting and source data; government accesses additional data (bank statements), begins to match data across tax types and potentially across taxpayers and jurisdictions in real time.

4. **Level 4: E-audit**  
   Level 2 data analyzed by government entities and cross-checked to filings in real time to map the geographic economic ecosystem; taxpayers receiving electronic audit assessments with limited time to respond.

**Paradigm Shift 2**  
In real-time **e-assessments** of both supplier and customer, no more declarations are necessary as the tax authorities have already received the relevant data.

5. **Level 5: E-assess**  
   Government entities using submitted data to assess tax without the need for tax forms; taxpayers allowed a limited time to audit government-calculated tax.

---

### Figure 9.2 Levels of Tax Reporting Maturity

| 1 | 2 | 3 | 4 | 5 |
|---|---|---|---|---|
| Level 1: E-File | Level 2: E-Accounting | Level 3: E-Match | Level 4: E-Audit | Level 5: E-Assess |

---

## 9.1.3 Tax Record Evidence Requirements

Another perspective following the fast global **CTC development** is the rising importance of **evidence** of your own, and the business partners’, **transaction data**. If tax authorities offer archived transaction data and **prefilled tax declarations** as part of a centralized or decentralized exchange system, it will be much more important for tax functions to establish a very strong **evidence position** to be able to confirm or challenge preapproved transaction data by tax authorities.

Based on a study rendered by Sovos, **“Global VAT-Trends 2020”**, Figure 9.3 shows how the **source of truth** of tax data shifts from the taxpayer to the tax authorities during the ongoing **CTC transformation**.

---

### Figure 9.3 Tax Record Evidence Shift

Tax record evidence requirements fall into three categories, as shown in Figure 9.3:

- **A: Post audit**  
  In post-transactional tax audits, there is a high dependency on taxpayer’s data sources. This leads to the necessity for a **backward ascertainment** of the taxpayer’s data sources.

- **B: E-audit procedure**  
  In this category, tax authorities work based on a **copy of a taxpayer’s data source**, which is cross-checked against trading partners.  
  We see an example in Greece, with the **myDATA (my Digital Accounting and Tax Application)** platform. You file tax information to myDATA by using a **platform-compatible software tool** (e.g., Fonoa). Data is transferred into myDATA, which then validates the information and generates **e-books**.

- **C: Future procedure**  
  As a service of tax authorities, they provide data based on **authenticated, preapproved transaction data** previously received from the taxpayer. The taxpayer performs a **completeness check** to confirm the data source of the tax authority.  
  In **Italy**, tax authorities already offer a **free archive solution** (provided by the **Sogei** company).

---

## 9.1.4 Global Tax Reporting Compliance Models

As the future developments of tax reporting hold lots of new requirements in readiness, a solution for tax reporting needs to reflect the **specific situation of a company** and the resulting **tax reporting requirements**.

During **SAP S/4HANA** implementations or deployments after go-live, tax functions can and should integrate their requirements and tax reporting solution considerations.

The following categories will be considered in vendor selection processes and tax reporting solution discussions, as shown in Figure 9.4:

- **Insourcing**  
  Following the insourcing alternative, the tax function has the highest degree of **control** and needs **in-house talent and knowledge** to control and retain the in-house capabilities. The costs for people, solutions, and processes are in line with the high level of control. The insourcing scenario can be accompanied by an external partner, as in all other scenarios.

## Role of the External Partner

The **role of the external partner** is different depending on the scenario. Possible partners include:

- **Solution providers** offering in-house solutions such as **SAP Document and Reporting Compliance**
- Providers of **Software as a Service (SaaS)** solutions such as **SAP S/4HANA Cloud**

### Outsourcing

**Outsourcing** of the tax reporting process requires an additional partner or solution provider. The **in-house tax function** involvement is very limited, often restricted to sign-off on tax declarations and returns. 

Key points about outsourcing:

- **Lower investments** as payment is based on services rendered.
- Reduced **in-house use of external knowledge**.
- **Minimal to no IT investments**.
- Often chosen to ensure **flexibility in tax functions** and **secure tax compliance** during new business establishment or restructuring.
- May act as a bridge to insourcing or co-sourcing models.

### Co-sourcing

Co-sourcing is a **mixed scenario** combining aspects of both insourcing and outsourcing. 

Features of co-sourcing:

- **Functional control remains with the in-house tax function** to retain control and knowledge.
- **In-house tax resources** are essential in very data-sensitive areas.
- Headcount-related costs for recruitment, training, and employment are **reduced with external employees**.
- Often used in **service hubs or shared service centers**.

---

**All alternatives require effective tax reporting solutions**, whether internal or external. The following sections cover how to manage the tax reporting process using SAP S/4HANA core functionalities or related SAP solutions.

## 9.2 Standardized Tax Reporting in SAP

It remains possible to manage, generate, extract, and submit tax reporting data in the **legally required format** using **SAP S/4HANA**. This section provides an overview of important settings for tax reporting in SAP S/4HANA.

### 9.2.1 Standard Report for Value-Added Tax Returns

Many jurisdictions require periodic submission of **VAT returns** according to local filing requirements. 

- **Paper forms** are largely replaced by **electronic transmission**.
- Electronic reporting is often done via **public tax portal interfaces**, e.g., the UK's **Making Tax Digital (MTD)** initiative.

#### Report RFUMSV00

The **standard SAP report** for VAT returns has traditionally been **RFUMSV00**.

- **Availability**: SAP no longer supports RFUMSV00 in several countries (see SAP Note 2480067).
- After support ends, no legal changes will be updated in this report within SAP S/4HANA.
- It's recommended to explore **future-proof tax reporting solutions**.

##### Running RFUMSV00

- The report can be run in the background or via **Transaction S_ALR_87012357**.
- Transaction path:  
  `Accounting > Financial Accounting > General Ledger > Reporting > Tax Reports > General > Advance Return on Sales/Purchases`

###### Data Selection

The report selects data from **table BSET (Document Segment Tax Data)**. 

Filter options include:

- **Posting date**
- **Fiscal year and fiscal month**
- **Document date**

Additional filtering reduces processing time:

- **VAT group**
- **Tax codes** (only relevant codes included)
- **CPU date** (technical entry date)
- **Document date** (entry date in financial accounting)

In certain countries, the **tax reporting date (BKPF-VATDATE)** can be used as an additional selection criterion if activated in global company code parameters.

##### System Proposal and Customization

- The default tax reporting date is typically the **posting date**.
- The system requires each accounting document with a tax code to have a value in the **BKPF-VATDATE** field if this is activated.
- SAP provides an enhancement spot **VATDATE_RULES** with a business add-in (BAdI) **VATDATE_VALUES**, allowing customization.

##### Output and Posting Settings

- Using the **Customize output** button, you can modify report layouts.
- Assign a **general ledger account** for posting VAT liability.
- Control selection of **output tax** and **input tax** detail level.
- Use selection criteria to avoid timeouts in large data sets.

##### Update Run Parameter

- Re-running the report with **Update documents** option timestamps records.
- Fields **STMDT** (tax return date) and **STMTI** (time of program run) are automatically populated.
- The **Do not update documents** parameter processes all documents regardless of previous runs.

#### Mapping Tax Codes to VAT Return Fields

- Assign **fields of the VAT return form** to tax codes.
- Example: Tax code at 19% assigned to **VAT return field 81**.
- Transaction keys such as **MWS (accounts receivable tax codes)** and **VST (accounts payable tax codes)** control posting behavior.

#### VAT Group Settings

- Defined under menu:  
  `Financial Accounting > General Ledger Accounting > Periodic Processing > Reporting > Sales/Purchases Tax Returns > Define Tax on Sales/Purchases Groups`
- A **Tax Group** represents the VAT group, assigned to a controlling company code responsible for reporting.

#### Company Code to VAT Group Assignment

- Assign company codes to VAT groups via:  
  `Financial Accounting > General Ledger Accounting > Periodic Processing > Reporting > Sales/Purchases Tax Returns > Assign Company Codes to Tax on Sales/Purchases Groups`
- Identify controlling and subsidiary company codes and map them to the VAT group.

---

### Report RFUMSV10

The **RFUMSV10** report provides an **additional list for advance returns for tax on sales/purchases** used for **reconciliation purposes**.

- Selects data from **database table BSEG (Accounting Document Segment)**.
- Displays the **general ledger account** of VAT assessment bases.
- Does not capture postings without tax codes, so **no completeness check** is possible.

---

© 2022 by Rheinwerk Publishing Inc., Boston (MA)

## 9.2.2 Standard Reports for EC Sales List

You’ll notice many parallels to **report RFUMSV00**. One important difference to **report RFUMSV00** is the selection field **G/L account**, as this program additionally selects the **general ledger account** in the standard layout.

You can also use **SAP programs** to report your company’s **tax-exempt intra-community supplies** and other **intra-community services** and/or **deliveries** as part of **intra-community triangular transactions** in the **European Commission (EC) Sales Lists** (and **EC Purchase List** in some jurisdictions, e.g., Spain).

The submission of **EC Sales and Purchase Lists** is typically due at a fixed date per filing period that differs from one EU country to another (see **Chapter 2**).

The following sections provide an overview of the **Customizing settings** relevant for data selection and output processing of the EC Sales List.

---

### Data Medium Exchange Engine Format Tree

The system requires a **Data Medium Exchange (DME) Engine format tree** with the structure of the current reporting form to prepare the reporting data in **comma-separated values (CSV)** format.

The **DME Engine** is a set of data exchange tools that you can call using **Transaction DMEE**. You can also find the DME Engine in **Customizing** via menu path:

```
Financial Accounting (New) • General Ledger Accounting (New) • Periodic Processing • Reporting • Sales/Purchases Tax Returns • Define DME Formats for Tax Reporting
```

arriving at the screen shown in **Figure 9.13**.

Originally intended for communication with banks, the **DME Engine** is used to process data for various reports, including **tax reports**. The message type is specified by the **Tree type**, which is **ASLD (Summary Report)** for the EC Sales List.

---

### Figure 9.13 DME Engine Settings for the EC Sales List

(Includes screenshot of the DME Engine Settings)

---

### Figure 9.14 Transaction DMEE: EC Sales List Latvia

(Includes example of a DMEE structure of the EC Sales List for Latvia)

---

### Report RFASLD20

The basis for the data preparation is the **country-independent report RFASLD20** (**EC Sales List Report in DTA Format**).

---

### Report RFASLDXX Availability

There are several countries where SAP no longer supports **report RFASLD20**. SAP maintains it in the form of **notes** or as part of **support packages**. **SAP Note 2480067** shows the SAP supporting period and replacement of existing legal reports with **SAP Document and Reporting Compliance statutory reports**.

You can find **report RFASLD20** in the accounting application menu by choosing:

```
Accounting • Financial Accounting • General Ledger • Reporting • EC Sales List • General • EC Sales List in DME Format
```

(or executing **Transaction S_P00_07000221**).

In **Figure 9.15**, you can see the **report RFASLD20 selection screen**, which includes parameters such as **Company code**, **Document Number**, **Fiscal Year**, and the general selections such as **Posting date** and **Reference number**.

With these selection parameters, you can choose the relevant data for a **monthly or quarterly EC Sales (and EC Purchase) List**. You need to enter the **Reporting Quarter** or **Reporting Period** in the respective fields.

---

### Figure 9.15 Report RFASLD20: EC Sales List

(Includes screenshot of the selection screen)

---

Further selections are possible and comparable to **report RFUMSV00**.

With **report RFASLD20**, you can and should activate **Select Goods Delivery** and **Select Service** as both types of supplies are to be reported in the **EC Sales List**, even though EC Sales List requirements can differ in the European Union (EU).

The **Output control** is limited to the digital format in report RFASLD20 either as **list output** or as a **DME file**. The electronic transmission needs to be set up separately according to local requirements.

---

### Report RFASLM00

With **report RFASLM00**, you have an alternative to the digital variant in that you can **print out the EC Sales List**.

**Report RFASLM00** is accessible via **Transaction S_ALR_87012400** or menu path:

```
Accounting • Financial Accounting • General Ledger • Reporting • EC Sales List • General • EC Sales List
```

An **SAPscript** and a **PDF-based form for printing the EC Sales List** is provided by SAP with the technical name **F_ASL_DE**.

In comparison to **report RFASLD20**, there is a **Print control** section in the screen shown in **Figure 9.16** to manage the print parameters.

---

### Figure 9.16 Report RFASLM00: EC Sales List

(Includes screenshot of the selection screen with print controls)

---

In **Figure 9.17**, you see the **Print Screen List** screen after clicking **Print parameter (form)** in Figure 9.16.

In the **Print Screen List**, you can select your **Output Device** to print the **EC Sales List form**.

---

### Figure 9.17 Print Screen

(Includes print screen selection dialog)

---

## 9.3 SAP Document and Reporting Compliance for SAP S/4HANA

Advanced compliance reporting can be used in the context of **SAP S/4HANA** and **SAP S/4HANA Cloud**, now under the name **SAP Document and Reporting Compliance for SAP S/4HANA**.

With **SAP Document and Reporting Compliance for SAP S/4HANA**, you can forward aggregated company-related information to individual **local government and tax authorities**.

This ensures that **global tax reporting requirements** are met.

The variety and complexity of **local regulations** and their **tax-relevant processes** mapped in IT systems require a **central solution** for the administration and monitoring of all different requirements.

The **SAP solutions for compliance reporting** are delivered in two versions:

- **Basic compliance reporting**  
- **Advanced compliance reporting (ACR)**

The basic compliance reporting services are part of **SAP S/4HANA Enterprise Management** and are therefore included in the **SAP S/4HANA license**.

**Advanced compliance reporting** requires an **additional license**.

This section outlines the overall settings, values, objectives, and rationale for the associated configuration area.

---

### 9.3.1 Setting Up Compliance Reporting

**SAP Document and Reporting Compliance statutory reporting** needs to be set up before you can manage your tax reporting requirements with the solution.

In the following sections, we’ll discuss the steps required to enable a proper setup.

---

### Reporting Entities and Categories

To create reports in **ACR**, it’s required to set them up in **Customizing**.

To do so, you can execute **Transaction SPRO** and navigate to:

```
Financial Accounting • Advanced Compliance Reporting • Setting up Your Compliance Reporting
```

In **Customizing**, links are created between the **reporting entities** for which **VAT reports** must be submitted and the **definitions, categories, and activities** relevant for the corresponding compliance reporting scenario.

We’ll walk through some key configuration activities at a high level in this section, as shown in **Figure 9.18**.

---

### Figure 9.18 Define Compliance Reporting Entities

(Includes screenshot of Define Compliance Reporting Entities dialog)

---

The purpose of the **ACR documentation** is to capture all data following the configuration steps for ACR.

We highly recommend basing all implementation activities and the ACR setup on a separate **documentation** that becomes part of the **tax configuration framework** (until go-live) and the **tax control framework** (after go-live).

This documentation will contain a **standardized description** as well as the **technical specifications (characteristics)** required to maintain the VAT reporting parameters in the SAP system.

Let’s consider the key activities at a high level, as shown in the Dialog Structure in Figure 9.18:

1. **Define Compliance Reporting Entities**  
   Set up according to the **tax reporting-related entity structure**.

2. **Assign Report Categories to Reporting Entity**  
   Report categories that are needed for this reporting entity. A report category corresponds to the kind of compliance reporting needed, such as the **VAT return for Germany**.  
   The report categories assigned to a reporting entity should have the **same organizational units** assigned.

3. **Set Periodicity of Report Category**  
   For each report category, a **periodicity per government regulations** needs to be defined. The periodicity comprises several parameters that are used to determine time periods.

---

### Figure 9.19 Set Periodicity of Report Category

(Includes overview of standard settings and parameters)

---

Details of periodicity parameters:

- The **From** field (**active-from date** for report category) and **To** field (**active-to date**) determine the start and end of the period in which the system is able to generate reporting tasks for this report category.

- A report can be indicated as **ad hoc** or **nonperiodic** by selecting the **Is Adhoc** checkbox. Ad hoc reports have no periodicity assigned to them. Examples include **Standard Audit File for Tax (SAF-T)** reports in France or Austria.

- The **Offset** field determines the number of days after the period end that the system uses to calculate the **due date** for submission to the authorities. The value can be positive, negative, or zero.

- The **Time Unit** for offset calculation can be on a **daily**, **weekly**, **monthly**, or **yearly** basis.

- The **FY Variant** defines the reporting periods in a year for which you need to submit your legal reports to the tax authorities. It doesn’t define the company code’s fiscal year.

- The **Notify** field, the **notification period in days**, measures the number days before the report due date when the notification period for business users begins.

- The **Tax Calendar** field is useful to maintain different due dates for different reporting periods.

- With a **Factory Calendar** according to the requirements of your company, you distinguish between **working days** and **nonworking days**.

- With the **Due Date Adjustment** field, you can adjust the due date for the reporting period in case the calculated due date falls on a nonworking day.

---

4. **Set Properties of Reporting Activity**  
   Here you can assign time periods for the use of reporting activities, for example, the earliest possible date.

---

### Figure 9.20 Set Periodicity of Report Category (continued)

(Additional settings overview if applicable)

---

5. **Enter Parameters Specific to Report Category**  
   For each reporting entity, the following specifications need to be configured and documented per assigned reporting category.

6. **Enter Parameters Specific to Reporting Entity**  
   For each reporting entity, the specifications need to be configured and documented.

---

### Key Concepts

- **Reporting entity**  
  This is the **legal entity** within the organization that is obligated to submit certain compliance reports, as listed in Figure 9.18.  
  A reporting entity can comprise one or more organizational units, such as **business place**, **company code**, **section code**, or **tax jurisdiction**. One organizational unit in the reporting entity must be marked as the **leading organizational unit**.  
  The assignment of the organizational unit to the reporting entity is **time dependent**. If there is an organizational change, it must only be considered as of a certain point in time, and a new set of organizational codes must be assigned with the respective valid-from date.

- **Report category**  
  Report categories group versions of a report. The system uses the report category to help create concrete reports for a specific period and specific organizational units.  
  For example, **VAT returns for Germany** is one report category, and the **EC Sales List for Germany** is another.  
  For each of these report categories, the government may issue new versions over time in response to changes in legislation.

---

### Figure 9.20 Assign Reporting Categories to Reporting Entities

(Includes screenshot of report category assignment)

## 9.3 SAP Document and Reporting Compliance for SAP S/4HANA

### Tax Code Groups and Mapping

A **tax group version** enables a time-dependent mapping of different **tax codes** to the same field of the **VAT return form** for the same reporting country. If the **VAT rate** changes, a new tax group version should be created. This function allows the processing of **VAT reports** for reporting periods before the VAT rate change took place.

For configuration, use menu path:  
**Financial Accounting** • **General Ledger Accounting** • **Periodic Processing** • **Report** • **Sales/Purchases Tax Returns** • **Assign Tax Group Version Time-Dependent (view V_T007Z)**.

You will arrive at the screen shown in Figure 9.21. A **tax group version** and the **starting date** need to be defined, whereas the **from date** specifies the date from which the appropriate periodic VAT return can be generated by the system.

#### Table 9.1 Relevant Countries for Tax Groups

| Country Name | Short | From Date    | Version          |
|--------------|-------|--------------|------------------|
| Austria      | AT    | 01.05.2022   | Reporting Entity |
| Australia    | AU    | 01.05.2022   | Reporting Entity |
| Sweden       | SE    | 01.05.2022   | Reporting Entity |

The next step is defining the mapping between **tax codes** and the **tax boxes** on the VAT return for the **tax base amounts**. Follow the configuration for financial accounting under:  
**General Ledger** • **Accounting** • **Periodic Processing** • **Report** • **Sales/Purchases Tax Returns** • **Group Tax Base Balances (view V_T007K)**, or execute **Transaction OBCG**.

All required combinations of **tax code** and **account key** (Trs) have to be assigned to the relevant **group number** (GrpNo).

#### Table 9.2 Tax Code Mapping Example

| Tax Code | Description                               | Trs (Account Key) | Bal. GrpNo |
|----------|-------------------------------------------|-------------------|------------|
| S2       | SE-OUT-25% Output VAT (standard rate)     | MWS               | 42         |
| SW       | SE-IN-25% Purchase of services             | RC EU ESA         | 21         |
| SX       | SE-IN-25% Purchase of services third country | RC ESA         | 22         |

The group number for tax base amount 42 corresponds to **tax box 42**. Note that there may be a difference between the **internal group number** (any number between 1 and 99) and the **external group number**, which is the actual mapping to the tax box.

The next step is defining the mapping between the **VAT codes** and the **VAT boxes** on the periodic VAT return for the **tax amounts**. Follow the configuration for financial accounting under:  
**General Ledger** • **Accounting** • **Periodic Processing** • **Report** • **Sales/Purchases Tax Returns** • **Group Tax Balances (view V_T007L)**, or execute **Transaction OBCH**.

- Figure 9.22 shows a sample grouping of tax codes to tax base field numbers of a German VAT return.
- All required combinations of **tax code** and **account key** (Trs) must be assigned to the relevant **group number** (GrpNo).
- Figure 9.23 shows an example of a grouping with German **tax codes** and **group numbers**.
- Example: Two accounts payable tax codes mapped to the same VAT return field 61.

### Tax Box Structure

Several countries in SAP have the **standard boxes** of the VAT return preassigned. However, if no preassigned boxes exist, they must be defined.

For this, follow the configuration for financial accounting under:  
**General Ledger** • **Accounting** • **Periodic Processing** • **Report** • **Sales/Purchases Tax Returns** • **Define Tax Box Structure**.

#### Key Configuration Activities (Figure 9.24):

1. **Tax Box Structures**  
   Define for which structure the tax box will be used (e.g., “IE RTD – IE RTD Structure”, or “FIVATRET – VAT Return Finland”).

2. **Tax Groups**  
   Define the mapping between the field of the VAT return, tax codes, and account key.

3. **Tax Boxes**  
   Define the tax boxes including the number, name, and whether it’s related to the tax amount, tax base, or a sum (Figure 9.25).

4. **Assignment of Tax Groups to Tax Boxes**  
   Assign the tax group to the tax box (Figure 9.26).

### Internal and External Group Number Mapping

The **internal group number** can differ from the actual number of the **tax box**. Therefore, a mapping is performed between the **internal** and **external numbers**.

Follow the configuration under:  
**Financial Accounting** • **General Ledger Accounting** • **Periodic Processing** • **Report** • **Sales/Purchases Tax Returns** • **Assign External Tax Group to Internal Tax Group**.

#### Table 9.3 Example Internal and External Group Number Mapping

| Internal Group Number | External Group Number | Text                                   |
|----------------------|----------------------|---------------------------------------|
| 1                    | 000                  | Supply of goods/services               |
| 2                    | 011                  | Export                                |
| 17                   | 017                  | Intra-community supply of goods      |

This mapping allows group number assignment independent from the existing group numbers in a VAT return.

### Tax Payable Posting

With the **post payable** activity, **tax amounts** can be paid by posting the balances of **input tax accounts** and **output tax accounts** to a **VAT payable account**.

- A **general ledger account** for posting tax payables must be specified.
- The **SAP_BR_GL_ACCOUNTANT** general business role must be assigned to the user to enable post tax payables reporting.
- For **VAT groups**, additional configuration might be needed to allocate **VAT payable amounts** per entity.

---

### 9.3.2 Setting Up EC Sales Reporting

The **EC Sales List** or **recapitulative statement** is a mandatory reporting obligation within the EU to report **intra-community supplies**. 

- In 2010, filing obligations extended to EU cross-border services subject to the **B2B default place of supply rule**.
- Some member states (e.g., Spain) require reporting **intra-community acquisitions of goods**.

Configure under:  
**Financial Accounting** • **Advanced Compliance Reporting** • **Setting Up Your Compliance Reporting**.

To set up the **EC Sales List** for a reporting entity, follow the dialog structure shown in Figure 9.27. Some countries, such as Austria in the example, have a preset configuration.

- Figure 9.27 shows **Enter Parameters Specific to Report Category**.
- Figure 9.28 shows **Enter Parameters Specific to Reporting Entity**.

---

### 9.3.3 Electronic Document Processing

**Continuous Transaction Controls (CTC)** have become prevalent, requiring companies to issue business documents electronically (e.g., delivery notes, invoices, credit memos, debit memos, tax certificates) to business partners and/or legal authorities.

**SAP Document and Reporting Compliance for SAP S/4HANA** provides electronic document (**eDocument**) processing functionality to meet CTC requirements.

Visit the SAP Help Portal for details on your reporting requirements: [SAP Help Portal](http://s-prs.co/v549502).

- This product can be deployed **in the cloud** or **on-premise**.
- System and software requirements vary by deployment.

### eDocument Processing Framework

The framework allows creation of electronic instances for documents based on source documents (e.g., invoices) from other SAP applications, containing business logic for:

- Triggering eDocument generation.
- Mapping, submission, and reception of electronic messages.

The framework supports the growing number of regulator-required processes consistently and standardizes common capabilities, allowing easy incorporation of country-specific solutions.

Tasks supported include:

- Creating, processing, and monitoring eDocuments for country-/region-specific processes (**Transaction EDOC_COCKPIT**).
- Importing incoming messages and processing them (**Transaction EDOC_INBOUND_MSG**).
- Uploading incoming XML messages from the file system to the application server (**Transaction EDOC_INBOUND_UPLOAD**).
- Submitting bundled eDocuments (**Transaction EDOC_SUMMARY**).
- Automating background processing steps (**Transaction EDOC_BACKGROUND**).
- Setting the status of eDocuments to **Completed** (**Transaction EDOC_COMPLETE**).

### Integration Services from SAP Business Technology Platform (SAP BTP)

To send or receive eDocuments, the framework uses different SAP BTP services depending on scenario or country/region:

- **Peppol Exchange service** for SAP Document and Reporting Compliance for SAP S/4HANA Cloud.  
  SAP-hosted service for exchanging business documents among Peppol network participants.

- **SAP Cloud Integration for data services** (pre-July 2020 onboarding) / **SAP Integration Suite**.  
  SAP-hosted and customer-managed services offering integration packages with preconfigured flows.

- **SAP Document Compliance, inbound invoicing option for Brazil (Nota Fiscal Eletrônica project)**.  
  SAP-hosted service enabling inbound nota fiscal receipt and processing via the eDocument cockpit.

### Deployment Alternatives for eDocument Processing: Italy Example

**Connected to Tax Authority** (via SAP Integration Suite):

Process flow:

1. Create the **source document** (e.g., invoice) using an SAP application. Saving it creates an **eDocument instance** in the database.

2. Submit the eDocument by running the **Italy eInvoice process** in the eDocument cockpit (**Transaction EDOC_COCKPIT**).

3. The system retrieves the eDocument from the database and calls the **interface connector** via **EDOC_INTERFACE_CONNECTOR** BAdI.

4. This BAdI calls the interface type to connect the system to **SAP Application Interface Framework**.

5. SAP Application Interface Framework maps transactional data into the required **XML format** and saves the XML files.

6. The system calls **SAP BTP** via an **ABAP proxy** to communicate with the tax authorities’ or business partners’ systems.

7. If successful, the XML is saved. After receiving a response, the response XML is also saved.

8. [Further steps not included in the provided text.]

## SAP Document and Reporting Compliance Integration Alternatives

### Connected to Tax Authority

- **SAP BTP** processes the **XML** to comply with official communication requirements.
- Triggers web services to send XML files to the **external system**.
- Receives status information back from external systems.
- Transforms status into a consumable format (decoding and mapping).
- Forwards information to the **SAP Application Interface Framework** and then to the **SAP system**.
- System updates the **eDocument** in the database with information from external systems.

---

### Connected to Customer-Specific Solution

The SAP Document and Reporting Compliance solution connects to a **customer-specific solution** managing the link between **SAP S/4HANA** and **SDI** (Italian tax authority platform). The process flow:

1. Create the **source document** using an SAP application (e.g., financial accounting or sales and distribution).
   - Saving the source document creates an **eDocument instance** in the database.

2. Submit the eDocument by running the **Italy Fattura XML process** in the **eDocument cockpit report (Transaction EDOC_COCKPIT)**.
   - Refer to application help documentation in the report.

3. System retrieves the eDocument from the database.
   - Calls the **interface connector** to deploy the **eDocument interface connector (EDOC_INTERFACE_CONNECTOR) BAdI**.
   - This BAdI calls the **AIF_XML interface type** to connect to **SAP Application Interface Framework**.

4. SAP Application Interface Framework:
   - Maps transactional data into the required **XML format**.
   - Saves XML files in the database for further processing.

5. Options available:
   - Download XML and process manually.
   - Use the customer-specific solution for XML processing.

---

### Connected to SAP Partner Solution

The SAP Document and Reporting Compliance solution connects to an **SAP partner solution** such as **EDI service providers** offering **Reporting as a Service (RaaS)**. The process flow:

1. Create the **source document** using an SAP application.
   - System creates an **eDocument instance** in the database.

2. Submit the eDocument by running the **eDocument cockpit report (Transaction EDOC_COCKPIT)**.
   - See application help documentation in the report.

3. System retrieves the eDocument.
   - Calls the interface connector to deploy **eDocument interface connector (EDOC_INTERFACE_CONNECTOR) BAdI**.
   - BAdI calls the **AIF_PROXY interface type** to connect to **SAP Application Interface Framework**.

4. SAP Application Interface Framework:
   - Maps transactional data into the required XML format.
   - Saves XML files in the database.

5. System sends the XML and additional data to the **partner solution**.
   - The partner processes the XML and transfers it to the tax authority system.

---

## 9.4 Summary

This chapter covered the big picture of **tax reporting** with SAP:

- Established **key concepts** supporting worldwide tax reporting requirements.
- Explored standardized **tax reports** delivered by SAP.
- Examined **statutory reporting** with SAP Document and Reporting Compliance for **SAP S/4HANA**.
- Reviewed key configuration and **eDocument processing** functionality.

The next chapter focuses on a core piece of the tax function: **tax monitoring**.

---

## Chapter 10: Tax Monitoring in SAP S/4HANA

To conclude the SAP tax journey, this chapter explores **indirect tax monitoring**:

- **Requirements** and **tax control framework**.
- Available **SAP solutions** for tax monitoring.
- Examples of tax monitoring with **SAP Tax Compliance**.

### 10.1 Why Monitor Tax?

Tax authorities worldwide are introducing:

- **Continuous transaction controls (CTC)** (e.g., live tax reporting, e-invoicing).
- **Periodic transaction controls (PTC)** (e.g., Standard Audit File for Tax - SAF-T).

These controls aim to improve collection and control of tax-relevant data, enabling real-time cross-checks and assessments. This accelerates the **digitization of tax functions** and stakeholder interaction.

The **transparency of tax data** is paramount for authorities in preventing tax fraud.

---

### Evolution of Tax Management

- Responsibility for tax matters traditionally rested with **subject matter experts**.
- Due to globalization, business model transformation, and technology, tax management approaches are rapidly evolving.
- Studies show over **70%** of the world’s largest economies use technology like electronic audits and ERP data analysis.
- Taxpayers must prepare for changes to avoid **business disruption**, **tax risk**, and **unplanned tax charges**.

---

### Importance of Indirect Taxes

- **Indirect taxes** such as **Value-Added Tax (VAT)** and **Goods and Services Tax (GST)** have become crucial for government revenue.
- While **direct tax rates** have decreased, VAT and GST rates have increased and will continue to rise.
- Managing indirect taxes is critical for **cash flow optimization** and **risk reduction**.

---

### Value Drivers for Tax Monitoring

Goals related to tax function and monitoring include:

- **Reduction of compliance risks**
- **Efficiency improvements**
- **Cash flow optimization**

---

### 10.1.1 Compliance Risks

Lack of tax monitoring introduces various risks, including:

- **Loss of tax deductions**
  - Caused by delayed filings or missing tax return recognition.
- **Fines and penalties**
  - Result from failure to meet legal procedures and deadlines.
- **Increased cost to defend audits**
  - Due to lack of documentation during audits.
- **Increase in audit activity**
  - Often triggered by findings or uncertainties in foreign company activities.
- **More data requests from tax authorities**
  - Resulting from perceived low compliance.
- **Increased cost of compliance**
  - Overall increased compliance costs impact financial results.
- **Systemic ERP system exceptions**
  - Erroneous tax master data or settings may cause widespread errors.
- **Reputational risks**
  - Public announcements of compliance deviations harm operations and finances.

---

### 10.1.2 Efficiency Improvements

Tax monitoring enhances tax function efficiency by:

- **Standardizing tax compliance management processes**
  - Streamlining risk identification, mitigation, and controls.
- Increasing **usage of tax technology**
  - Supports operational tax management.
- Involving **tax as a business partner** in process automation
  - Ensures integration of tax requirements from project start to go-live.
- Enabling **identification, mitigation, and control** of tax risks and exceptions
  - Strengthens the tax control framework.

## Tax Monitoring in SAP S/4HANA

Tax monitoring enables **automated tax controls** and can be a perfect start for workflows inside or outside the **ERP system**. See **Section 10.5** for more information.  

### Teamwork  
Teamwork in a **tax function** is based on having the right **competencies** within a team and a focused understanding and execution of the individual role as part of the team. When it comes to tax monitoring, **tax specialists**, **data scientists**, and **coders** collaborate and create **tax technology** and **tax process solutions**.  

### Data-based Tax Management  
**Data-based tax management** is a prerequisite to have the right **master data** in place to describe **tax-relevant objects** in the ERP system (e.g., company code, VAT ID) and to work with the right **transaction data** to enable **tax decisions/determination** and **tax reporting**.  

### Multiple Processes for Different Jurisdictions  
Different local **processes for the same process step** (e.g., VAT return plausibility checks before filing) often result in **inefficiencies** and **redundancies**.  

### High Effort in Tax Audits  
**Tax auditors** request historical data and historical mitigation tasks that are sometimes hard to find or complicated to combine based on **archived data**.  

### Manual Correction Efforts and Processes  
Correction tasks with a **high repetition rate** due to mass data postings often require **high effort** during mitigation.  

---

### 10.1.3 Cash Flow Optimization  
The **indirect tax data flows** lead to many **payment-relevant transactions** that can be optimized starting from **compliance** and **efficiency**, impacting the **cash flow**.  

Especially, the **procure-to-pay end-to-end tax scenario** leads to many opportunities for **cash flow optimization**, such as:  
- Timely deduction of **input VAT** or **import VAT**  
- Correct payment of **withholding taxes (WHT)**  
- Application of **WHT suspension regimes** in a compliant and timely manner  

#### Figure 10.1 Indirect Tax Data Flows  
Your company buys and sells **services and goods**. **Suppliers**, **customers**, and your company report to, pay money to, and receive money from the **tax authorities**. The tax authorities can be different bodies connected by a **data exchange**. All these flows are connected by a **common business transaction**, a **common reporting obligation**, and a corresponding amount to be **paid or refunded**.  

---

#### Example: Indirect Tax Data Flows  

**Tax Meaning for Taxpayers:**  
- Affects every transaction  
- Millions of transactions per year  
- Complexity due to diverse and locally different regulations  

**Tax Meaning for Governments:**  
- Biggest income source  
- Very sensitive regarding fraud  
- Strong technological efforts to detect fraud  

**Entities Involved:**  
- Company  
- Customer  
- Supplier  
- Government Authorities  

---

## 10.2 Tax Monitoring Requirements  

Most **tax compliance solutions** download business data for only part of the business and execute **tax rule checks** once per quarter, after tax declaration completion.  

Processes for defining, scheduling, executing compliance checks, and reporting and analyzing results are rarely automated. **Manual processes** are required to:  
- Identify **compliance requirements**  
- Trigger and execute **mitigation tasks**  
- Track and remediate **compliance problems**  
- Report results  

Based on the generic process of **continuous tax monitoring** and **risk management**, SAP solutions provide a **holistic approach** with predefined automated or semi-automated steps to establish a **tax control system**.  

Having a **single source of data** and applying **dynamic exception reporting** and **tax risk management** provides valuable insight into understanding a company’s **VAT position** and helps to make better, informed decisions.  

#### Figure 10.2 Tax Monitoring Steps  

On a conceptual level, defining and valuating **tax-related risks** leads to a catalog of **principles**, **measures**, and **controls**. On an operational level, these must be implemented.  

Relevant categories for implementation include:  
- Necessary **roles and responsibilities** to execute controls  
- Concrete **control catalog** for identified controls  
- Identification of **exceptions** based on tax-relevant data  
- **Correction steps** based on findings  
- Integration of **learnings** into monitoring rules and mitigation tasks for continuous improvement  

The **monitoring level** describes action items that a generic monitoring process consists of:  
- Identify **compliance requirements** (e.g., correct standard tax rate)  
- Define and implement **queries** (e.g., comparing calculated tax with posted tax amount)  
- Plan and execute **queries** (e.g., monthly VAT return filing deadlines)  
- Output results and perform **compliance risk analysis** (generate results list and highlight exceptions)  
- Execute necessary **corrective actions** for deviations found  
- Follow up on **correction tasks** and clean up compliance risks  
- Integrate monitoring results into the **monitoring process** to adjust checks (e.g., reduce false positives)  

---

### Deploying Tax Monitoring and Compliance Management with SAP Solutions  

Steps include:  
1. **End-to-end technical implementation**  
2. Creation of **data and integration models** reflecting tax-relevant business processes  
3. Client-specific creation or adaptation of third-party **ready-to-use tax compliance algorithms** based on the SAP tax data model  
4. Creation and use of **worklists**  
5. **Change management**  

Implementing SAP-based tax monitoring solutions (e.g., SAP Tax Compliance) requires a specially designed **data model**, built on years of tax data analytics and SAP HANA experience. This improves performance, reduces resource consumption, and allows tax functions to maintain and customize rules to a changing tax control environment.  

### Success Factors for SAP Tax Compliance Projects  

Successful projects require collaboration among personnel from different disciplines, combining:  
- Business and technical **know-how**  
- Experience in **risk and controls**  
- Ability to extract insights from **mass data**  
- Knowledge of **digital audit approaches** by tax administrations  

---

### Table 10.1 Tax Monitoring Project Areas Based on an SAP Tax Compliance Example  

| IT Tasks                                          | Customization Tasks                      | Steps with SAP Solutions                          |  
|--------------------------------------------------|-----------------------------------------|-------------------------------------------------|  
| Assessment of **infrastructure and hardware**    | **Tax risk assessment**                   | Creation of **tax compliance scenarios**          |  
| Specific adjustments of **data model**            | Implementation of **check routines**      | SAP Tax Compliance **workflow** and reporting     |  
| Technical setup of SAP Tax Compliance environment | Customizing of **dashboards and cockpits** | User training and documentation                    |  

---

## 10.3 Tax Control Framework  

As outlined in Chapter 1, a **tax operating model** includes a **tax control framework** to manage **tax processes** and **tax data** from a tax control point of view.  

The **management of an enterprise** is obligated to install and control a framework to ensure the **correctness** and **completeness** of the company’s **tax-relevant obligations**.  

#### Figure 10.3 Tax Control Framework (Source: EY)  

**Controls during business transactions recording** and on the **master data level** are called **preventive controls**. These typically lie in the **operational processes**: order-to-cash and procure-to-pay.  

In all companies, there is a high dependency on **master data** describing **tax-relevant objects** such as:  
- Customer data  
- Supplier data  
- Material data  
- Plants and locations  

Master data is stored separately to be used for transactions. **Transactional data** is recorded during business processes based on **transaction types**. Examples include:  
- Sales orders  
- Invoices  
- Purchase orders  

This transactional data can be populated by master data (e.g., customer VAT ID) or entered during the transaction process (e.g., alternative customer VAT ID).  

When data passes operational processes and is interfaced into **financial data**, we enter the **record-to-report process**. Corrections such as **credit notes**, **bad debt adjustments**, or **accruals** are typical in this area.  

Once corrected values pass the **general ledger** and **annual or quarterly reports**, **tax returns and declarations** are based on the financial data.  

Controls in this area are called **detective controls** because the data has left the operational process, and exceptions can now be identified based on **financial data**.

## Financial and Operational Data in SAP Tax Monitoring

Financial data has no direct relation to the **operational base data** such as the **country of departure** or **country of destination** since these aren’t relevant for accounting. However, they may be relevant to identify **VAT-exempt intra-community supplies of goods**.

Connections between the **financial data** and the **operational data** need to be built on the basis of an **SAP data model**. Along the SAP process, different SAP tables can be linked by key fields and identifiers. The result is called the **SAP tax data model**, which forms the basis for all tax monitoring activities in SAP.

## 10.3 Tax Control Framework

We can distinguish different steps in a **tax control framework**, as shown in Figure 10.4:

### 1. Identify  
The first step is to identify the **tax-relevant risks** based on the **tax process design framework**. Refer to Chapter 1, Section 1.5, for the different **tax-relevant end-to-end scenarios**.

### 2. Mitigate  
Once identified, risks need to be mitigated with corresponding **control measures**. The extent of coverage depends mainly on the **amount at risk (yearly basis)** and the **likelihood of the risk event**.

### 3. Control  
Implemented measures need to be controlled to ensure **completeness, correctness, accuracy, and actuality**. Controls can be monitored via:  
- **Plausibility checks** (e.g., comparing EC Sales List to VAT return for EC supply of goods)  
- **Analytical data checks** (e.g., monthly SAP Tax Compliance scenario run)

### Example  
During the **order-to-cash process**, complex transactions like **drop shipments** worth 100 MEUR at risk with over 50% likelihood can cause **incorrect tax treatment**, resulting in inaccurate **VAT returns** and payments. This high risk calls for **automated tax determination logic** and **real-time tax monitoring** to identify and mitigate exceptions.

### Risk Assessments in Business Processes  
- **Indirect tax risk assessments** involve multiple stakeholders outside the tax department who influence or make tax decisions.

### Figures and Tables  
- **Figure 10.5**: Procure-to-Pay Risks  
- **Figure 10.6**: Order-to-Cash Risks  
- **Table 10.2**: Procure-to-Pay Risks, Mitigations, and Controls  
- **Table 10.3**: Order-to-Cash Risks, Mitigations, and Controls

---

## Procure-to-Pay Process Risk Flow (Figure 10.5)

- **Supplier Master Data Team** maintains supplier master data  
- **Procurement Team** creates purchase orders  
- **Accounts Payable Team** posts and checks invoices  
- **Treasury Department** manages payments  

### Key Steps  
- Maintain supplier master data  
- Create purchase order  
- Check receipt and post goods receipt note  
- Scan and archive invoice  

---

## Table 10.2 Procure-to-Pay Risks, Mitigations, and Controls

| Department/Area | Process Step         | Risk                                                | Control                                                             |
|-----------------|---------------------|-----------------------------------------------------|---------------------------------------------------------------------|
| Supplier master data | Maintain supplier master data | Wrong supplier plant country causing wrong VAT treatment | Regular request/check of supplier plants by master data team or supplier |
| Procurement      | Create purchase order | Wrong VAT treatment causing wrong supplier invoice posting | Automated accounts payable VAT determination and monthly tax monitoring controls |
| Accounts payable team | Post invoice         | Wrong tax code assignment                            | Accounts payable tax determination in purchase order as proposal     |
| Treasury department | Payment based on three-way match | Input VAT deduction too high                          | Accounts payable tax determination in purchase order as proposal     |

---

## Order-to-Cash Process Risk Flow (Figure 10.6)

- Begins with **customer master data**  
- Moves through **sales** and **financial accounting interfaces**  
- Includes **invoicing** and **tax code allocation**  
- Ends with **internal and external reporting**  

---

## Table 10.3 Order-to-Cash Risks, Mitigations, and Controls

| Department/Area           | Process Step                           | Risk                                         | Control                                                                 |
|--------------------------|-------------------------------------|----------------------------------------------|-------------------------------------------------------------------------|
| Master data maintenance  | Maintain product master data          | Wrong material tax classification causing wrong VAT treatment | Regular checks based on tax monitoring exceptions                        |
| Central VAT transaction processes | Tax decision                       | Wrong VAT decisions causing wrong VAT returns and invoices | Automated accounts receivable VAT determination and real-time tax monitoring |
| General ledger           | Sales and distribution/financial accounting interface | Wrong tax codes due to incorrect settings     | Tax code concept aligned with best practices for plants abroad          |

---

## 10.4 SAP Solutions for Indirect Tax Monitoring

This section covers SAP offerings for **indirect tax monitoring**, focusing on **SAP S/4HANA embedded analytics**, **SAP Analytics Cloud**, **SAP BW/4HANA**, and **SAP Tax Compliance**.

### 10.4.1 SAP S/4HANA Embedded Analytics

- Works directly on the **SAP S/4HANA database** for **real-time reporting** without needing data replication.  
- No legacy data from other sources is available.  
- Simplifies modeling and reduces complexity.  
- Integrates with **SAP Fiori apps** for tax functions, though tax-specific frontends require customization.

> **Note:** For direct tax functionalities, refer to Chapter 7.

### 10.4.2 SAP Analytics Cloud

- Combines analytical and transactional capabilities from various operational and legal systems on one platform.  
- Uses **SAP Business Warehouse (SAP BW)** embedded functionality plus advanced data visualization.  
- Benefits:  
  - Reduces need for full-scale data warehousing and third-party applications.  
  - Provides **real-time operational reporting** on tax matters like **VAT, GST,** and **EC Sales Lists**.  
  - Tracks relevant data for audit trails.  
  - Modern interfaces for business users, developers, and administrators.  
  - Extracts large volumes of SAP data from SAP S/4HANA or SAP S/4HANA Cloud.

#### Figure 10.7 SAP Analytics Cloud Overview

- Data modeling based on **Core Data Services (CDS) views** containing VAT analytics logic.  
- SAP Analytics Cloud used for consistent visualization.  
- No data replication needed as SAP core data is monitored directly.

### 10.4.3 SAP BW/4HANA

- **SAP BW/4HANA** is a **business warehouse** based on the **SAP HANA** in-memory database technology for fast and flexible reporting.  
- Often exists as a data source for other functions like controlling or finance.  
- Can be a **single source of truth** for the tax department, especially when integrating legacy data.  
- Does not include a built-in UI for tax monitoring; requires additional design and maintenance of frontend interfaces.

## 10.4.4 SAP Tax Compliance

**SAP Tax Compliance** delivers the technical framework for an **end-to-end process** covering the **automated detection, investigation, and mitigation of tax issues** while increasing tax compliance in business processes. Automating tax-checking processes with a unified, standardized set of rules reduces time and effort and allows anomalies to be identified more accurately.

This is facilitated by using predefined components including the following:

- **Standard SAP data request** to quickly identify and collect required data  
- **Structured knowledge transfer** to enable IT, tax, and business users  
- **Customizable and ready-to-use check routines** to quickly begin monitoring tax data and detecting tax risks  
- **Predefined workflow content** that can be easily customized to set up a workflow process to mitigate identified tax risks  

Checks can be grouped centrally into **scenarios** based on predefined fields such as line of business, geographic location, timing, and tax type. The solution not only identifies **compliance issues** and detects error-prone postings with a variety of check routines but also covers **indirect tax supply chain management** and helps with managing **indirect tax cash flow**.

Furthermore, SAP Tax Compliance provides **context-rich information** for each alert, enabling improved **transparency**, **traceability**, and **auditability** of data for the use of the tax function. Mitigation actions are triggered semiautomatically, including mitigation status and documentation. **Tax compliance reporting** completes the cycle of SAP Tax Compliance management, leveraging the tax function to the next level of tax automation and risk management.

The ability to be close to activities of the business through the effective use of data, even if they are located at disparate places, becomes increasingly important to **centralize and standardize VAT processes** in shared service environments without losing oversight and control of **VAT** and **GST compliance**.

The solution provides the ability to view and analyze country or sector-specific data, as well as drill down to individual tests and transactional data. This enables management of compliance, focus on major cost drivers, and development of a tax strategy on a local and global basis.

### SAP Tax Compliance Workflow (Figure 10.8)

The workflow starts with the connection of data sources from different SAP and non-SAP sources. The data runs through **compliance scenarios** that check the tax requirements based on a central **check repository**. These checks lead to hits where exceptions need to be identified by the respective owners of SAP Tax Compliance tasks.

- The SAP Tax Compliance user can assign tasks to connected business partners to validate the hits.  
- If hits are confirmed as exceptions, corrective actions can be taken and managed with task lists that assign tasks to respective owners in operational business processes.  
- Compliance checks and tax-relevant ERP settings can be optimized over time when certain findings appear on a recurring basis.

This leads to **efficient tax compliance management** with SAP Tax Compliance. See Section 10.5 for more details.

---

### SAP Tax Compliance Sidecar and Add-On Approaches

SAP Tax Compliance needs basic data to operate and enable data-based workflows. There are two major alternative ways to access the tax-relevant transaction data and master data:

#### Sidecar Approach

One approach is the **sidecar method** (Figure 10.9). Companies with different data sources, including non-SAP systems, can use this replication method to have one **single source of truth** for SAP Tax Compliance management. This method helps companies be more independent from **SAP S/4HANA core activities and outages**. The replication method requires selection of reasonable data replication intervals to enable respective check routines.

#### Add-On Approach

Another approach is **SAP Tax Compliance as an add-on solution** (Figure 10.10). In this scenario, SAP Tax Compliance operates directly on the **SAP S/4HANA core database**, enabling **real-time compliance checks** without the need for data replication.

---

## 10.5 Monitoring with SAP Tax Compliance

### Benefits of SAP Tax Compliance

The benefits of SAP Tax Compliance include:

- **Best-in-class in-house automated tax compliance process solution**  
- Enables a company-wide **standardized tax compliance approach** for multiple jurisdictions  
- Documentation of **tax controls and corrective actions**  
- Reduction of **manual effort** for tax compliance  
- Establishes clear **roles and responsibilities** in the tax compliance process  
- Improved **tax data quality** as the basis for tax declarations  

SAP Tax Compliance software comes with broad tax compliance functionality and workflow integration. However, a reasonable business case for tax depends on the SAP tax data model implementation, including data tests for sales and distribution, materials management, and financial accounting. Data sources and replication methods have a huge impact on the tax use case.

Organizations need to adopt SAP Tax Compliance’s **workflow concept** as the tax function has deep reach into business processes. This is a great opportunity for the quality of tax data and a significant change to be managed for many organizations.

---

### SAP Tax Compliance Apps Overview

After exploring SAP solutions for tax monitoring and compliance, detailed processes with SAP Tax Compliance are explored here.

SAP Tax Compliance offers a **Compliance Results Overview app** (Figure 10.11) presenting all relevant information captured in one dashboard. This enables **smart handling** of all tax compliance tasks by the tax compliance manager.

---

### Manage Compliance Checks App

- Create a compliance check by clicking **Create** in the menu bar (Figure 10.12).  
- Create your own data test by selecting **Available Fields** under the FIELD CONTROL section.  
- Example: A sample data test called **Test 1** accesses selected fields (Figure 10.13).  

In the **ASSIGNMENT OF TASK LIST TEMPLATES** section, a task list can be assigned to the compliance check (Figure 10.14). Task lists, tasks, and confirmations can be managed automatically by toggling settings to ON.

---

### Compliance Scenarios

A key function is the ability to combine compliance checks into **compliance scenarios**. Using the **Manage Compliance Scenarios app**:

- Assign data tests and manage the frequency of scenario runs.  
- Click **Create** to define a compliance scenario by assigning general information, compliance checks, and scheduling (Figure 10.15).  
- Example: Scenario **Tax Compliance Demo Package** with scheduling details for start date, next run date, and recurrence (Figure 10.16).  

---

### Running Compliance Scenarios

After setup, a compliance scenario can be activated manually or based on recurrence settings:

- Run manually via the **Run Compliance Scenario app** by entering the Compliance Scenario ID and Date, then clicking Execute (Figure 10.17).  

---

### Compliance Check Results

The **My Compliance Check Results app** provides an overview of compliance scenario runs and the status of data tests supporting tax compliance during filing deadlines (Figure 10.18).

- Run status can be **Completed** if no hits are found.  
- Use filters to manage multiple scenarios.  
- Open runs allow confirmation of tax compliance items.  

---

### Compliance Check Hits

Clicking line items from the results overview leads to the **Compliance Check Hits app** (Figure 10.19):

- Shows a summary of compliance hits and their handling.  
- Exception line items can be confirmed by users.  

Detailed line item views after selection allow:

- Extracting lists for forwarding in workflow.  
- Checking and resolving issues within the workflow (Figure 10.20).

## Figure 10.20 Compliance Check Hits App: Detailed View

Once you select **compliance hits** in the **result list**, you can create and assign a **task list** for the selected hits based on available templates. In Figure 10.21, we’ve chosen **Template 169**, which can be set up and changed in the **Manage Task List Templates** app.

---

## Figure 10.21 Create Task List for Selected Hits

---

## Figure 10.22 Applicable Task Templates

Figure 10.22 shows the **Applicable Task Templates** that can be applied to create a new task list. In our demo data, the templates involve about two steps:

1. **Cancel the billing document** and issue a new billing document.  
2. **Correct the VAT return** / amend the expected **VAT burden**.

Based on that functionality, you don’t need to start from scratch each time you create a task list by clicking on **Create** in the Create Task List For Selected Hits window shown earlier in Figure 10.21.

---

© 2022 by Rheinwerk Publishing Inc., Boston (MA)

---

## 10.5 Monitoring with SAP Tax Compliance

Regarding **task lists**, you can manage them in a separate app called **Manage Task List Templates**, as shown in Figure 10.23. Task list templates can be deployed for **regional customizing** and for **local tax requirements**.

---

## Figure 10.23 Manage Task List Templates App (Source: EY)

---

**Compliance tasks** can be assigned to users in the **workflow**, as shown in Figure 10.24. We navigate to this window by starting the **My Compliance Tasks** app. Then we choose the tasks assigned to the **task list 46** on the left side, which comprises our selected compliance hits results from the list shown previously in Figure 10.20.

The **task list item** contains the task, the respective **compliance check** that led to a hit, and a **check description**, including the **VAT risk**. Furthermore, the user that receives this task finds a description of exactly what to do to mitigate this hit, including a deadline until **May 14, 2022**.

**Transaction details** ensure solid communication based on the transaction level. When the **SAP Tax Compliance manager** clicks on **Complete**, the task will be assigned to the responsible user (in our case, an **accounts payable user**), and the task appears as **Completed** from the user point of view.

The status of the task can be checked starting with the **Compliance Results Overview** app, as shown previously in Figure 10.11.

Clicking the **Show Log** button shows the timeline on the left side where all historical actions are recorded that are assigned to this task. Based on this, **log tax audits** and further requests from authorities can be managed very efficiently for compliance.

---

## Figure 10.24 Assign Workflow Items to Other Relevant Users (Source: EY)

---

## 10.6 Summary

In this chapter, we provided you with a basic understanding of **tax monitoring activities**. You learned about the role of **tax monitoring** as an activity in a **tax control framework**. Furthermore, you’re now familiar with the most relevant **SAP solutions** for tax monitoring.

We detailed the best-in-class solution, **SAP Tax Compliance**, to make you comfortable working with the most important functionality. We’ve now completed our coverage of **tax in SAP S/4HANA**, from basic settings all the way to monitoring.

We hope that you’ve gained the skills and confidence to implement the **tax function** in your **SAP S/4HANA system**.

---

© 2022 by Rheinwerk Publishing Inc., Boston (MA)

---

## Appendices

- **A Europe** ........................................................................................................................ 441  
- **B India** ............................................................................................................................ 461  
- **C North America** ......................................................................................................... 465  
- **D The Authors** .............................................................................................................. 473  

---

© 2022 by Rheinwerk Publishing Inc., Boston (MA)

---

## Appendix A  
### Europe

In this appendix, we’ll take a deeper look into special **reporting requirements** and settings for **Europe**, or more specifically the **European Union (EU)**. We’ll look into **Intrastat reports**, the **European Commission (EC) Sales List** and **EC Purchase List**, and **Standard Audit File for Tax (SAF-T)**.

Note that there are additional requirements that are mandatory in many jurisdictions that will not be covered here.

---

### A.1 Intrastat Report

The **Intrastat report** is a **statistical report** that tracks movement of goods between different **EU member states**. Generally, it’s linked to a **threshold of cross-border goods movements** and created monthly once this threshold is reached.

Even though the Intrastat report is a **statistical reporting requirement** related to goods movement, it’s classified as an **indirect tax reporting requirement**.

Several configuration steps are involved. Starting **January 2022**, the EU has tried to **harmonize the fields** of the Intrastat report. For some countries, this resulted in new fields, such as:

- **Nature of transaction** (business transaction type in SAP)  
- **Country of origin**  
- **Value-added tax (VAT) ID** of the recipient of the goods  

These are generally all available in **SAP S/4HANA**, as they have been mandatory in other countries before.

**Note:**  
The **user exit** to map, modify, or exclude data for the Intrastat report is **EXIT_SAPLV50G_001**.

We’ll explain the **basic settings** for Intrastat reporting, as well as how to set up **default values** and **providers of information**, in the following sections. We’ll also show you how to run an Intrastat report.

---

### A.1.1 Basic Settings for Intrastat Fields

There are many **key fields** for Intrastat reporting. Let’s walk through each.

---

#### Define Partner Country

To get started, use **Transaction SPRO** and follow path:  
**Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Partner Country • Define Partner Country**,  
or use **Transaction SM30** with object **/ECRS/V_TPCI**. 

Here, you set up all the **country codes** that are relevant for Intrastat reporting, that is, all **EU member countries**.

Note that with the **Northern Ireland treaty**, Northern Ireland must be added here as well as country **XI**.

This view should be prepopulated as shown in Figure A.1. If it’s not, click the **New Entries** button, and add the **Partner Country** and **Description**.

---

#### Figure A.1 Intrastat: Define Partner Country

---

#### Assign Partner Country to Country of Declaration

Use **Transaction SPRO** and follow path:  
**Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Partner Country • Assign Partner Country to Country of Declaration**,  
or use **Transaction SM30** with object **/ECRS/V_TRAP**.

For all partner countries created in the step before, you now need to create the **mapping** to the countries of the company code(s).

For every country where your company must file an **Intrastat report (Country of Decl.)**, a mapping to each **Partner Country** must be made, as shown in Figure A.2.

---

#### Figure A.2 Intrastat: Assign Partner Country to Country of Declaration

---

#### Define Exceptions for Conversion of Partner Country

Use **Transaction SPRO** and follow path:  
**Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Partner Country • Define Exceptions for Conversion of Partner Country Declaration**,  
or use **Transaction SM30** with object **/ECRS/V_TMPC**.

This is the place where you can define **mappings between special regions** that belong to a country and an Intrastat country, such as **Northern Ireland**. Or you can define mappings between countries that belong inside the **VAT union** of the EU, such as **Monaco**, which belongs to the VAT region of France.

An example is shown in Figure A.3.

---

#### Figure A.3 Intrastat: Define Exceptions for Conversion of Partner Country Declaration

---

#### Define Country of Origin

Use **Transaction SPRO** and follow path:  
**Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Country of Origin • Define Country of Origin**,  
or use **Transaction SM30** with object **/ECRS/V_TCOO**.

As the **country of origin** can be any country in the world, all countries should be maintained here. These should be prepopulated with the values in table **T005**.

If a country is missing, click the **New Entries** button, and add the **country code** and **name of the country**, as shown in Figure A.4.

---

#### Figure A.4 Intrastat: Define Country of Origin

---

#### Assign Country of Origin to Country of Declaration

Use **Transaction SPRO** and follow path:  
**Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Country of Origin • Assign Country of Origin to Country of Declaration Origin**,  
or use **Transaction SM30** with object **/ECRS/V_TRCO**.

Like we did for the partner country, we now need to assign all **countries of origin** to every country where your company must file an Intrastat report (**Country of Declaration**), as shown in Figure A.5.

---

#### Figure A.5 Intrastat: Assign Country of Origin to Country of Declaration

---

#### Define Exceptions for Mapping of Country of Origin

Use **Transaction SPRO** and follow path:  
**Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Country of Origin • Define Exceptions for Mapping of Country of Origin**,  
or use **Transaction SM30** with object **/ECRS/V_TMCO**.

Again, as you did for the partner countries, you can now define **exceptions for special regions or countries**, as shown in Figure A.6.

---

#### Figure A.6 Intrastat: Define Exceptions for Mapping of Country of Origin

---

#### Define Region

Use **Transaction SPRO** and follow path:  
**Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Region • Define Region**,  
or use **Transaction SM30** with object **/ECRS/V_TRGI**.

Here, you define the **regions within a country**. This isn’t relevant for all countries and should be checked in the individual Intrastat regulations. In **Germany**, for example, this information must be reported.

You can see an example of some German regions in Figure A.7.

---

#### Figure A.7 Intrastat: Define Region

---

#### Define Conversion of Region

Use **Transaction SPRO** and follow path:  
**Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Region • Define Conversion of Region**,  
or use **Transaction SM30** with object **/ECRS/V_TMRE**.

Usually, the **SAP regions** (see table **T005E**) are different from the official **Intrastat regions** published by the authorities.

Within this setting, a **mapping between the SAP regions and Intrastat regions** can be made (see Figure A.8).

---

#### Figure A.8 Intrastat: Define Conversion of Region

---

#### Define Business Transaction Type

As mentioned earlier, the **business transaction type** is one of the fields that was harmonized inside the EU with the **2022 changes** for the Intrastat report.

The **nature of transactions** should be validated with the respective **directives** of each country, but they generally consist of **eight categories** ranging from:

- Transactions involving actual **change of ownership** with **financial compensation**  
- **Return and replacements free of charge**  
- Special cases, such as **call-off stock** or **processing under contract**  

**Table A.1** shows the business transaction types that all **EU countries** have agreed on and that are used in the Intrastat report.

## A Europe

### Table A.1 Intrastat: Nature of Transaction Codes

| Code | Category                                               | Description                                                                                  |
|-------|-------------------------------------------------------|----------------------------------------------------------------------------------------------|
| **11** | Transactions involving actual change of ownership with financial compensation | Outright sale/purchase except direct trade with/by private consumers                         |
| **12** | Direct trade with/by private consumers (including distance sale) |                                                                                            |
| **21** | Return and replacement of goods free of charge after registration of the original transaction | Return of goods                                                                             |
| **22** | Replacement of returned goods                          |                                                                                            |
| **23** | Replacement (e.g., under warranty) for goods not being returned |                                                                                            |
| **31** | Transactions involving intended change of ownership or change of ownership without financial compensation | Movements to/from a warehouse (excluding call-off and consignment stock)                    |
| **32** | Supply for sale on approval or after trial (including call-off and consignment stock) |                                                                                            |
| **33** | Financial leasing                                     |                                                                                            |
| **34** | Transactions involving transfer of ownership without financial compensation |                                                                                            |
| **41** | Transactions with a view to processing under contract (not involving change of ownership) | Goods expected to return to the initial member state/country of export                       |
| **42** | Goods not expected to return to the initial member state/country of export |                                                                                            |
| **51** | Transactions following processing under contract (not involving change of ownership) | Goods returning to the initial member state/country of export                               |
| **52** | Goods not returning to the initial member state/country of export |                                                                                            |
| **71** | Transactions with a view to/following customs clearance (not involving change of ownership, related to goods in quasi-import or export) | Release of goods for free circulation in a member state with a subsequent export to another member state |
| **72** | Transportation of goods from one member state to another member state to place the goods under the export procedure |                                                                                            |
| **80** | Transactions involving the supply of building materials and technical equipment under a general construction or civil engineering contract for which no separate invoicing of the goods is required and an invoice for the total contract is issued | N/A                                                                                        |
| **91** | Other transactions that can’t be classified under other codes | Hire, loan, and operational leasing longer than 24 months                                    |
| **99** | Other                                                 |                                                                                            |

© 2022 by **Rheinwerk Publishing Inc., Boston (MA)**

---

## A.1 Intrastat Report

### Define Business Transaction Type

- Use **Transaction SPRO** and follow path:  
  *Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Define Business Transaction Type*,  
  or use **Transaction SM30** with object **/ECRS/V_T605**.
  
- Mapping of **business transaction types** must be made for each country where you file the Intrastat report.

- Example shown in **Figure A.9** illustrates defining business transaction types.

### Define Procedure

- From **2022 onward**, the **statistical procedure** should generally no longer be relevant.

- To check or define entries, use **Transaction SPRO** with path:  
  *Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Define Procedure*,  
  or use **Transaction SM30** with object **/ECRS/V_T616**.

### Define Movement Code

- The **movement code** is specific to some countries (e.g., the Czech Republic).

- To define the **mode of transport code**, use **Transaction SPRO** with path:  
  *Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Define Movement Code*,  
  or **Transaction SM30** with object **/ECRS/V_TSMC**.

- **Figure A.10** shows an example of movement codes for the Czech Republic.

---

### Table A.1 Intrastat: Nature of Transaction Codes (Cont.)

| Code | Category                                               | Description                                                                                  |
|-------|-------------------------------------------------------|----------------------------------------------------------------------------------------------|
| **80** | Transactions involving the supply of building materials and technical equipment under a general construction or civil engineering contract for which no separate invoicing of the goods is required and an invoice for the total contract is issued | N/A                                                                                        |
| **91** | Other transactions that can’t be classified under other codes | Hire, loan, and operational leasing longer than 24 months                                    |
| **99** | Other                                                 |                                                                                            |

© 2022 by **Rheinwerk Publishing Inc., Boston (MA)**

---

### Define Mode of Transport at the Border

- Use **Transaction SPRO** and follow path:  
  *Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Define Mode of Transport at the Border*,  
  or use **Transaction SM30** with object **/ECRS/V_T618**.

- There are **nine mode of transport codes** used around the EU:

  1. **Sea transport**
  2. **Rail transport**
  3. **Road transport**
  4. **Air transport**
  5. **Postal consignments**
  7. **Stationary modes of transport**
  8. **Inland water transport**
  9. **Own propulsion**

- These codes must be mapped to all countries where your company files Intrastat reports.

- **Figure A.11** shows an example for the Czech Republic.

© 2022 by **Rheinwerk Publishing Inc., Boston (MA)**

---

### Define Port and Airport

- The definition of a specific **port or airport** combined with the mode of transport code is specific to a few countries.

- To define this, use **Transaction SPRO** at path:  
  *Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Define Port and Airport*,  
  or use **Transaction SM30** with object **/ECRS/V_TPRT**.

### Define Incoterms

- Define **all Incoterms** as per country requirements.

- Use **Transaction SPRO** with path:  
  *Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Define Incoterms*,  
  or use **Transaction SM30** with object **/ECRS/V_TINC**.

- Definition can differ by country; for example, the Czech Republic groups Incoterms, others use three-digit codes.

- See example in **Figure A.12**.

### Define Collection Center

- Definition of a **collection center** is specific to some countries.

- Use **Transaction SPRO** at:  
  *Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Define Collection Center*,  
  or **Transaction SM30** with object **/ECRS/V_TCCE**.

---

## A.1.2 Default Values

### Define Default Values for Sales

- Map the **item category** for a country to a **business transaction type** and, where relevant, to a statistical procedure or movement code.

- Use **Transaction SPRO** at:  
  *Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Default Values • Sales • Define Default Values for Sales*,  
  or **Transaction SM30** with object **/ECRS/V_TDVS**.

- Example (**Figure A.13**):  
  - Item category **TAN** is mapped to business transaction type **11** (final sale).  
  - Item category **TANN** is mapped to business transaction type **34** (free-of-charge supplies).

- Other columns might not be relevant for certain countries (e.g., Germany).

### Define Material Group for Intrastat

- You can create an **Intrastat group** for goods with similar Intrastat requirements.

- This will be assigned on the **material master** level.

- Use **Transaction SPRO** with path:  
  *Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Default Values • Sales • Define Material Group for Intrastat*,  
  or **Transaction SM30** with object **/ECRS/V_TVFM**.

- Example shown in **Figure A.14**.

### Define Default Values for Purchasing

- Map the combination of **country of declaration, purchasing document category, purchasing document type, and item category** to a business transaction type for receipts.

- Use **Transaction SPRO** at:  
  *Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Default Values • Purchasing • Define Default Values for Purchasing*,  
  or **Transaction SM30** with object **/ECRS/V_T161B**.

- Example (**Figure A.15**):  
  - Purch. Doc. Category **F** (purchase order)  
  - Purchasing Doc. Type **NB** (standard purchase order)  
  - Item category **0** (standard)  
  - Mapped to Bus. Trans. Type Receipts **11** (final purchase)

### Define Business Transaction Type for Returns to Supplier

- Map **receipt types** to **return types** for goods to be returned to a supplier.

- Use **Transaction SPRO** with path:  
  *Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Default Values • Purchasing • Define Business Transaction Type for Returns to Supplier*,  
  or **Transaction SM30** with object **/ECRS/V_TBTR**.

- Example (**Figure A.16**):  
  - Business transaction type **11** (final purchase) mapped to return type **21** (return of goods).

### Define Procedure for Returns to Supplier

- Similar to business transaction types, define **statistical procedures** for returns if relevant.

- Use **Transaction SPRO** at path:  
  *Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Default Values • Purchasing • Define Procedure for Returns to Supplier*,  
  or **Transaction SM30** with object **/ECRS/V_TPRR**.

---

### Define Default Values for Transportation Data for Dispatches

- Define **default transport data** on the sales side (e.g., mode of transport for specific country pairs).

- Use **Transaction SPRO** with path:  
  *Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Default Values • Transportation Data • Define Default Values for Transportation Data for Dispatches*,  
  or **Transaction SM30** with object **/ECRS/V_T617_2**.

- Example (**Figure A.17**):  
  - Goods from **Germany to Austria** always delivered via **road**.

### Define Default Transportation Data for Receipts

- Define transport data defaults on the **receipt side** similarly.

- Use **Transaction SPRO** with path:  
  *Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Default Values • Transportation Data • Define Default Transportation Data for Receipts*,  
  or **Transaction SM30** with object **/ECRS/V_T617_1**.

- Example shown in **Figure A.18**.

### Define Default Transportation Data for Stock Transfers

- Assign default values for **stock transfers** between your own plants.

- Use **Transaction SPRO** with path:  
  *Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Default Values • Transportation Data • Define Default Values for Transportation Data for Stock Transfers*,  
  or **Transaction SM30** with object **/ECRS/V_T161W_T**.

### Define Default Values for Place of Delivery

- Relevant for defining defaults for certain **Incoterms**, which influence **transport responsibility**, **indirect tax**, and the Intrastat report.

- Example (**Figure A.19**):  
  - Default place of delivery for **Incoterm EXW** (pick-up at supplier warehouse) is defined as another EU member state.

- Use **Transaction SPRO** with path:  
  *Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Default Values • Define Default Values for Place of Delivery*,  
  or **Transaction SM30** with object **/CCEE/SIFIINCP3**.

## A.1 Intrastat Report

### Define Exceptions for Inclusion and Exclusion of Country and Region
Remember how you defined the special **partner regions** earlier? Now, you can **include or exclude specific regions** from your Intrastat report.  
Use **Transaction SPRO** and follow the path:  
Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Data Selection Control • Define Exceptions for Inclusion and Exclusion of Country and Region,  
or use **Transaction SM30** with object **/ECRS/V_T609II**.  

In Figure A.20, we include **Monaco** and exclude the **Canary Islands** — a region of Spain that doesn’t belong to the **EU VAT regime**.  

### Define Exclusion of Sales Document Item Category
If you have special **item categories** used in your sales documents, such as **text items**, you can exclude them by using **Transaction SPRO** and following the path:  
Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Data Selection Control • Define Exclusion of Sales Document Item Category,  
or use **Transaction SM30** with object **/ECRS/V_TICX**.  

### Define Sales Document Item Category for Services
Here, you can define the **item categories for services**. These will be excluded from the Intrastat report, as the report is only relevant to track the movement of **goods inside the EU**.  
Use **Transaction SPRO** and follow the path:  
Governance, Risk and Compliance • International Trade • Intrastat • Basic Data • Data Selection Control • Define Sales Document Item Category for Services,  
or use **Transaction SM30** with object **/ECRS/V_TICS**.  

In Figure A.21, we’ve recorded item category **TAD**.  

### A.1.3 Provider of Information
Before you can run your Intrastat report, you need to create a **provider of information**. This is a **reporting entity** that represents a country where your company code is registered for **VAT**. It can be different from the company code or legal entity.  
Use **Transaction /ECRS/POI_EDIT**.  

Click the **Create icon**, and fill in the fields for your Provider of Info., as shown in Figure A.22. We’ve chosen a combination of the **company code** and **country**. Additionally, enter the **country of declaration** and the **company code** you’re creating the provider of information for.  

You’ll be forwarded to an overview populated by the information available for your company code in the declaration country. Fill in missing information — for this example, fill in the **State of Tax Office** and **Tax Number**, as shown in Figure A.23.  

Next, fill in the **basic settings** of your report. If you’re using a different **exchange rate** than the standard rate M, enter it in the **Exchange Rate Type** box, as shown in Figure A.24.  

For **Germany**, you only have two choices for the **receipt declaration level** (Receipt Decl. Level) and **dispatch declaration level** (Dispatch Decl. Level):  
- 0 (No Declaration)  
- 2 (Standard Declaration)  

Other countries may offer options such as an extended declaration. Also enter your **declaration format file** (Declar. File Format). You can extract the result as a Microsoft Excel file if you like, but in this case, we’ll create an **XML** file.  

Finally, for the provider of information—and definitely an important step—enter the **plants** that belong to this registration in their respective boxes (see Figure A.25). If you don’t enter the Plant, the transactions won’t show up in your Intrastat report.  

### A.1.4 Run Intrastat Report
Use **Transaction VE01** to run the Intrastat report for **dispatches**, as shown in Figure A.26. This runs a program in the background that collects all information on dispatches to be reported in the Intrastat report for the defined provider of information in a certain reporting period.  

A popup like Figure A.27 will show you how many **data entries** were found.  

Use **Transaction MEIS** to run the Intrastat report for **receipts**, as shown in Figure A.28. This runs a background program collecting all information on receipts for the defined provider of information in a certain reporting period.  

A popup similar to Figure A.29 will show how many **data entries** were found.  

Use **Transaction /ECRS/RP_EDIT** to manage your Intrastat report. Enter your provider of information to see the declarations you created, as shown in Figure A.30.  

You can click into the return—always split by **receipt** and **dispatch**—and view the details of the return in a certain period (Figure A.31).  

On the **item level**, you can see any **errors** or **missing information** that needs maintenance, as shown in Figure A.32. When all fields are filled, the **Correct** field is automatically checked.  

### Integration with SAP Global Trade Services
If **SAP Global Trade Services (SAP GTS)** is used, you can configure all this in **SAP GTS** as well. The applications always integrate; logistical data from SAP GTS is sent to the **SAP S/4HANA core**.  

---

## A.2 EC Sales and Purchase List

The **EC Sales List** and **EC Purchase List** are **reporting requirements** all over the EU. Each country has local names, for example:  
- **Zusammenfassende Meldung** in Germany and Austria  
- **VIES Return** in Ireland  

Their main purpose is to show **financial flows between member states**. Some key tax code settings and the standard report for the EC Sales/Purchase List are explained below.  

### A.2.1 Tax Code Settings
As described in Chapter 3, Section 3.1.1, the **EU code** determines whether a **tax code** must be considered for the EC Sales List/Purchase List. Important indicators used frequently in practice include:  

- If the **EU Code field** is blank, the tax code isn’t EU relevant or not relevant for EC Sales List/Purchase List.  
- **EU Code 1** is generally used for **intra-community supplies of goods**.  
- Difference between **EU code 1** and **4**:  
  - EU code 1 → goods  
  - EU code 4 → services  
- For supplies of services to customers in other EU countries, use **EU code 4**.  
- On purchasing side, the most relevant are **EU codes 9 and 5**:  
  - **EU Code 9** → **intra-community acquisitions of goods**  
  - **EU Code 5** → acquisition of services  
- **EU code A** indicates a transaction subject to **reverse charge** but **not relevant for the EC Purchase List**; used for example for **local reverse charge transactions**.  

### A.2.2 SAP Standard Report
Use **Transaction S_ALR_87012400** for the standard **EC Sales List/Purchase List report** in SAP S/4HANA.  

On the selection screen, the most important parameters are:  
- **Company code**  
- **Reporting country** (if using plants abroad)  
- **Extraction period** (you can choose date range, reporting quarter, or reporting period depending on your setup)  

Figure A.33 shows some available fields of the output, which can be adapted depending on country requirements.  

### EC Sales List/Purchase List with SAP Document and Reporting Compliance
**SAP Document and Reporting Compliance** (formerly SAP Advanced Compliance Reporting and SAP Document Compliance) offers functionality for the EC Sales List/Purchase List.  
For details on **advanced compliance reporting (ACR)**, refer to **Chapter 9**.  

---

## A.3 Standard Audit File for Tax

The **SAF-T file** is a standard data structure used widely across **Europe** to support **predefined data extraction**. Some countries use the SAF-T file as a **monthly reporting requirement**; others request it **on-demand** to support **tax audits**.  

A SAF-T file generally contains:  
- Information about the **taxpayer** creating the file  
- **Master data**  
- **Accounting data**  
- Can also include **inventory** and **transactional data**  

SAP offers one general SAF-T format and several predefined country-specific formats where SAF-T is mandatory with deviating format, such as:  
- **Portugal**  
- **Poland**  

Use **Transaction SPRO** and follow the path:  
Financial Accounting • General Ledger Accounting • Periodic Processing • Report.  

Here, you find the **Standard Audit File – Taxation menu** with:  
- **Activate Countries for SAF-T Data Extraction**  
- **Activate Countries for SAF-T File Generation**  

There is also a submenu for country-specific settings, for example for **Norway**. Settings for other countries are found further down in respective submenus in **Statutory Reporting**, for example:  
- Statutory Reporting: Portugal • Standard Audit File for Tax Purposes (SAFT-PT)  
- Statutory Reporting: Poland • SAF-T for Poland  

---

## Appendix B: India

The **Indian goods and services tax (GST)** is implemented within a **dual model**. Within the country, it includes:  
- **Central GST (CGST)** levied by the **central government**  
- **State GST (SGST)** or **Union Territory GST (UTGST)** levied by the **states or union territories**  

For **cross-state transactions**, **Integrated GST (IGST)** applies, equivalent to the sum of **CGST** and **SGST**.  

All available solution notes for GST in India can be found in **SAP Note 2435968**.  

### Example
- For goods sold **within the state**, e.g., Maharashtra, a combination of **CGST and SGST** applies.  
- For goods sold **between two states**, such as Maharashtra and Karnataka, **IGST** applies.  

The standard **tax procedure** for India is **TAXINN**. With **TAXINN**, the system determines the tax rate **not based on the percentage added via Transaction FTXP directly in the tax code, but based on conditions maintained via Transaction FV11**.  

This appendix briefly covers how **tax determination** works in India and how it differs from the SAP standard approach described in this book. It also includes pointers on **master data management** for tax-relevant master data and discusses **official document numbering (ODN)**.  

### B.1 Tax Determination
In tax determination, you’ll have multiple **financial accounting condition types** for the different types of **GST** that are part of the tax procedure **TAXINN**.  

These are split again depending on usage. For example, different financial accounting condition types and account keys exist for:  
- **Standard output GST**  
- **Nondeductible GST**  
- **Reverse charges**  
- **Import GST**  

The following sections take a closer look at the condition types and condition technique for **tax determination in India**.

## B India

### B.1.1 Condition Types

To view the **financial accounting condition types** listed in this section, use **Transaction OBQ1**. 

- **Table B.1** shows the **deductible input condition types**.
- **Table B.2** shows **nondeductible input condition types**.
- **Table B.3** shows **output tax condition types**.

All **input condition types** listed have **access sequence JGSI** (**IN: GST for Input Taxes**) assigned, and all **output condition types** listed have **access sequence JGSO** (**IN: GST for Output Taxes**) assigned.

| GST Type | SAP Condition Type | SAP Account Key | Description             |
|----------|--------------------|-----------------|-------------------------|
| CGST     | JICG               | JIC             | IN: Central GST         |
| SGST     | JISG               | JIS             | IN: State GST           |
| IGST     | JIIG               | JII             | IN: Integrated GST      |
| UTGST    | JIUG               | JIU             | IN: Union Ter. GST      |

**Table B.1 Deductible Input Condition Types**

| GST Type | SAP Condition Type | SAP Account Key | Description             |
|----------|--------------------|-----------------|-------------------------|
| CGST     | JICN               | NVV             | IN: Central GST – ND    |
| SGST     | JISN               | NVV             | IN: State GST – ND      |
| IGST     | JIIN               | NVV             | IN: Integrated GST – ND |
| UTGST    | JIUN               | NVV             | IN: Union Ter. GST – ND |

**Table B.2 Nondeductible Input Condition Types**

| GST Type | SAP Condition Type | SAP Account Key | Description             |
|----------|--------------------|-----------------|-------------------------|
| CGST     | JOCG               | JOC             | IN: Central GST – OP    |
| SGST     | JOSG               | JOS             | IN: State GST – OP      |
| IGST     | JOIG               | JOI             | IN: Integrated GST – OP |
| UTGST    | JOUG               | JOU             | IN: Union Ter. GST – OP |

**Table B.3 Output Tax Condition Types**

---

### B.1.2 Condition Technique

Tax is determined via **Transaction FV11**, generally used to maintain a **tax rate** determined for a certain **tax code**. Compared to **Transaction MEK1** or **Transaction VK11**, it enables determination of different **tax amounts** for a single tax code.

To view the **access sequences**, use **Transaction OBQ2**. In the tables discussed, the **region of the customer (REGIO)** and the **supplying plant (WKREG)** are always present. These indicate the **tax jurisdiction** and thus the **tax rate**.

Tax rate determination via **Transaction FV11** works together with the standard **condition technique** in sales and distribution and materials management but offers more flexibility for countries with multiple tax rates.

- For **output tax determination**, access sequence **JGSO** is used, with three tables:
  - 768 ALAND | WKREG | TAXK1 | TAXM1 | REGIO | STEUC
  - 775 WKREG | TAXK1 | REGIO | MATNR
  - 750 WKREG | REGIO | TAXK1

- For **input tax determination**, access sequence **JGSI** is used, also with three tables:
  - 792 ALAND | REGIO | WKREG | VEN_CLASS | TAXIM | STEUC
  - 790 REGIO | WKREG | VEN_CLASS | MATNR
  - 791 REGIO | WKREG | VEN_CLASS | SRVPOS

---

### B.2 Stock Transfers

In **India**, **cross-state stock transfers** are taxable transactions requiring tax determination and a tax invoice. 

Specific setup includes:

- Plants created as **business partners** with vendor and customer data, including **tax numbers**.
- Plant assigned in the **Vendor: General Data** tab in business partner data.
- Customer number assigned to a plant using **Transaction SPRO**, navigating path:  
  *Materials Management • Purchasing • Purchase Order • Set Up Stock Transport Order • Define Shipping Data for Plants*

SAP recommends creating an **info record**. You can also use the **condition technique** on the purchasing side to determine indirect tax.

Note: Similar to the **EU setup**, a separate **billing type** and **pricing procedure** are required.

For detailed information, refer to the appendix of **SAP Note 2474335**.

---

### B.3 Multiple Tax IDs

In India, the same customer or vendor often has **multiple GST IDs** across different states. Per **SAP Note 2514392**, maintaining multiple GST numbers for Indian business partners is **not possible**.

The recommended solution:

- Create as many **business partners** as there are **GST numbers**.
- This enables assignment of the **GST number to the region** for GST number determination.

---

### B.4 Official Document Numbering in Sales and Distribution

**Official Document Numbering (ODN)** is the SAP solution for legal requirements regarding **sequential numbering without gaps**. It is more flexible than standard document numbering.

Instructions for ODN according to GST rules in India are in **SAP Note 2491168**.

Steps to begin with ODN:

1. Maintain **document classes** via **Transaction SM30** and view **V_DOCCLS**. Enter country **"IN"**. Click **New Entries** and enter document classes. Each represents a distinct number range.
2. Assign the **billing type** and **document type** to the document class via **SM30** with view **J_1IG_V_T003_I**, choosing whether ODN is required for the combination.
3. Maintain the **number group** (specific number range) via **SM30** with view **J_1IG_V_NUMGRP**.
4. Assign the **number range group** to the combination of company code, chart of accounts, and document class via **SM30** with view **J_1IG_V_OFNUM**.
5. The generated **official document number** is found in table **VBRK**, field **XBLNR**.

---

## Appendix C  
### North America

North America, especially the **United States**, has a distinctive indirect tax system with no national-level indirect tax. Instead, **state and local city taxes** apply, resulting in approximately **13,000 taxing jurisdictions**, each with unique laws, rules, and procedures.

**Canada** combines **federal GST** with **provincial harmonized sales tax (HST)** or **Quebec sales tax**, depending on the province.

This appendix covers:

- Tax determination via **jurisdiction codes**
- Specialties of **tax codes** and **tax procedures** for SAP tax determination in North America

---

### C.1 Jurisdiction Codes

To set up **tax jurisdictions**, use **Transaction SPRO** and follow this path:

*Financial Accounting • Financial Accounting Global Settings • Tax on Sales/Purchases • Basic Settings • Specify Structure for Tax Jurisdiction Code*

Here you can:

- Define the structure of the **tax jurisdiction code** per tax procedure
- Create new structures with **New Entries**
- Specify the lengths for the first to fourth levels in the hierarchy

**Indicator Tx In** defines whether taxes are determined on a line-by-line basis.

**Example:** The jurisdiction codes of tax procedure **TAXUSX**:

- 2 digits for the **state code** (e.g., Alaska = AK)
- 3 digits for the **county code** (e.g., area code 907 = Anchorage County)
- 4 digits for the **city code** (e.g., ANCH = Anchorage)

To define the actual **tax jurisdiction codes**, use **Transaction SPRO** and navigate to:

*Financial Accounting • Financial Accounting Global Settings • Tax on Sales/Purchases • Basic Settings • Define Tax Jurisdictions*

---

### C.2 Tax Codes

Tax codes in North America are complex due to the **tax procedure**.

An example tax procedure can include four levels of taxes for each type:

- **Nondeductible input sales tax** (partial)
- **Deductible input sales tax**
- **Output sales and use tax**
- **Output sales tax**

These levels correspond to the different indirect tax levels determined at the jurisdiction level.

Refer to **Chapter 3, Section 3.3** for detailed tax determination.

**Transaction FV11** is used to maintain tax rates for codes and enables determination of different tax amounts for a single tax code.

Example tax procedure:

- **TAXUSX** shows a sales tax code with all levels having value **100**, representing federal, county, city, and others.

For **TAXCAJ**, the tax jurisdiction is directly shown on the tax code level, including a federal value with jurisdiction code **CA00** and provincial value with the province jurisdiction code.

---

### C.3 Tax Types

Due to multiple indirect tax types working together in North America, **pricing procedures** for Canada and the US offer several tax types.

The recommended best practice, especially in the US with frequent rate changes and many jurisdictions, is to use a **tax engine**.

#### RVAAUS (Standard USA without Jurisdiction Code)

- Pricing procedure **RVAAUS** is the standard US pricing procedure **without jurisdiction codes**.
- Used when the country (US) is assigned to **tax procedure TAXUS**, not TAXUSJ.
- It has three levels:
  - **State Sales Tax** with condition type **UTX1**
  - **County Sales Tax** with **UTX2**
  - **City Sales Tax** with **UTX3**

Condition categories:

- **UTX1**: Condition category **D** (Tax) — standard tax conditions
- **UTX2** & **UTX3**: Condition category **M** (Sales tax with license checking)

A **license check** is performed with customer master data; if a valid exemption license is found, condition records with customer tax classification "L" apply.

Condition record maintenance resembles **condition type MWST** via **Transaction VK12**, with options for departure/destination country, domestic options including country, state, county, customer tax classification, and material tax classification.

#### RVAJUS (Standard USA with Jurisdiction Code)

- Pricing procedure **RVAJUS** is the standard US pricing procedure **with jurisdiction codes**.
- Used when the country (US) is assigned to **tax procedure TAXUSJ**, not TAXUS.
- Condition types differ:
  - **UTXJ** has condition category **1** (TaxJurDic level 1) with license check
  - Acts as a **trigger condition** triggering other conditions
  - Sets the **tax code**, while tax values build in the following conditions

## Maintenance of Condition Records for UTX1

The **maintenance of condition records** is performed on a **country basis**. The condition table includes:

- **Tax departure country**
- **Region**
- **Customer and material tax classifications**
- A field for **license number and date**

The next levels, called **tax jurisdiction conditions**, are maintained via **Transaction FV12** on a country and tax code basis, not via **Transaction VK12**.

The **tax jurisdiction code** is stored in the **customer master data** and is used during pricing to identify the **tax value**.

If the pricing procedure used for external tax determination, **UTXJ**, has **formula 300** assigned in the field **alternative calculation type**, this indicates **external tax determination**.

---

## C North America

### RVAXUD (Standard USA / Tax per document)

In the pricing procedure **RVAXUD** (see Figure C.7), there are two condition types:

- **UTXD**
- **UTXE**

These must be used together for **technical reasons**. Both have a **formula value assigned** but **no access sequence** and are **group conditions**. 

**UTXD** uses **UTXJ** as a **reference condition type**, meaning it uses the same **condition records** as **UTXJ**.

---

### RVAACA (Standard - Canada)

The standard pricing procedure **RVAACA** for Canada (see Figure C.8) includes three tax types:

- **CTX1**: Standard GST on the **federal level**
- **CTX2**: Provincial tax **HST or QST**
- **CTX3**: Provincial tax based on the **gross value** of the tax base amount plus GST

It refers to step **911**, which is the **net value plus GST**.

All three condition types belong to **condition category D (Tax)**.

---

### RVAJCA (Standard - CA / With Jur.Code)

For Canada, like the United States, condition types differ (see Figure C.9). 

- **CTXJ** has **condition category 1 (TaxJurDic)**, **level 1** with **license check** as a **trigger condition**.

The **tax jurisdiction conditions** are maintained via **Transaction FV12** on a country and tax code basis. The **tax jurisdiction code** in the **customer master data** is used during pricing to identify the **tax value**.

---

## Appendix D: The Authors

- **Michael Fuhr**: Certified tax advisor with a business informatics background. Co-leads a German Big Four indirect tax technology practice delivering **end-to-end indirect tax solutions** for multinational and regional clients. Deputy lead of expert committee III of the Institute of Digitization for Taxes, shaping future developments in **real-time reporting** and **e-invoicing**.

- **Dirk Heyne**: Partner in the Big Four advising clients on the transformation and digitization of their tax function and its integration with finance. Focuses on supporting tax functions during **SAP S/4HANA** transformation projects, tax data management, and implementation of **BEPS 2.0** requirements. Certified tax advisor and public accountant with a PhD in Mathematics and Computer Science and two master’s degrees from the Technical University Munich.

- **Nadine Teichelmann**: Senior consultant in the EY indirect tax technology practice, Munich. Advises multinational clients on digitizing tax departments. Holds a degree in business informatics and is pursuing a PhD in tax technology. Focuses on technical and business process aspects of **indirect tax implementation projects** in **SAP S/4HANA**.

- **Jan Walter**: Tax partner in EY business tax practice, Munich. SAP S/4HANA expert supporting tax functions in transformation projects. Has over 15 years of experience in **tax operating model transformation**, **direct tax-enabled ERPs**, and tax compliance management. Project lead for tax technology and transformation projects and developer of SAP-based tax applications.

---

## Index (Selected Terms)

- **1+2 approach** .................................................. 299  
- **ABAP** .............................................................. 262, 263  
  - programs ......................................................... 265  
  - statements ....................................................... 265  
- **ABC contract** .................................................... 74  
- **Access requirement** ................................. 176, 185, 197  
  - enhancements ................................................. 276  
- **Access sequence** ...................... 165, 175, 182, 184, 463  
  - 0003 .................................................................. 188  
  - add condition table ........................................ 186  
  - define .................................................................. 184  
  - exclusive ............................................................ 185  
  - MWST .................................................... 185, 188  
  - transfer pricing ............................................ 362, 364  
- **Account assignment** ................................ 142, 315  
  - categories ......................................................... 142  
- **Account determination** .................................. 178  
- **Account key** ......................................................... 180  
- **Account mapping** ............................................... 334  
- **Account screening** ............................................ 326  
- **Account solution** ................................................ 296  
- **Account split** ....................................................... 308  
- **Accounting document** ......................... 162, 164, 169  
- **Accounting principle** ........................................... 297  
- **Accounts payable** ................................................ 204  
- **Accruals** .................................................................... 180  
- **Accumulation type** .............................................. 101  
- **Actual cost** ................................................................ 33  
- **Add-on** ................................................................. 253, 428  
- **Adjustment account** ............................................ 296  
- **Administration** ...................................................... 380  
- **Advance return** ........................................... 128, 130  
- **Advanced compliance reporting (ACR)** ........ 398  
  - documentation .................................................. 399  
  - set up ..................................................................... 399  
- **Airport** ....................................................................... 449  
- **Allocation rule** ....................................................... 370  
- **Amendment** ............................................................... 79  
- **Amortization** ............................................................. 30  
- **Analytical review** .................................................. 327  
- **Analytics** ......................................................... 293, 372  
  - direct tax ............................................................. 326  
- **Annual tax return** ................................................. 109  
- **Append structure** ............................... 195, 269, 367  
  - add ......................................................................... 195  
- **Application design** ............................................... 341  
- **Application manager** ............................................. 32  
- **Application programming interface (API) integration** .................................................... 39  
- **Application scenario** ........................................... 342  
- **Arm’s length principle** ....................... 89, 350, 351  
- **Asset** ................................................................... 93, 321  
- **Audit** ....................................... 91, 292, 295, 380, 414  
  - ensure readiness ................................................. 92  
  - log .......................................................................... 255  
  - post ........................................................................ 383  
- **Audit-to-return adjustment** ............................ 295  
- **Automated digital services** ................................. 89  
- **Automation** ....................... 19, 21, 39, 44, 58, 308, 426  
- **Availability level** ...................................................... 39  
- **Avalara AvaTax** ..................................................... 258  

---

- **Balance carryforward** .......................................... 304  
- **Balance sheet** ........................................ 33, 296, 297  
  - accounts .............................................................. 314  
  - commercial versus tax .................................. 301  
  - select items ......................................................... 307  
- **Balance Sheet/Income Statement – Multidimensional app** ................................... 375  
- **Base amount** .......................................... 25, 101, 122  
- **Base calculation** ....................................................... 98  
- **Base Erosion and Profit Shifting (BEPS)** ......... 86  
  - 2.0 .......................................................... 88, 349, 354  
- **Base ledger** .............................................................. 298  
- **Basic compliance reporting** ............................. 398  
- **Basic settings** ............................................................. 97  
- **Batch input** ............................................................. 388  
- **Bill of materials (BOM)** ................................ 248, 359  
- **Billing** ........................................................................... 34  
  - date .............................................................. 166, 243  
  - quantity ............................................................... 214  
- **Billing document** ......................................... 164, 212  
  - delivery-related ................................................ 234  
  - exchange rate ................................................... 133  
  - issue output ....................................................... 172  
  - user exits .................................................... 274, 275  
  - VAT IDs ....................................................... 152, 214  
- **Billing plan** .............................................................. 240  
  - milestone billing .............................................. 242  
  - periodic billing .................................................. 244  
  - pricing .................................................................. 243  
- **Billing plan type** .................................................... 241  
  - assign .................................................................... 242  
  - maintain .............................................................. 241  
- **Billing type** .............................................................. 283  
  - credit notes ......................................................... 274  
- **Bill-to party** ............................................................. 146  
- **Brazil** .............................................................. 39, 66, 68  
- **Brownfield approach** ..............................................

## B

- **Business add-in (BAdI)** 262, 368  
- **Tax reporting date** 115  
- **Business case** 29, 51  
- **Business intelligence (BI)** 341  
- **Business partner** 34, 137, 145, 286, 318  
  - choose role 146  
  - internal 286  
  - maintain 34  
  - plant 228  
  - roles for tax ID 145  
  - supplier 229  
  - type 35  
  - WHT 318  
- **Business process** 58  
- **Business process document (BPD)** 52  
- **Business Rule Framework plus (BRFplus)** 338  
- **Business transaction type** 445  
  - define 447  
  - returns to supplier 451  
- **Business unit segmentation** 353  
- **Business vendor** 202  

## C

- **Calculation rule** 166  
- **Calculation type** 183  
- **Calling and exiting program unit** 265  
- **Canada** 39, 465, 470  
- **Cash discount** 101  
- **Cash flow optimization** 416  
- **Central Finance** 23, 49  
- **Central GST (CGST)** 461  
- **Centralized exchange model** 85  
- **Certificate of exemption** 104  
- **Chain transaction** 74, 150, 216, 217  
  - first entrepreneur 218  
  - second entrepreneur 220  
  - special 75  
  - third entrepreneur 225  
- **Change of ownership** 446  
- **Chart of accounts** 94, 113, 312  
  - account descriptions 315  
  - overview 316  
  - requirements 313  
- **Chart of depreciation** 321  
- **Check routine** 328, 330, 419  
- **Checking rule** 44, 146, 230  
- **Class** 256  
- **Classic coding block extension** 366  
- **Clearance model** 84  
- **COBL structure** 366  
- **Coding block** 323, 366, 368, 369  
- **Collection center** 449  
- **Commercial invoice** 68  
- **Communication header** 267, 284  
- **Communication item** 268  
- **Company code** 119, 122, 123, 310  
  - assign 392  
  - currency 127  
  - display 312  
  - fiscal year variants 302  
- **Comparable uncontrolled price (CUP) method** 351  
- **Compliance** 354, 355, 373, 379  
  - costs 414  
  - digital model 380  
  - global model 384  
  - management 417  
  - reporting 398  
  - risks 414  
  - show log 437  
  - tasks 437  
- **Compliance check** 427  
  - create 430  
  - hits 435  
  - task lists 431  
- **Compliance Check Hits app** 434  
- **Compliance Results Overview app** 429, 437  
- **Compliance scenario** 432  
  - run 434  
- **Concur Expense** 207  
- **Concur Invoice** 208  
- **Concur Travel** 207  
- **Condition amount** 180, 183  
- **Condition base value** 180  
- **Condition category** 183  
- **Condition class** 182  
- **Condition logic** 175, 251  
- **Condition record** 165, 178, 190, 191, 210  
  - create 189  
  - plants abroad 124  
- **Condition table** 176, 184, 185, 187, 244, 268, 285  
  - add to access sequence 186  
  - assign fields 187  
  - create 190  
  - create data element 191  
  - nonmaintainable materials 203  
  - purchasing 188  
  - requirements 197  
  - sales and distribution 188  
  - status of country 269  
  - technical view 191  
- **Condition technique** 38, 175, 251, 268, 463  
- **Condition type** 105, 156, 175, 176, 181, 210  
  - assign to countries 157  
  - AZWR 243  
  - billing plans 243  
  - control data 182  
  - create/copy 182  
  - CTX1 470  
  - CTX2 470  
  - CTX3 470  
  - CTXJ 470  
  - custom 138  
  - define 181  
  - DIFF 180, 182  
  - display for tax procedure 105  
  - financial accounting 166  
  - formulas 182  
  - GRWR 43, 178  
  - India 462  
  - intra-community stock transfers 234  
  - manual entries 184  
  - MWST 177, 180, 182, 183  
  - plants abroad 124  
  - pricing 132, 158  
  - pricing discounts 246  
  - sales and distribution 165  
  - settings 177, 179  
  - statistical 178  
  - transfer pricing 362  
  - TTX1 135, 184, 210  
  - US 468, 469  
  - UTX1 468  
  - UTX2 468  
  - UTX3 468  
  - UTXD 470  
  - UTXE 470  
  - UTXJ 469  
  - WHT 102  
  - WIA1 124  
  - WIA2 124  
  - WIA3 124  
  - YZWR 243  
- **Condition value** 183  
- **Conditional rebate** 247  
- **Construction provider** 137, 138, 271  
- **Consumer tax (CT)** 66  
- **Consumer-facing business** 89  
- **Content manager** 31  
- **Continuous transaction controls (CTC)** 80, 81, 83, 383, 408, 413  
  - decentralized 85  
- **Contract type** 244  
- **Control** 101, 259, 420, 421, 423, 424  
  - data 105, 116  
- **Conversion** 261  
  - rule 131  
- **Coordination** 350  
- **Copying control** 213  
  - billing documents 132  
  - settings 214  
  - STO for delivery 231  
- **Core data services (CDS) view** 287, 328, 329, 425  
- **Corporate income tax** 86  
- **Co-sourcing** 384  
- **Cost allocation**

## Costing and Related Concepts

- **Cost center allocation** ......................................... 360  
- **Cost uplift** ....................................................... 359, 360  
- **Costing** ...................................................................... 358  
- run ......................................................................... 360  
- variants ............................................................... 359  
- **Cost-plus method** .............................. 351, 357, 358  

## Country and Tax Reporting

- **Country of declaration** .............................. 442, 443  
- **Country of origin** .................................................. 443  
- assign ................................................................... 443  
- exceptions .......................................................... 444  
- **Country-by-country reporting (CbCR)** ......................................... 87, 351, 374, 375  
- **Country-independent tax** ................................. 165  

## Systems and Tools

- **Coupa** ........................................................................ 206  
- **Created-on date** ..................................................... 168  

## Accounting and Transactions

- **Credit account** ................................................ 99, 104  
- **Credit note** .............................................................. 274  
- **Credit posting** ........................................................ 360  
- **Cross-border goods movement** ...... 63, 120, 124  
- Cross-border supplies of goods ...................... 244  
- Cross-border transaction ..................................... 76  
- Cross-company transaction ............................. 226  

## Currency and Exchange

- **Currency** .................................................................. 126  
- assign multiple ................................................. 127  
- exchange rates ................................................. 126  
- translation ................................................ 129, 131  
- types ...................................................................... 127  

## Customization and Extensions

- **Custom coding** ...................................................... 261  
- **Custom Fields and Logic app** .................. 366, 367  

## Customer Management

- **Customer** ............................................... 137, 144, 208  
- create invoice ..................................................... 224  
- exempt .................................................................. 140  
- license ................................................................... 271  
- master record ....................................................... 34  
- projects ................................................................. 281  
- tax classification .............................................. 137  
- **VAT ID** ................................................................... 420  

## Customer Exits

- **Customer exit** ............................................... 262, 280  
- **EXIT_SAPLMEKO_001** .................................... 284  
- **EXIT_SAPLMEKO_002** ................................... 287  
- identify ................................................................. 281  

## Customer Tax Classification

- **Customer tax classification** .................... 137, 164  
- examples ............................................................. 139  
- maintain .............................................................. 138  
- vendor WHT ....................................................... 144  

## Customs and Country Specifics

- **Customs clearance** ............................................... 446  
- **Czech Republic** ............................................. 447, 449  

## Dashboard and Data Management

- **Dashboard template** ............................................ 342  
- **Data capturing** ....................................................... 292  
- **Data element** ................................................ 191, 269  
  - add ......................................................................... 194  
  - add to field catalog ......................................... 196  
  - create .................................................................... 194  

## Data Structure and Tools

- **Data flow** ..................................................................... 24  
- **Data layer** ................................................................. 257  
- **Data Medium Exchange (DME) Engine** ........ 394  
- **Data processing statement** ............................... 266  
- **Data quality** ...................................................... 56, 326  
- **Data replication** ........................................................ 44  
- **Data Retention Tool (DART) catalog** ................ 25  
- **Data type** .................................................................. 194  

## Data-based Tax Management and Date

- **Data-based tax management** ........................... 415  
- **Date category** .......................................................... 241  

## Accounting Accounts

- **Debit account** .................................................. 99, 104  

## Tax Models and Statements

- **Decentralized CTC and exchange model (DCTCE)** ................................................................... 85  
- **Declarative statement** ........................................ 265  

## Default Values and Delivery

- **Default value** .......................................................... 449  
  - place of delivery ................................................ 452  
  - purchasing .......................................................... 450  
  - sales ....................................................................... 449  
  - transportation data ........................................ 451  

## Deferred Tax and Liability

- **Deferred tax** .................................................. 113, 335  
- liability .................................................................... 93  

## Delivery Management

- **Delivery** ................................................. 164, 212, 224  
  - billing documents ............................................ 234  
  - block ...................................................................... 240  
  - place ...................................................................... 452  
  - type ......................................................................... 230  

## Financial Postings and Ledgers

- **Delta posting** ........................................................... 296  
- **extension ledger** ................................................ 298  
- **ledger groups** ...................................................... 301  

## Demand and Deployment

- **Demand management** ........................................ 205  
- **Deploy phase** ............................................................. 54  
- **Deployment** .............................................................. 47  

## Depreciation and Design

- **Depreciation area** .................................................. 321  
- target ledger groups ........................................ 322  
- **Design workshop** ..................................................... 52  

## Principles and Controls

- **Destination principle** ...................................... 69, 71  
- **Detective control** ................................................... 420  

## Determinations and Development

- **Determination extension** .................................. 151  
- **Determination rule** .............................................. 149  
  - examples .............................................................. 152  
  - maintain .............................................................. 150  
- **Development package** ......................................... 194  

## Digital Compliance and Reporting

- **Digital compliance model** ................................. 380  
- **Digital reporting requirements (DRRs)** ... 80, 379  
  - drivers, problems, and effects ........................ 82  
  - EU .............................................................................. 82  
- **Digital tax administration** ................................... 18  

## Tax Allocations and Planning

- **Direct allocation** .................................................... 370  
- **Direct tax** ................................................... 57, 86, 291  
  - basics ..................................................................... 291  
  - data analytics and monitoring ................... 326  
  - filing ....................................................................... 332  
  - lifecycle ......................................................... 57, 307  
  - master data .............................................. 294, 317  
  - organizational structures ......... 294, 295, 311  
  - planning ............................................................... 340  
  - process .................................................................. 292  
  - reporting ...................................................... 93, 332  
  - tax tagging .......................................................... 322  

## Discounts and Project Phases

- **Discount** .......................................................... 122, 246  
- **Discover phase** ......................................................... 51  

## Dispatch and Declaration

- **Dispatch** .................................................................... 451  
  - declaration level ............................................... 455  
  - run report ............................................................ 455  

## Sales and Distribution

- **Distribution channel** ................................. 181, 226  
- **Division** ..................................................................... 181  

## Document Management

- **Document class** ...................................................... 464  
- **Document date** ............................... 162, 167, 388  
- **Document type** ...................................................... 229  
  - assign .................................................................... 230  
  - define ..................................................................... 229  

## Documentation and Domain

- **Documentation** ...................................................... 258  
- **Domain** ............................................................ 191, 194  
  - create ..................................................................... 192  
  - value range ......................................................... 192  

## Domestic Tax

- **Domestic tax** ................................................. 185, 188  

## Double Taxation and Payments

- **Double taxation** ....................................................... 72  
- **Down payment** ...................................................... 235  
  - calculate tax ...................................................... 237  
  - connect receipt to request ............................ 239  
  - create request .................................................... 237  
  - general ledger accounts ................................ 236  
  - link to standard account ............................... 236  
  - receipt ......................................................... 238, 240  

## Drop Shipment

- **Drop shipment** ............................................... 74, 217  
  - inside US .............................................................. 220  

## Electronic Processes and Reporting

- **E-accounting** ........................................................... 382  
- **E-assess** ..................................................................... 382  
- **E-audit** ....................................................................... 382  
- **EC Purchase List** ..................................................... 459  
  - standard reports ............................................... 394  
  - tax code ................................................................ 112  
  - tax code settings .............................................. 459  
- **EC Sales List** ............................................... 63, 79, 459  
  - print ....................................................................... 397  
  - set up ..................................................................... 407  
  - standard reports ............................................... 394  
  - tax code ................................................................ 112  
  - tax code settings .............................................. 459  

## Economic and Educational Concepts

- **Economic link** ........................................................... 72  
- **Education** .................................................................... 71  
- **Efficiency** .................................................................. 415  

## Electronic Filing and Invoicing

- **E-file** ........................................................................... 382  
- **E-invoicing** ..................................... 19, 45, 80, 82, 83  

## Electronic Data Interchange (EDI)

- **Electronic Data Interchange (EDI)** .......... 85, 198, 246, 411  
  - add mapping ..................................................... 199  

## Electronic Documents

- **Electronic document (eDocument)** ............... 408  
  - connected to customer solution ................ 410  
  - connected to partner solution .................... 411  
  - connected to tax authority ..........................

## E

- **Electronic filing** .................................................................. 78  
- **Electronic Road Transportation Control System (EKAER)** ...................... 381  
- **E-match** ........................................................................ 382  
- **Embedded analytics** .......................................................... 293, 329, 371, 424  
  - dashboard ........................................................... 330  
  - versus **SAP Tax Compliance** ........................................... 331  
- **Enhancement** ................................................................ 261, 368  
  - assignments ................................................................. 282  
  - implementations ............................................... 115, 266, 281  
  - overview ...................................................................... 281  
  - purchasing ............................................................... 288  
  - sales and distribution ................................................ 276  
- **Equity account** .................................................................. 314  
- **EU code** ....................................................................... 113, 459  
- **EU Triangular Deal** ........................................................ 220  
- **European E-invoicing Service Providers Association (EESPA)** ....................... 80  
- **European Union (EU)** ........................................................ 62, 69, 74, 79, 407, 441  
  - digital reporting requirements ....................................... 82  
  - **EC Sales and Purchase List** ........................................ 459  
  - **Intrastat report** ........................................................... 441  
  - legal requirements ............................................................ 80  
  - **SAF-T** ......................................................................... 460  
- **European VAT Code** ........................................................... 270  
- **Event** .............................................................................. 68  
- **tax exempt** .................................................................... 70  
- **Exception** ...................................................................... 453  
- **Exchange rate** ............................................................ 126, 455  
  - advance return ................................................................ 130  
  - assign to country ........................................................... 130  
  - basic settings ................................................................... 127  
  - conversion rules ............................................................ 131  
  - define ............................................................................ 130  
  - determine ...................................................................... 116  
  - import from XML file .................................................... 130  
  - maintain ......................................................................... 129  
  - sales and distribution .................................................... 131  
  - types ........................................................................... 127, 129  
- **Exemption** ..................................................................... 70, 140, 218, 311, 468  
  - categories ........................................................................ 70  
  - licenses ........................................................................... 271  
  - reason ............................................................................ 103, 110  
  - rules ................................................................................ 72  
  - **VAT** ............................................................................ 234  
- **Explore phase** .................................................................. 52  
- **Export** .......................................................................... 71  
  - tax ................................................................................. 186, 188  
- **Extendable interface** ...................................................... 255  
- **Extension ledger** .......................................................... 298  
  - fiscal year variants ........................................................ 303  
  - posting period variants ................................................... 304  
- **External group number** ................................................ 406  
- **External partner** ............................................................. 254  
- **External system** .............................................................. 280  
- **External tax calculation** ................................................. 39  
  - engine .......................................................................... 253, 256  
- **External tax determination** .......................................... 254  

## F

- **Factory calendar** .............................................................. 400  
- **Field catalog** .................................................................. 196  
- **Field control** ................................................................... 430  
- **Field status group** .......................................................... 319  
  - subgroups ...................................................................... 319  

**Personal Copy for Yun Kyoung Oh, ohleo199@gmail.com**

## Field status variant

- ............................................................ 319  

## Filing

- .................................................................. 77, 293, 312, 332  
  - electronic ....................................................................... 78  

## Financial accounting

- ............................................................................... 32, 34, 97, 160, 163, 166, 239  
- **WHT** ......................................................................... 170  
- **Financial link** ................................................................ 72  
- **Financial reporting** ...................................................... 93  
- **Financial statement** .................................................. 291, 375  
  - annual ......................................................................... 295  
- **First entrepreneur** ....................................................... 218  
- **Fiscal year variant** ...................................................... 302  
- **Fit-to-standard analysis** ................................................ 52  
- **Fixed ledger** .................................................................. 297  
- **Flash title** ...................................................................... 220  
- **Forecast planning** ........................................................ 341  
- **Form routine** ................................................................ 172  
- **Function module** ..................................................... 264, 282  

## G

- **General ledger** ........................................................... 296, 297  
  - assessment ................................................................... 369  
- **General ledger account** ........................................... 116, 236, 319, 388  
  - customer-specific fields ............................................. 320  
  - ledger groups ................................................................ 301  
  - tax relevant .................................................................. 116  
  - transaction types ....................................................... 320  
  - **VAT** ......................................................................... 118  
- **Generally Accepted Accounting Principles (GAAP)** ............ 93, 296, 307  
- **Germany** ...................................................................... 444, 455  
- **Gift** ................................................................................ 70  
- **Global Anti-Base Erosion (GloBE)** .................................. 90  
- **Global enterprise** .......................................................... 87  
- **Global tax management** ................................................ 36  
- **Goods and services tax (GST)** ................................. 64, 413  
- **ID** .................................................................................. 464  
- **India** ............................................................................. 461  
- **Goods receipt** ............................................................ 160  
- **Governance and organization** ..................................... 28  
- **Greece** .......................................................................... 383  
- **Greenfield approach** ...................................................... 49  
- **Gross margin** .............................................................. 358, 361  
- **Group condition** .......................................................... 183  
- **Group number** ............................................................. 406  
- **Group valuation** ........................................................... 360  
- **Grouping** ....................................................................... 72  
- **Guided buying** ........................................................... 205  
- **Gulf Cooperation Council** ............................................ 64  

## H

- **Historical data** ............................................................. 26  
- **Hungary** ........................................................................ 381  
- **Hypercare** ..................................................................... 54  

## I

- **Identification** ................................................................ 146  
- **If condition** ................................................................... 270  
- **Immediate Supply of Information System (SII)** ..................... 110  
- **Implementation project** .......................................... 22, 26, 28, 258  
- **SAP Activate** ................................................................ 51  
- **Import** .......................................................................... 141  
- **Importation of goods** ................................................... 69  
- **Imposto sobre Circulaçao de Mercadorias e Serviços (ICMS)** ....... 67  
- **Imposto sobre Produtos Industrializados (IPI)** ....................... 67  
- **Imposto Sobre Serviços (ISS)** ............................................ 67  
- **Includes** ....................................................................... 264  
- **Income inclusion rule** .................................................. 90  
- **Income tax** ..................................................................... 86  
  - reporting ........................................................................ 93  
- **Incoterm** ................................................................. 216, 225, 267, 273  
  - define ........................................................................... 449  
- **India** ................................................................ 64, 461  
  - condition types ................................................................ 462  
  - multiple IDs .................................................................... 464  
  - **ODN** .......................................................................... 464  
  - stock transfers ................................................................ 463  
  - tax determination .......................................................... 461  
- **Indirect allocation** ............................................. 369, 370  
- **Indirect tax** ................................................ 19, 61, 175  
  - ABAP basics .................................................................. 262  
  - chain transactions ......................................................... 225  
  - content and maintenance ............................................. 258  
  - custom coding ................................................................ 261  
  - data flow ....................................................................... 416  
  - digital reporting requirements ................................. 80  
  - engines and add-ons .................................................. 251  
  - gap ................................................................................ 19  
  - global systems ............................................................ 61  
  - jurisdictions ......................................................... 107, 157  
  - lifecycle ......................................................................... 27  
  - monitoring .................................................................. 424  
  - registration ................................................................... 42  
  - returns .......................................................................... 78  
  - special jurisdictions ....................................................... 66  
  - special transactions ..................................................... 217  
  - standard reporting requirements ............................... 76  
  - tax codes ...................................................................... 109  
  - tax procedures ............................................................ 105  
  - tax processes ................................................................ 55  
  - tax rate .......................................................................... 109  
  - values abroad ................................................................ 43  
- **Indirect tax determination** .................................. 157, 175  
  - condition logic in SAP .................................................. 175  
  - customizing scenarios ........................................... 202, 215  
  - purchasing ......................................................................

## I

- **sales and distribution** ..................................... 208  
- **Inline declaration** ................................................. 264  
- **Input tax** ......................................... 62, 124, 184, 202  
- **Insourcing** ................................................................ 384  
- **Intangible goods** ...................................................... 68  
- **Integration** ..................................................... 160, 254  
- **layer** ....................................................................... 257  
- **templates** ............................................................. 254  
- **Intercompany billing** .......................................... 226  
- **Intercompany profit** ............................................ 364  
- **Intercompany stock transfer** ................. 228, 231  
- **Intercompany transaction** ................................ 351  
- **Interface** ................................................. 255, 261, 280  
- **Internal group number** ...................................... 406  
- **Internal number range** ....................................... 275  
- **Internal stock transfer** ........................................ 227  
- **International Financial Reporting Standards (IFRS)** ......................................... 93, 296  
- **International tax order** .......................................... 88  
- **International trade** .................................................. 31  
- **Interoperability model** .......................................... 84  
- **Intra-community supply of goods** ........... 63, 71  
- **Intracompany stock transfer** ........................... 228  
- **Intra-group transaction** ........................................ 73  
- **Intrastat** ............................................ 31, 79, 178, 441  
  - **basic settings** ..................................................... 441  
  - **default values** .................................................... 449  
  - **manage report** .................................................. 457  
  - **material group** .................................................. 450  
  - **provider of information** ................................ 453  
  - **report** .................................................................... 148  
  - **run report** ............................................................ 455  
  - **view return** .......................................................... 457  
- **Intrastate acquisition** ............................................. 70  
- **Investment** ................................................................. 30  
- **Invoice** .......................................... 161, 164, 212, 381  
  - **Brazil** ........................................................................ 68  
  - **create** .................................................................... 224  
  - **date** .............................................................. 162, 167  
  - **electronic** ................................................................ 80  
  - **numbering** .......................................................... 215  

### Invoice (Cont.)

- **posting** ........................................................ 101, 201  
- **receipt** ................................................................... 224  
- **tax-relevant** ....................................................... 240  
- **texts** ...................................................................... 216  
- **user exits** ............................................................. 274  
- **Invoice-credit method** .......................................... 64  
- **IT expert** ...................................................................... 28  
- **Item category** ......................................................... 221  
  - **assign** ................................................................... 221  
  - **assign billing plan types** ............................... 242  
  - **billing relevance** ............................................... 125  
  - **kit items** ............................................................... 248  
  - **sales document** ................................................. 453  
  - **services** ................................................................. 453  
  - **stock transfers** .................................................. 229  
  - **TAN** ........................................................................ 450  
- **Item condition** ...................................................... 184  

## J

- **JAGGAER** .................................................................. 206  
- **Japan** ...................................................................... 64, 68  
- **Journal entry** .......................................................... 352  
  - **extend** .................................................................. 366  
  - **item** .............................................................. 366, 369  
- **Jurisdiction** ...................................................... 39, 107  
  - **chain transactions** ............................................. 74  
  - **codes** ..................................................................... 465  
  - **define** .................................................................... 466  
  - **define codes** ....................................................... 108  
  - **exemptions** ........................................................... 72  
  - **GST** ........................................................................... 64  
  - **indirect tax** ......................................................... 157  
  - **reporting requirements** ................................. 381  
  - **requirements** ..................................................... 122  
  - **requirements gathering** ................................... 52  
  - **sales tax** .................................................................. 65  
  - **special indirect tax** ............................................. 66  
  - **tax rates** .............................................................. 109  

## K

- **Key performance indicator (KPI)** .......... 346, 371  
- **Key user extensibility** ......................................... 366  
- **Kit** ............................................................................... 248  
- **KOMK structure** .............. 266, 267, 281, 283, 284  
- **KOMKAZ structure** ........................... 194, 266, 267  
- **KOMP structure** ........................ 266, 281, 283, 287  
- **KOMPAZ structure** ................... 194, 195, 266, 269  

## L

- **L-account** .................................................................. 296  
- **Leader** ........................................................................... 28  
- **Ledger** ................................................................. 94, 295  
  - **balance carryforward** ..................................... 304  
  - **business operations** ........................................ 305  
  - **deactivate** ........................................................... 300  
  - **depreciation areas** ........................................... 322  
  - **extension** ............................................................. 298  
  - **fiscal year variant** ............................................ 302  
  - **groups** ................................................................... 301  
  - **parallel** ................................................................. 297  
  - **posting period variant** ................................... 304  
  - **set up** ..................................................................... 299  
  - **solution** ...................................................... 296, 297  
  - **standard** .............................................................. 297  
- **Legacy system** ........................................................... 50  
- **Legal entity** .............................................................. 311  
- **Legal valuation** ....................................................... 360  
- **License** ....................................................................... 271  
- **Local currency** .............................................. 126, 127  
  - **advance return** .................................................. 128  
- **Local file** ................................................... 87, 351, 373  
- **Location-based supply** ........................................ 217  
- **Logistical data** ........................................................ 381  

## M

- **Maintenance** .............................................................. 54  
- **Making Tax Digital (MTD)** ................................. 386  
- **Malaysia** ....................................................................... 66  
- **Manage Compliance Checks app** .......... 329, 430  
- **Manage Compliance Scenarios app** ..... 329, 432  
- **Manage Launchpad Spaces and Pages app** .... 32  
- **Manage Substitution and Validation Rules app** ............................................................. 368  
- **Manage Task List Templates app** .......... 435, 437  
- **Management transfer price** .............................. 350  
- **Market price** ............................................................ 358  
- **Market segment** .......................................... 367, 369  
- **Market state** ............................................................... 89  
- **Master data management** .................................... 53  
- **Master data screening** ......................................... 326  
- **Master file** ............................................... 87, 351, 373  
- **Material** ..................................................................... 209  
  - **group** ..................................................................... 450  
  - **movement** .............................................................. 33  
- **Material Ledger** ............................................... 33, 360  
- **Material master** ..................................................... 359  
- **Material tax classification** ....................... 133, 136  
  - **examples** ............................................................. 139  

### Material tax classification (Cont.)

- **maintain** .................................................... 135, 136  
- **purchasing** .......................................................... 136  
- **sales and distribution** ..................................... 134  
- **Materials management** .............................. 33, 160  
- **Medical care** ............................................................... 71  
- **Microsoft Excel** ...................................................... 296  
- **Migration** .................................................................... 47  
  - **strategies** ............................................................... 48  
  - **template approach** ............................................ 50  
- **Milestone billing** .......................................... 240–242  
- **Mini One Stop Shop (MOSS)** ............................. 113  
- **Mitigation** ................................................................ 421  
  - **action** .................................................................. 426  
- **Mode of transport** ................................................. 448  
- **Modularization statement** ................................ 265  
- **Monaco** ...................................................................... 443  
- **Monitor Compliance Scenario Runs app** ..... 329  
- **Monitoring** ................................................ 44, 56, 413  
  - **business partners** .............................................. 318  
  - **direct tax** .................................................... 293, 326  
  - **indirect tax** .......................................................... 424  
  - **requirements** ...................................................... 417  
  - **solutions** ............................................................... 418  
  - **tax movements** ................................................. 314  
  - **transfer pricing** ........................................ 353, 364  
- **Movement code** ..................................................... 447  
- **Multinational enterprise (MNE)** ................. 88, 89  
- **Multiplatform** ......................................................... 259  
- **Multiyear project report** ..................................... 354  
- **My Compliance Check Results app** ...... 329, 434  
- **My Compliance Tasks app** ................................. 437  
- **myDATA** .................................................................... 383  

## N

- **Natural person** ........................................................

## 202

- **Nature code** ..................................................................... 110  
- **New implementation** ............................................ 49  
- **Nexus** .......................................................................... 75, 88  
- **Nondeductible VAT** .............................................. 118  
- **Nonroutine profit** ................................................... 89  
- **Nonstock item** ........................................................ 136  
- **North America** ........................................................ 465  
  - jurisdiction codes ............................................. 465  
  - tax codes .............................................................. 466  
  - tax types ............................................................... 468  
- **Northern Ireland** ......................................... 442, 443  
- **Number range** ........................................................ 275  

© 2022 by Rheinwerk Publishing Inc., Boston (MA)

## 483  
### Index - O

- **Object** ......................................................................... 263  
- **OECD Committee on Fiscal Affairs (CFA)** ....... 91  
- **Official document numbering (ODN)** ........................ 215, 464  
- **Online validation** .................................................. 148  
- service ...................................................................... 43  
- **OpenText** ................................................................. 204  
- **Operational transfer pricing** .................. 349, 350  
  - automate ............................................................. 356  
  - challenges ........................................................... 355  
- **Optical character recognition (OCR)** ............. 204  
- **Order-to-cash** ..................... 59, 163, 175, 280, 421, 423  
  - example ............................................................... 165  
- **Organization for Economic Cooperation and Development (OECD)** ..................... 64, 350  
- **Organizational link** ................................................. 72  
- **Origin principle** ........................................................ 70  
- **Outbound delivery** ..................................... 164, 231  
  - create .................................................................... 232  
  - number ................................................................. 233  
- **Output control** ............................................. 388, 396  
- **Output device** ......................................................... 397  
- **Output tax** ...... 110, 114, 124, 137, 177, 184, 236  
  - down payments ................................................ 239  
- **Output type** ............................................................. 171  
- **Outsourcing** ............................................................ 384  
- **Overhead cost controlling** ................................ 369  

## P

- **Pan-European Public Procurement OnLine (Peppol) network** ................................. 80  
- **Parallel accounting** ............................................... 296  
- **Parallel currencies** ................................................... 33  
- **Parallel ledgers** ....................................................... 297  
- **Parallel valuation** .................................................. 360  
- **Partner country** ..................................................... 442  
  - assign .................................................................... 442  
  - exceptions for conversion ............................ 443  
- **Partner solution** ....................................................... 39  
- **Partner type** ............................................................ 199  
- **Partnership** .............................................................. 312  
- **Payer** ....................................................... 146, 149, 152  
  - no VAT ID ............................................................ 156  
- **Payment** ................................................................... 381  
- **PDF output** .............................................................. 170  
- **Periodic billing** ............................................. 240, 241  
  - billing plan .......................................................... 244  
- **Periodic tax return** ............................................... 109  
- **Periodic transaction controls (PTC)** .......... 80, 81  
- **Periodicity** ............................................................... 400  
- **Permanent establishment abroad** ................ 311  
- **Place of delivery** .................................................... 452  
- **Planning** .......................................................... 294, 340  
  - scenarios ............................................................. 341  
- **Plant** ......... 119, 141, 179, 209, 227, 287, 455, 463  
  - assign customer data .................................... 228  
  - assign to supplier ............................................ 229  
  - define tax indicators ...................................... 141  
  - requesting ........................................................... 160  
  - supplying ................................................... 164, 231  
- **Plants abroad** .......... 119, 120, 127, 180, 189, 234  
  - activate ................................................................ 121  
  - non-EU ................................................................. 124  
  - pricing procedure ............................................ 181  
  - pros and cons .................................................... 120  
  - settings ....................................................... 121, 123  
  - tax procedures .................................................. 105  
  - VAT registration ............................................... 123  
- **Plausibility check** ................................................. 421  
- **Poland** .......................................................................... 44  
- **Port** ............................................................................. 449  
- **Posting date** ............................................................ 162  
- **Posting key** ....................................................... 99, 104  
- **Posting period variant** ....................................... 304  
- **Predictive analytics** ............................................. 341  
- **Prefix** ......................................................................... 266  
- **Prepare phase** ........................................................... 52  
- **Preventive control** ............................................... 420  
- **Price condition** ...................................................... 362  
  - create .................................................................... 364  
- **Pricing** .................................................... 260, 266, 283  
  - carry out new pricing ..................................... 273  
  - conditions ........................................................... 157  
  - date .................................................... 131, 132, 168  
  - discount ............................................................... 246  
  - exchange rate type ......................................... 132  
  - material master ............................................... 359  
  - routines ............................................................... 276  
  - structure ..................................................... 267, 268  
  - tax determination ........................................... 156  
  - transfer pricing ................................................. 352  
  - type ........................................................................ 214  
- **Pricing communication structure** ................ 187  
  - add data elements ........................................... 194  
- **Pricing procedure** ..................... 157, 175, 176, 180  
  - analysis ................................................................ 210  
  - Canada ................................................................. 470  
  - condition types ........................................ 177, 179  
  - create .................................................................... 181  
  - define .................................................................... 180  
- **Personal Copy for Yun Kyoung Oh, ohleo199@gmail.com**  

## Index - continued

- **Pricing procedure (Cont.)**  
  - document ............................................................ 181  
  - India ...................................................................... 461  
  - intercompany billing ...................................... 226  
  - pricing discounts .............................................. 246  
  - RVAA01 ................................................................. 176  
  - RVAAUS ................................................................ 180  
  - RVWIA1 .............................................. 180, 181, 234  
  - transfer pricing ....................................... 361, 364  
  - US ................................................................. 468–470  
- **Print control** ........................................................... 397  
- **Print type** ................................................................. 179  
- **Priority rule** ............................................................. 149  
  - examples ............................................................. 152  
- **Private vendor** ....................................................... 202  
- **Procedure** ................................................................. 447  
  - returns to supplier ........................................... 451  
- **Process expert** ........................................................... 28  
- **Processing layer** ..................................................... 257  
- **Procurement** ................................................. 160, 204  
- **Procure-to-pay** .......... 58, 160, 175, 288, 416, 422  
  - example ............................................................... 161  
- **WHT** ....................................................................... 316  
- **Profit allocation** ........................................................ 88  
- **Profit and loss (P&L)** ......................... 360, 365, 369  
  - accounts .............................................................. 314  
- **Profit markup** ......................................................... 360  
- **Profit split** ................................................................ 363  
- **Profitability analysis** ........................................... 178  
  - custom characteristics ......................... 367–369  
- **Program** .................................................................... 265  
  - flow logic ............................................................. 266  
  - MV45AFZB ........................................................... 273  
  - MV45AFZZ ........................................ 266, 267, 269  
  - print output ........................................................ 172  
  - RFIMPNBS ........................................................... 130  
  - RFTBFF00 ............................................................ 130  
  - RV60AFZZ ................................................. 266, 274  
  - V05EZZAG ........................................................... 151  
  - V05EZZRG ........................................................... 151  
- **Program of Social Integration (PIS)/Contribution for the Financing of Social Security (COFINS)** ................................... 67  
- **Proof of delivery** .................................................... 226  
- **Provider of information** ..................................... 453  
- **Provisions report** .................................................. 353  
- **Public interest** ........................................................... 71  
- **Puerto Rico** ................................................................. 39  
- **Purchase contract** ................................................. 200  
- **Purchase info record** ................................. 199, 283  
  - create .................................................................... 199  
  - tables ..................................................................... 284  
- **Purchase order** ................................... 160, 200, 381  
  - create ..................................................................... 222  
  - document types .................................................

## Index (229-486)

### Nonmaintaining Materials
- Page 203

### Reference
- Page 224

### Purchase Requisition
- Pages 160, 200, 222, 223

### Purchasing
- Page 160
- **Access sequences** — Page 184
- **BAdIs** — Page 262
- **Basic settings** — Page 133
- **Condition records** — Page 189
- **Condition tables** — Pages 188, 190
- **Condition types** — Page 181
- **Customer exits** — Page 280
- **Customizing scenarios** — Page 202
- **Default values** — Page 450
- **Indirect tax determination** — Page 198
- **Organizations** — Page 119
- **Pricing procedures** — Page 176
- **SAP Ariba** — Page 205
- **Tax determination parameters** — Page 289
- **Tax indicator examples** — Page 143

### Query
- Page 338

### Realize Phase
- Page 52

### Reallocation
- Page 89

### Real-time Analysis
- Pages 329, 425

### Real-time Reporting
- Pages 19, 45, 81, 110, 380
- **Model** — Page 84

### Rebate
- Page 246

### Recapitulative Statement
- Page 81

### Receipt
- Page 452
- **Declaration level** — Page 455
- **Run report** — Page 456
- **Type** — Page 451

### Receiver
- Page 370

### Reconciliation Account
- Page 236
- **Link** — Page 236

### Record-to-report
- Pages 60, 420

### Recurrence
- Page 432

### Recurring Cost
- Page 30

### Redetermination
- Page 273

### Reduced Rate Product
- Page 140

### Refund
- Page 79

### Region
- Page 444
- **Conversion** — Page 445

---

© 2022 by **Rheinwerk Publishing Inc., Boston (MA)**

---

### Registration for Indirect Taxation Abroad (RITA)
- Page 42

### Regulation
- Page 83

### Remote Function Call (RFC) Interface
- Page 39

### Report
- Page 261
- Categories — Pages 400, 402
- RFASLD20 — Page 395
- RFASLM00 — Page 397
- RFBILA00 — Page 375
- RFUMSV00 — Pages 120, 126, 261, 386
- RFUMSV10 — Page 393
- RV80GHEN — Page 277
- Transfer pricing — Page 354

### Reporting
- Pages 45, 57, 109, 260, 379
- **BEPS** — Page 87
- **Compliance** — Page 398
- **Compliance models** — Page 384
- **Country** — Pages 113, 125
- **Date** — Page 162
- **Digital requirements** — Page 80
- **Direct tax** — Pages 293, 332
- **EC Sales List** — Page 407
- **Entities** — Page 401
- **Financial** — Page 93
- **Intrastat** — Page 441
- **Maturity** — Page 381
- **Standard** — Page 385
- **Standard requirements** — Page 76
- **Tax requirements** — Page 93
- **Transfer pricing** — Pages 354, 371
- **Worldwide requirements** — Page 379
- **Reporting & Simulation app** — Page 373

### Repository Browser
- Page 255

### Requirement
- Pages 179, 197

### Requirements Gathering
- Page 52

### Resale Price
- Pages 358, 361
- **Method** — Page 351

### Resale-minus Method
- Pages 358, 361

### Residence
- Page 225
- **Country** — Page 155

### Return Type
- Page 451

### Returns to Supplier
- Page 451

### Reverse Charge
- Pages 63, 111, 137, 138

### RISE with SAP
- Page 27

### Risk
- Pages 414, 421, 423, 424
- **Analysis** — Page 417
- **Assessments** — Pages 419, 422
- **Management** — Page 21
- **Reporting** — Page 353

### Rounding Rule
- Pages 101, 183

### Routine
- Pages 197, 276
- **Activate** — Page 277

### Routine Profit
- Page 89

### Routing
- Page 204
- **In** — Page 323
- **Out** — Page 323

### Run Advanced Compliance Reports App
- Page 340

### Run Compliance Reports App
- Page 31

### Run Compliance Scenario App
- Pages 329, 434

### Run Phase
- Page 54

---

## Sales and Distribution

### Sales and Distribution Overview
- Page 163

### Access Sequences
- Page 184

### BAdIs
- Page 263

### Basic Settings
- Page 133

### Condition Records
- Page 189

### Condition Tables
- Pages 188, 190

### Condition Types
- Page 181

### Customizing Scenarios
- Page 215

### Dates
- Page 166

### Exchange Rates
- Page 131

### Indirect Tax Determination
- Page 208

### Material Tax Classification
- Page 134

### ODN
- Page 464

### Pricing Procedures
- Page 176

### Tax Determination Parameters
- Page 280

### Transfer Pricing
- Page 362

### User Exits
- Page 266

### WHT (Withholding Tax)
- Pages 102, 170

---

## Sales and Service Tax (SST)
- Page 66

### Sales Area
- Page 180

### Sales Document Type
- Page 131

### Sales List
- Page 79

### Sales Order
- Pages 163, 208, 245
- Addresses — Page 209
- Create — Page 221
- Invoices — Page 212
- User Exits — Page 267

### Sales Organization
- Pages 119, 180

### Sales Tax
- Pages 65, 70, 467
- Group — Page 391

---

## SAP Technologies and Platforms

### SAP Activate
- Pages 26, 51

### SAP Analytics Cloud
- Pages 331, 340, 372, 425
- Connect tax data — Page 343
- Create a story — Page 342
- Implement tax scenarios — Page 344
- Tax forecasting — Page 345
- Tax monitoring — Page 346

### SAP Application Interface Framework
- Pages 410-412

### SAP Ariba
- Page 205
- SAP Ariba Contracts — Page 206
- SAP Ariba Discount Management — Page 206
- SAP Ariba Invoice Management — Page 206
- SAP Ariba Sourcing — Page 205
- SAP Ariba Spend Analysis — Page 206
- SAP Ariba Supplier Lifecycle and Performance — Page 206
- SAP Ariba Supply Chain Collaboration for Buyers — Page 205

### SAP Business Technology Platform (SAP BTP)
- Pages 253, 409

### SAP Business Warehouse (SAP BW)
- Page 425

### SAP BW/4HANA
- Pages 371, 426

### SAP Cloud Integration for Data Services
- Page 253

### SAP Concur
- Page 207

### SAP CoPilot
- Page 32

### SAP Document and Reporting Compliance
- Pages 31, 45, 293, 337, 384, 398, 408, 460
- Create a report — Page 337
- Document definition — Page 338
- Features — Page 46
- Parameters — Page 338
- Queries — Page 338
- Run report — Page 340
- Set up (incomplete reference)

## Index of SAP and Tax-Related Topics

### General SAP Terms and Modules

- **SAP ERP** ........................................................................ 49  
- **SAP Fiori** ...................................................................... 31  
- **SAP Fiori apps reference library** .......................... 329  
- **SAP Fiori launchpad** ................................................ 31  
- **SAP Global Trade Services (SAP GTS)** ..... 31, 458  
- **SAP HANA** ................................... 23, 32, 37, 44, 49, 370  
- **SAP Integration Suite** ................ 39, 253, 254, 409  
- **SAP Localization Hub, tax service** .................. 253  
- **SAP Profitability and Performance Management** ....... 333, 363, 369, 372  
  - process activities .............................................. 333  
  - tax reporting elements .................................. 334  

### SAP S/4HANA

- **SAP S/4HANA** ................ 17, 21, 23, 30, 47, 50, 97, 254, 292  
  - business processes ...................................... 55, 58  
  - CDS ......................................................................... 328  
  - chain transactions ................................. 219, 220  
  - company codes ................................................. 310  
  - database table ...................................................... 32  
  - deployment options ........................................... 47  
  - embedded analytics .............................. 329, 424  
  - extensibility .............................................. 366, 368  
  - implementation projects .................. 22, 28, 51  
  - ledgers ........................................................ 297, 305  
  - migration ............................................................... 47  
  - migration strategies .......................................... 48  
  - monitoring ......................................................... 413  
  - on-premise ............................................................ 47  
  - reporting .......................................... 332, 379, 385  
  - simplification list ................................................ 30  
  - solution grouping ............................................... 37  
  - special payment processes ........................... 235  
  - tax capabilities .................................................... 36  
  - tax data management ..................................... 25  
  - tax determination .............................................. 38  
  - tax operating model ......................................... 22  
  - tax processes ........................................................ 26  
  - time dependency ................................................. 40  
  - transfer pricing .................................................. 371  
  - WHT ....................................................................... 317  

### SAP S/4HANA Cloud Editions

- **SAP S/4HANA Cloud** ............. 40–42, 47, 231, 254  
  - extensibility ........................................................ 368  
- **SAP S/4HANA Cloud, private edition** .............. 47  

### SAP Tax and Compliance Solutions

- **SAP solutions for global tax management** ................................................. 36, 37  
- **SAP Tax Compliance** ............ 29, 37, 44, 293, 328, 418, 426, 429  
  - access tax-relevant data ................................ 428  
  - add-on ................................................................... 428  
  - applications ........................................................ 328  
  - navigation ............................................................. 44  
  - versus embedded analytics .......................... 331  
- **SAP tax data model** .................................... 421, 429  

### Other Important SAP Terms

- **Scalability** ................................................................. 259  
- **Schedule line category** ........................................ 222  
- **Second entrepreneur** ................................. 219, 220  
- **Segmented business margin reporting** ........ 353  
- **Segmented tax margin reporting** ................... 353  
- **Selective data transition** ....................................... 49  
- **Self-billing** ................................................................ 246  
- **Self-supply** .................................................................. 70  
- **Seller privilege tax (SPT)** ....................................... 66  
- **Sender** ........................................................................ 370  
- **Service tax** ................................................................ 111  
- **Shipping** .................................................................... 232  
  - condition .............................................................. 216  
  - Ship-to address ................................... 209, 215, 227  
  - Ship-to party .............................. 145, 149, 209, 219  
- **VAT ID** ................................................................... 152, 153  
- **Sidecar approach** ................................................... 428  
- **Simplification list** .................................................... 30  
- **Simulation** ........................................... 354, 371, 372  
- **Single entity principle** ......................................... 229  
- **Sixth Directive** .......................................................... 63  
- **Smart forms** ............................................................. 170  
  - display ................................................................... 170  
- **Social security** ........................................................... 71  

### Sold-to Party

- **Sold-to party** .............................. 145, 149, 209, 219  
- **VAT ID** ................................................................... 153  

### Financial and Tax Terms

- **Source currency** ..................................................... 128  
- **Spacing** ...................................................................... 265  
- **Special general ledger indicator** ...................... 237  
- **Spend management** ............................................ 206  
- **Standard Audit File for Tax (SAF-T)** .... 19, 81, 91, 293, 400, 460  
- **Standard dimension** ............................................ 365  
- **Standard ledger** ........................................... 297, 299  
  - overview ............................................................... 299  
- **Standard rate product** ......................................... 139  
- **Standard setup** ....................................................... 252  
- **Standardization** ................................................ 23, 84  
- **State GST (SGST)** .................................................... 461  
- **Statement** ................................................................ 265  
- **Statutory reporting** ................................ 45, 57, 398  
- **Statutory-to-tax adjustment** ............................ 307  
  - account split ....................................................... 308  
  - approve and post ............................................. 309  
  - create rules ......................................................... 308  
  - profit impact ...................................................... 309  
- **Step** ............................................................................. 176  

### Stock and Logistics

- **Stock transfer** ............................................... 180, 227  
  - billing scenarios ................................................ 234  
- **India** ...................................................................... 463  
- **Intra-community in EU** .................................. 234  
- **Item categories** ................................................. 229  
- **Legal entities** ....................................................... 229  
- **Non-EU** .................................................................. 124  
- **Transport data** ................................................... 452  
- **Stock transport order (STO)** .............................. 228  
  - create .................................................................... 231  
  - document types ................................................ 230  
  - outbound deliveries ........................................ 231  
- **Storage location** ..................................................... 119  

### Miscellaneous

- **Story** ........................................................................... 342  
- **Structure** ......................................................... 263, 266  
- **Subject to tax rule** .................................................... 90  
- **Subroutine** ..................................................... 265, 277  
- **Substituição Tributária (ICMS-ST)** ..................... 67  
- **Subtotal** ..................................................................... 179  
- **Subtraction method** ............................................... 65  
- **Superclass** ................................................................ 256  
- **Supplier interaction** ............................................. 206  
- **Supplier management** ........................................ 205  
- **Supply of goods** ........................................................ 68  
- **Supply of services** .......................................... 69, 215  
- **Supplying plant** ..................................................... 231  
- **Switch-over rule** ....................................................... 90  
- **System conversion** .................................................. 49  

## Important SAP Tables

- **ACDOCA** ....................................................... 32, 366  
- **BKPF** ......................................................................... 33  
- **BSEG** ...................................................................... 394  
- **BSET** ...................................................................... 387  
- **EINA** ...................................................................... 284  
- **EINE** ....................................................................... 284  
- **EKKO** ..................................................................... 284  
- **EKPO** ..................................................................... 284  
- **KNA1** ...................................................................... 284  
- **KNV1** ...................................................................... 273  
- **KNVL** ..................................................................... 271  
- **KOMK** ................................................................... 194  
- **KOMP** ................................................................... 194  
- **LFA1** ....................................................................... 284  
- **MATDOC** ................................................................ 33  
- **SKA1** ....................................................................... 316  
- **T004** ...................................................................... 315  
- **T005** ............................................................. 279, 443  
- **VBAK** .................................................. 266, 271, 273  
- **VBAP** ............................................................ 266, 271  
- **VBPA** ..................................................................... 266  
- **XVBAK** .................................................................. 266  
- **YVBAK** .................................................................. 267  

## Task Lists

- **Task list** ..................................................................... 431  
  - create .................................................................... 435  
  - manage ................................................................ 437  
- **Task template** ......................................................... 436  

## Tax Account and Administration

- **Tax account** ..................................... 26, 94, 113, 116  
  - assign tax categories ..................................... 116  
  - assign tax codes ............................................... 118  
- **Chart of accounts** ............................................. 315  
  - descriptions ........................................................ 315  
  - types ...................................................................... 313  
- **VAT** ........................................................................ 118  
- **Tax administration** ....................... 83, 87, 380, 414  
- **Tax advisor** ................................................................. 28  
- **Tax amount** ..................................................... 25, 201  
- **Tax audit** ........................................ 91, 292, 295, 380  
  - ensure readiness ................................................. 92  
- **Tax authority** ...................................................................

## Index of Tax Topics

- **Tax balance sheet** ................................................. 296  
- **Single source of truth** ..................................... 297  
- **Versus commercial** .......................................... 301  
- **Tax box structure** ................................................. 404  
- **Tax calculation** ......................................................... 38  
  - External .................................................................. 39  
  - Time-dependent ............................................ 40–42  
- **Tax calendar** ........................................................... 400  

---

## Tax Categories and Classifications

- **Tax category** ......................................... 116, 157, 236  
  - Assign to countries .......................................... 157  
  - Pricing procedures ........................................... 157  
- **Tax codes** .............................................................. 117  
- **Tax classification** ................................. 34, 133, 260  
  - Condition table .................................................. 186  
  - Customer .............. 137, 164, 208, 272, 284–286  
  - Examples ............................................................. 139  
  - Material ......................... 133, 209, 217, 270, 274  
  - Overwrite ............................................................. 274  
- **Tax code** ............................................ 26, 97, 116, 187  
  - Assign multiple .................................................. 118  
  - Availability .......................................................... 260  
  - Basic WHT .............................................................. 98  
  - Central repository ................................................ 50  
  - Deletion ................................................................... 42  
  - Design .................................................................... 109  
  - EC Sales/Purchase List ................................... 459  
  - Enter chart of accounts .................................. 113  
  - EU ........................................................................... 113  
  - Extended WHT ................................................... 103  
  - Foreign ..................................................................... 42  
  - Groups ......................................................... 117, 402  
  - Inactive ................................................................. 113  
  - Indirect tax ......................................................... 109  
  - Invoices ................................................................ 201  
  - Maintain .............................................................. 110  
  - Mapping .................................................... 402, 403  
  - Mapping with EDI ............................................ 198  
  - North America ................................................... 466  
  - Properties ............................................................ 111  
  - Purchase orders ................................................ 201  
  - Purchasing organization data .................... 199  
  - Reporting countries ......................................... 125  
  - Sales orders ......................................................... 211  
  - Service tax ........................................................... 111  
  - Settings ................................................................. 110  
  - Table .......................................................................... 40  
  - Target .................................................................... 113  
  - Time-dependent .................................... 39, 41, 42  
  - Transfer ................................................................. 163  

---

## Tax Compliance and Control

- **Tax compliance** ........................................................ 36  
- **Tax officer** ....................................................................... 28  
- **Tax control framework** ...... 28, 44, 414, 419, 421  

---

## Tax Data and Analytics

- **Tax data** .................................................................... 134  
  - Model ................................................. 341, 421, 429  
  - Recording ................................................................ 55  
  - Structure ................................................................. 25  
  - Warehouse .............................................................. 24  
- **Tax data analytics** ....................................... 293, 326  
  - Real-time .............................................................. 329  
- **Tax data management** ................................ 24, 293  
  - Business partners ................................................ 34  
  - Operational ........................................................... 25  

---

## Tax Decisions, Deduction, and Destination

- **Tax decisions** ............................................................. 55  
- **Tax deduction** ......................................................... 414  
- **Tax departure country** .......... 142, 164, 179, 188, 191, 209, 227, 268  
- **VAT ID** ................................................................... 154  
- **Tax destination country** ....... 124, 151, 160, 215, 227, 268–270  
  - Adapt ........................................................... 215, 217  
  - VAT ID ................................................................... 155  

---

## Tax Determination

- **Tax determination** .......................... 38, 53, 55, 258  
  - Categories ............................................................ 258  
  - Customer exits ................................................... 280  
  - Direct tax .............................................................. 292  
  - External ................................................................ 254  
  - India ....................................................................... 461  
  - Integration .......................................................... 160  
  - Matrix ..................................................................... 55  
  - Pricing ................................................................... 156  
  - Purchasing data ................................................ 289  
  - Sales and distribution data ........................... 280  
  - User exits .............................................................. 266  

---

## Tax Engine and Exemption

- **Tax engine** ......................... 253, 256, 259, 323, 468  
  - Examples .............................................................. 257  
  - Layers ..................................................................... 257  
- **Tax exemption** ....................................................... 197  

---

## Tax Filing and Forecasting

- **Tax filing** ................................. 37, 77, 293, 312, 332  
- **Tax forecasting** ....................................................... 345  

---

## Tax Function

- **Tax function** ................................................ 17, 26, 47  
  - Capabilities ............................................................ 36  
  - Challenges and requirements ........................ 18  
  - Chart of accounts .............................................. 312  
  - Digitalization ........................................................ 18  
  - Direct tax .............................................................. 291  
  - End-to-end processes ......................................... 55  
  - Environment ......................................................... 27  
  - Planning ............................................................... 342  
  - Realize phase ......................................................... 52  
  - Risk management ............................................... 21  
  - Tax ledger concept ............................................ 306  
  - Tax tagging .......................................................... 323  
  - User interface ........................................................ 31  
  - Value creation ...................................................... 20  
  - Value drivers ....................................................... 414  

---

## Tax Gap and Tax Groups

- **Tax gap** ........................................................................ 77  
- **Tax group** ......................................... 72, 73, 392, 402  
  - Assign to tax boxes .......................................... 406  
  - Version .................................................................. 402  

---

## Tax Identification Number

- **Tax identification number** ............ 145, 164, 169, 208, 286  
  - Country-specific checks .................................. 146  
  - Determination ................................................... 279  
  - Duplicate checks ............................................... 148  
  - Maintain .................................................... 146, 148  
  - Multiple ................................................................ 464  
  - Transfer ................................................................. 163  
  - View in financial accounting ....................... 169  

---

## Tax Invoice and Jurisdiction

- **Tax invoice** ................................................................. 68  
- **Tax jurisdiction code** ........................................... 108  

---

## Tax Law and Ledger

- **Tax law** ...................................................................... 311  
- **Tax ledger** .......................................................... 94, 295  
  - Balance carryforward ..................................... 304  
  - Benefits ................................................................. 306  
  - Business operations ........................................ 305  
  - Deactivate ........................................................... 300  
  - Depreciation areas ........................................... 322  
  - Fiscal year variant ............................................ 302  
  - Groups ................................................................... 301  
  - Lifecycle maintenance .................................... 306  
  - Posting period variant ................................... 304  
  - Set up ..................................................................... 299  
- **Tax Ledger Hub app** ................................... 306, 310  

---

## Tax Liability and Lifecycle

- **Tax liability** ................................................................. 93  
- **Tax lifecycle** ................................................... 295, 307  

---

## Tax Monitoring

- **Tax monitoring** ................................ 44, 45, 56, 413  
  - Indirect tax ......................................................... 424  
  - Requirements ...................................................... 417  
  - Solutions .............................................................. 418  

---

## Tax Number and Validation

- **Tax number category** .......................................... 148  
- **Tax number validation** ....................................... 202  

---

## Tax Operating Model

- **Tax operating model** ............................. 22, 47, 419  
  - Business case ......................................................... 29  
  - Governance ............................................................ 28  
  - Infrastructure and architecture .................... 23  
  - Tax data management ...................................... 24  
  - Tax processes ......................................................... 26  
  - Tax talents .............................................................. 27  

---

## Tax Payable and Planning

- **Tax payable posting** ............................................. 407  
- **Tax planning** .............................................................. 57  

---

## Tax Position and Procedures

- **Tax position account** ........................................... 313  
- **Tax procedure** ................................................. 98, 110  
  - Assign to country ............................................. 105  
  - Define .................................................................... 105  
  - Indirect tax ......................................................... 105  
  - Multiple ................................................................ 108  
  - Preassigned ........................................................ 107  
  - Without plants abroad ................................... 105  

---

## Tax Process

- **Tax process** ......................................................... 26, 55  

---

## Tax Rate

- **Tax rate** ............................... 39, 41, 86, 99, 110, 463  
  - Changes ................................................................... 41  
  - Indirect ................................................................. 109  
  - Maintain ............................................................. 334  
  - Minimum ...............................................................

## Index

### Retroactive Maintenance  
- **Page:** 42  

### Validity Periods  
- **Page:** 41  

### Tax Record Evidence  
- **Page:** 383  

### Tax Reporting  
- **Pages:** 20, 24, 45, 57, 379  

### Compliance Models  
- **Page:** 384  

### Country  
- **Page:** 126  

### Date  
- **Page:** 388  

### Indirect  
- **Page:** 42  

### Maturity  
- **Page:** 381  

### Standard  
- **Page:** 385  

### Worldwide Requirements  
- **Page:** 379  

### Tax Reporting Date  
- **Pages:** 114, 162  

### Determine  
- **Page:** 115  

### Tax Requirement  
- **Pages:** 25, 26, 55  

### Tax Return  
- **Pages:** 78, 312  

### Annual  
- **Page:** 295  

### Monthly  
- **Page:** 391  

### Tax Rule  
- **Page:** 309  

### Tax Service  
- **Page:** 253  

### Calculate Taxes  
- **Page:** 255  

### Extend  
- **Page:** 256  

### Tax Solution Grouping  
- **Page:** 37  

### Tax Subject  
- **Page:** 311  

### Tax Tagging  
- **Pages:** 94, 315, 320, 322  

### Detect WHT Transactions  
- **Page:** 324  

### Standardize Postings  
- **Page:** 323  

### Steps  
- **Page:** 323  

### Tax Talent  
- **Page:** 27  

### Tax Technology Hybrid  
- **Page:** 28  

### Tax Template  
- **Page:** 50  

### Tax Transfer Price  
- **Page:** 350  

### Documentation  
- **Page:** 354  

### Segmentation  
- **Page:** 353  

### Tax Type  
- **Pages:** 24, 36, 112, 199  

### End-to-End Processes  
- **Page:** 26  

### Lifecycles  
- **Page:** 27  

### North America  
- **Page:** 468  

### Tax Validation  
- **Page:** 33  

### Tax Valuation Area  
- **Page:** 322  

### Taxable Event  
- **Pages:** 68, 70, 76, 323  

### Taxable Profit  
- **Page:** 86  

### Taxing Rights  
- **Page:** 88  

### Taxpayer  
- **Pages:** 380, 381  

### Tax-Sensitized Account  
- **Page:** 314  

### Teamwork  
- **Page:** 415  

### Template Approach  
- **Page:** 50  

### Template Transaction Matrix  
- **Page:** 50  

### Testing  
- **Pages:** 53, 354, 373  

---

### Third Entrepreneur  
- **Pages:** 219, 225  

### Third-Party Order  
- **Page:** 221  

### Third-Party Process  
- **Page:** 220  

### Thomson Reuters ONESOURCE Determination  
- **Page:** 258  

### Till Fiscalization  
- **Page:** 381  

### Time Dependency  
- **Page:** 40  

### Activate  
- **Page:** 41  

### Time-Dependent Tax  
- **Page:** 109  

### TKOMK Structure  
- **Page:** 266  

### TKOMP Structure  
- **Page:** 266  

### Tolerance  
- **Page:** 113  

### Tooling  
- **Page:** 75  

---

## Transaction

### /ECRS/POI_EDIT  
- **Page:** 453  

### /ECRS/RP_EDIT  
- **Page:** 457  

### BP  
- **Pages:** 35, 137, 145, 202, 228  

### CA03  
- **Page:** 359  

### CK11N  
- **Page:** 360  

### CK13N  
- **Page:** 360  

### CK40N  
- **Page:** 360  

### CMOD  
- **Page:** 281  

### CS01  
- **Page:** 248  

### CS03  
- **Page:** 359  

### DMEE  
- **Page:** 394  

### EDOC_BACKGROUND  
- **Page:** 409  

### EDOC_COCKPIT  
- **Pages:** 408, 409, 411  

### EDOC_COMPLETE  
- **Page:** 409  

### EDOC_INBOUND_MSG  
- **Page:** 408  

### EDOC_INBOUND_UPLOAD  
- **Page:** 409  

### EDOC_SUMMARY  
- **Page:** 409  

### F-29  
- **Page:** 238  

### F-37  
- **Page:** 237  

### FAGLGVTR  
- **Page:** 304  

### FB01  
- **Page:** 353  

### FB03  
- **Pages:** 162, 163, 169  

### FB50L  
- **Pages:** 301, 323  

### FB60L  
- **Page:** 324  

### FC10  
- **Page:** 375  

### FINSC_LEDGER  
- **Page:** 127  

### FK03  
- **Page:** 35  

### FOTV  
- **Pages:** 40, 57  

### FS00  
- **Pages:** 116, 236  

### FTXP  
- **Pages:** 110, 166, 461  

### FV11  
- **Pages:** 463, 467  

### FV12  
- **Pages:** 469, 471  

### GGB0  
- **Page:** 368  

### GGB1  
- **Page:** 368  

### KEA5  
- **Page:** 367  

### KEDR  
- **Pages:** 367, 368  

### M/03  
- **Page:** 190  

### M/06  
- **Page:** 181  

### M/07  
- **Page:** 184  

### M/08  
- **Page:** 176  

### ME11  
- **Page:** 199  

### ME12  
- **Page:** 199  

### ME21N  
- **Pages:** 200, 222, 231, 353  

### ME31K  
- **Page:** 200  

### ME51N  
- **Page:** 200  

### MEIS  
- **Page:** 456  

### MEK1  
- **Pages:** 189, 463, 467  

### MIRO  
- **Pages:** 162, 163, 201, 224  

### MM02  
- **Pages:** 134, 136  

### MMBE  
- **Page:** 234  

### NACE  
- **Page:** 171  

### OB07  
- **Page:** 130  

### OB08  
- **Page:** 129  

### OB22  
- **Page:** 127  

### OBC8  
- **Page:** 116  

### OBCD  
- **Page:** 198  

### OBCG  
- **Page:** 403  

### OBCH  
- **Page:** 404  

### OBQ1  
- **Page:** 462  

### OBQ2  
- **Page:** 463  

### OBXR  
- **Page:** 236  

### OBY6  
- **Pages:** 114, 121  

### OMKA  
- **Page:** 196  

### OMKL  
- **Page:** 142  

### OMKM  
- **Page:** 141  

### OMKN  
- **Page:** 141  

### OMKO  
- **Page:** 142  

### OVX6  
- **Pages:** 135, 137  

### OXK3  
- **Page:** 366  

### S_AC0_52000644  
- **Page:** 113  

### S_ALR_87012357  
- **Page:** 386  

### S_ALR_87012400  
- **Pages:** 397, 459  

### SCFD_EUI  
- **Page:** 367  

### SE11  
- **Pages:** 191, 194, 195, 270, 367  

### SE16  
- **Pages:** 25, 312, 315  

### SE16N  
- **Page:** 316  

### SE19  
- **Page:** 116  

### SE24  
- **Page:** 256  

### SE37  
- **Page:** 265  

### SE80  
- **Page:** 255  

### SM30  
- **Pages:** 114, 116, 148, 442, 447, 464  

### SMARTFORMS  
- **Page:** 170  

### SMOD  
- **Pages:** 281, 282  

### SPRO  
- **Pages:** 98, 180, 213, 294, 399, 442, 463, 465  

### V/03  
- **Page:** 190  

### V/06  
- **Pages:** 132, 168, 181  

### V/07  
- **Page:** 184  

### V/08  
- **Pages:** 176, 361  

### VA01  
- **Pages:** 208, 209, 221, 245, 352  

### VA02  
- **Page:** 208  

### VA03  
- **Page:** 208  

### VA41  
- **Page:** 244  

### VE01  
- **Page:** 455  

### VF01  
- **Pages:** 212, 224  

### VF02  
- **Page:** 212  

### VF03  
- **Pages:** 172, 212  

### VK11  
- [Page number missing]  

© 2022 by **Rheinwerk Publishing Inc., Boston (MA)**

## Index Entries

- **189, 364, 463, 467**
- **VK12** ....................................................................... 468
- **VL10B** .................................................................... 232
- **VL31N** .................................................................... 224
- **VOFM** .......................................................... 197, 276
- **VOV7** ...................................................................... 124
- **VTFA** ...................................................................... 213
- **VTFL** ....................................................................... 213
- **XD02** ............................................................ 137, 146
- **XK02** ...................................................................... 144

## Transaction Concepts

- **Transaction currency** .......................................... 128  
- **Transaction data screening** .............................. 326  
- **Transaction group** ................................................ 366  
- **Transaction matrix** .............................................. 374  
- **Transaction processing** ...................................... 352  
- **Transaction tag** ...................................................... 366  
- **Transaction type** ..................................... 25, 74, 320  

### Transfer Pricing Methods

- **Transactional net margin method (TNMM)** ................................................................ 352  
- **Transactional profit split method** .................. 352  

### Transaction-Based Reporting

- **Transaction-based reporting (TBR)** ........... 79, 80  

## Transfer Pricing

- **Transfer pricing** ....................................... 22, 33, 349  
  - **alerts and reports** ............................................. 353  
  - **calculate prices** ................................................. 352  
  - **calculation** .......................................................... 357  
  - **challenges** ........................................................... 355  
  - **derivation** ............................................................ 368  
  - **determination** ................................................... 364  
  - **dimensions** ......................................................... 365  
  - **documentation** ....................................... 354, 373  
  - **indirect cost allocation** .................................. 369  
  - **management** ..................................................... 350  
  - **monitoring** ......................................................... 364  
  - **operational** ............................................... 349, 350  
  - **planning** .............................................................. 351  
  - **process** .................................................................. 351  
  - **reporting** .............................................................. 371  
  - **requirements** ...................................................... 356  
  - **tax** .......................................................................... 350  
  - **testing** ................................................................... 373  

## Additional Transaction Terms

- **Translation date** .................................................... 128  
- **Transport code** ....................................................... 448  
- **Transport data** .............................................. 451, 452  
- **Transport responsibility** .......................... 216, 225  
- **Tree type** .................................................................. 394  
- **Trial balance** .............................................................. 33  
- **Triangulation simplification** ........................... 220  
- **True-up** ..................................................................... 295  
  - **report** .................................................................... 353  

## Tax Rules and Concepts

- **Undertaxed payments rule** ................................. 90  
- **Union territory tax (UTGST)** ............................ 461  
- **United States** .............................. 39, 65, 70, 75, 465  
  - **tax types** .............................................................. 468  
- **Universal Journal** ........................................... 32, 365  
  - **extend** .................................................................. 365  
- **Update run** .............................................................. 389  
- **Use tax** ......................................................................... 66  

## User Exits

- **User exit** ....................................... 262, 266, 323, 324  
  - **invoices** ................................................................ 274  
  - **sales orders** ........................................................ 267  
- **USEREXIT_MOVE_FIELD_TO_VBAK/VBAP** ................................................... 271  
- **USEREXIT_NEW_PRICING** ............................ 273  
- **USEREXIT_NUMBER_RANGE** ...................... 275  
- **USEREXIT_PRICING_PREPARE_TKOMK** ................................................... 267, 274  
- **USEREXIT_PRICING_PREPARE_TKOMP** ................................................... 268, 270  

## VAT and Related Topics

- **VAT ID determination** ................................... 151  
- **User interface** ............................................................ 31  

### Validation and Substitution

- **Validation routine** ............................................... 151  
- **Validation/Substitution tool** ........................... 368  

### Valuation Concepts

- **Validity period** ................................................... 40, 41  
- **Valuation area** ....................................................... 322  
- **Valuation variant** ................................................. 359  
- **Value contract** ....................................................... 244  
- **Value flow** ................................................................ 372  

### Value-Added Tax (VAT)

- **Value-added tax (VAT)** ... 40, 57, 62, 76, 220, 413  
  - **accounts** .............................................................. 118  
  - **directive** .................................................................. 62  
  - **examples** ............................................................. 143  
  - **exempt** ................................................................. 234  
  - **group settings** ................................................... 391  
  - **indicator** .............................................................. 137  
  - **listings** ..................................................................... 81  
  - **mapping** .............................................................. 403  
  - **rate change** ........................................................ 402  
  - **registration** ........................................................ 123  
  - **returns** .................................................................. 391  
  - **risk example** ...................................................... 422  
  - **standard report** ................................................. 385  
  - **systems** .................................................................... 64  
  - **versus GST** .............................................................. 64  

### VAT ID

- **Value-added tax (VAT) ID** ....... 43, 145, 163, 197, 214, 277, 286  
  - **check** ...................................................................... 197  
  - **customer** .............................................................. 420  
  - **determination** ................................................... 149  
  - **dummy** ................................................................. 148  
  - **examples** ............................................................. 152  
  - **extensions** ........................................................... 151  
  - **rules** ....................................................................... 149  
  - **undetermined** .................................................... 154  

## Variables and Data Models

- **Variable** ..................................................................... 263  
  - **inline declaration** ............................................. 264  
- **VAT Information Exchange System (VIES)** .... 43  
- **Vendor invoice** ......................................................... 39  
- **Vendor invoice management (VIM)** ............. 204  
- **Vendor master record** ............................................ 34  
- **Vendor selection** ................................................... 258  
- **Vendor withholding tax** ..................................... 144  
- **Version control** ...................................................... 260  
- **Vertex Indirect Tax O Series** ............................. 257  
- **Virtual data model** ............................................... 329  
- **V-model** ....................................................................... 85  

## Workflow and Work Concepts

- **What-if simulation** ............................................... 373  
- **Withholding tax (WHT)** ..... 86, 98, 170, 291, 316  
  - **basic** ............................................................... 98, 317  
  - **business partners** .............................................. 318  
  - **condition type** .................................................... 102  
  - **country-specific settings** ................................ 104  
  - **define accounts** ......................................... 99, 104  
  - **define tax code** .......................................... 98, 103  
  - **define type** ........................................................... 100  
  - **determination** .................................................... 159  
  - **extended** .............................................. 94, 100, 317  
  - **input side** ............................................................. 316  
  - **invoice posting** .................................................. 101  
  - **sales and distribution** ..................................... 102  
  - **settings** ................................................................... 98  
  - **tax rate** ................................................................... 99  
  - **tax tagging** .......................................................... 324  
  - **tax type** ................................................................. 101  
  - **vendors** ................................................................. 144  
- **Work delivery** ........................................................... 76  
- **Work performance** .................................................. 76  
- **Work supply** .............................................................. 76  
- **Workflow** .............................................. 204, 261, 435  

## Z Entries

- **Z table** ..................................................... 269, 274, 288  
- **Zero percent indirect tax** ................................... 218  

---

## Service Pages

### Praise and Criticism

We hope that you enjoyed reading this book. If it met your expectations, please do recommend it. If you think there is room for improvement, please get in touch with the editor of the book: **meganf@rheinwerk-publishing.com**.  

We welcome every suggestion for improvement but, of course, also any praise! You can also share your reading experience via **Twitter, Facebook, or email**.

### Technical Issues

If you experience technical issues with your e-book or e-book account at **SAP PRESS**, please feel free to contact our reader service: **support@rheinwerk-publishing.com**.

### About Us and Our Program

The website [http://www.sap-press.com](http://www.sap-press.com) provides detailed and first-hand information on our current publishing program. Here, you can also easily order all of our books and e-books.  

Information on **Rheinwerk Publishing Inc.** and additional contact options can also be found at [http://www.sap-press.com](http://www.sap-press.com).

### Legal Notes

This section contains the detailed and legally binding usage conditions for this e-book.

### Copyright Note

This publication is protected by **copyright** in its entirety. All usage and exploitation rights are reserved by the author and **Rheinwerk Publishing**; in particular the right of reproduction and the right of distribution, be it in printed or electronic form. © **2022** by Rheinwerk Publishing, Inc., Boston (MA).

### Your Rights as a User

You are entitled to use this e-book for **personal purposes only**. In particular, you may print the e-book for personal use or copy it as long as you store this copy on a device that is solely and personally used by yourself.

You are not entitled to any other usage or exploitation. In particular, it is not permitted to forward electronic or printed copies to third parties.

## Copyright and Distribution Restrictions

Furthermore, it is **not permitted to distribute the e-book** on the Internet, in intranets, or in any other way or make it available to third parties. Any **public exhibition, other publication, or reproduction** of the e-book beyond **personal use** is expressly prohibited.

The aforementioned restrictions apply not only to the **entire e-book** but also to parts thereof (e.g., **charts, pictures, tables, sections of text**).

## Copyright Notes and Digital Watermark

**Copyright notes, brands, and other legal reservations** as well as the **digital watermark** may **not be removed** from the e-book.

### Digital Watermark

This e-book copy contains a **digital watermark**, a **signature** that indicates which person may use this copy. If you, dear reader, are **not this person**, you are **violating the copyright**.

Please refrain from using this e-book if you are not the authorized user and inform the publisher about this violation. A brief email to **info@rheinwerk-publishing.com** is sufficient. Thank you!

## Trademarks

The **common names, trade names, descriptions of goods**, and so on used in this publication may be **trademarks** without special identification and subject to **legal regulations** as such.

All of the screenshots and graphics reproduced in this book are subject to **copyright © SAP SE**, Dietmar-Hopp-Allee 16, 69190 Walldorf, Germany.

### Registered and Unregistered Trademarks of SAP SE

- **SAP**
- **ABAP**
- **ASAP**
- **Concur Hipmunk**
- **Duet**
- **Duet Enterprise**
- **ExpenseIt**
- **SAP ActiveAttention**
- **SAP Adaptive Server Enterprise**
- **SAP Advantage Database Server**
- **SAP ArchiveLink**
- **SAP Ariba**
- **SAP Business ByDesign**
- **SAP Business Explorer (SAP BEx)**
- **SAP BusinessObjects**
- **SAP BusinessObjects Explorer**
- **SAP BusinessObjects Web Intelligence**
- **SAP Business One**
- **SAP Business Workflow**
- **SAP BW/4HANA**
- **SAP C/4HANA**
- **SAP Concur**
- **SAP Crystal Reports**
- **SAP EarlyWatch**
- **SAP Fieldglass**
- **SAP Fiori**
- **SAP Global Trade Services (SAP GTS)**
- **SAP GoingLive**
- **SAP HANA**
- **SAP Jam**
- **SAP Leonardo**
- **SAP Lumira**
- **SAP MaxDB**
- **SAP NetWeaver**
- **SAP PartnerEdge**
- **SAPPHIRE NOW**
- **SAP PowerBuilder**
- **SAP PowerDesigner**
- **SAP R/2**
- **SAP R/3**
- **SAP Replication Server**
- **SAP Roambi**
- **SAP S/4HANA**
- **SAP S/4HANA Cloud**
- **SAP SQL Anywhere**
- **SAP Strategic Enterprise Management (SAP SEM)**
- **SAP SuccessFactors**
- **SAP Vora**
- **TripIt**
- **Qualtrics**

These are **registered or unregistered trademarks of SAP SE**, Walldorf, Germany.

## Limitation of Liability

Regardless of the **care taken** in creating texts, figures, and programs, neither the **publisher** nor the **author, editor, or translator** assume any **legal responsibility** or **liability** for possible errors and their consequences.