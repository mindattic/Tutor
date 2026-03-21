---
title: correlithm-object-technology
source: correlithm-object-technology.txt
formatted: 2026-02-08T17:56:03.1303003Z
---

## Correlithm Object Technology  
**P. Nick Lawrence, Ph.D.**  
**Correlithm Publications**  
**Dallas, Texas**  
**Correlithm Publications**  
A Division of **Lawrence Technologies, LLC**

---

### COPYRIGHT © 2004  
by **Lawrence Technologies, LLC**. All rights reserved. Printed in the United States of America. Except as permitted under the **United States Copyright Act of 1976**, no part of this publication may be reproduced or distributed in any form or by any means, or stored in a database or retrieval system, without the prior written permission of the publisher.  

This book is printed on acid-free paper.  

**ISBN 0-9752761-1-5**  

This book was produced in **Times New Roman**. The cover was designed by **Constance L. Wisber**. **Lightning Press, Inc.** was printer and binder.  
[http://www.LT.com](http://www.LT.com)

---

### Dedication  
For **Monty**, who believed, and **Connie**, who knew.

---

## Table of Contents  

- PREFACE XI  
- ACKNOWLEDGEMENTS XIII  

### CHAPTER I OVERVIEW  
- A Short Summary 2  
- What Good Are CO Systems? 3  
- How the Brain Works 6  
- The Problem 6  
- The Solution 6  

### CHAPTER II UNIT N-CUBES  
- Distances in Unit N-Cubes 9  
- Summary of Distances 12  
- Properties of Unit N-Cubes 14  
- Unfamiliar Geometry 15  
- Additional Perspectives 20  

### CHAPTER III PROBABILITY AND INFORMATION  
- Summary 26  
- Distribution of Distances 27  
- Probability of Proximity 28  
- Information Content of Proximity 31  
- Point Capacity 32  

### CHAPTER IV ELEMENTS OF CORRELITHM OBJECT COMPUTING  
- Spaces 39  
- Points 41  
- Soft Tokens 42  
- Rules 47  
- Strings 49  
- Sensors and Actors 52  
- Systems 53  
- Miscellaneous 54  

### CHAPTER V CORRELITHM OBJECT MACHINES  
- Definition of a Correlithm Object Machine 59  
- Building a New State Point 61  
- Cell Form of Rule 63  
- Target Cell Independence 63  
- Dimensions Appearing in Multiple Rules 64  

### CHAPTER VI COMPLETENESS  
- Logical “NOT” 70  
- Logical Completeness 71  
- Logical “NAND” 71  
- No Perceptron Limitations 73  
- Logical “XOR” 73  
- Turing Completeness 75  
- Flip-Flop 75  

### CHAPTER VII STRINGS  
- String Concepts 80  
- Approximate Relative Location 85  
- Ordinality 90  
- Higher Dimensions 91  
- Comments 96  

### CHAPTER VIII ARCHITECTURE  
- Serial Implementations 99  
- Concurrent Implementations 100  
- Analog Implementations 105  
- Properties and Capabilities 108  
- Sampling 109  
- Superposition 111  
- Statistical “Holograms” 113  
- Straddlers 118  

### CHAPTER IX CONVERTERS  
- Cardinal Converters 120  
- Continuous Converters 122  
- Ordinal Converters 125  
- Higher Dimensional Converters 127  
- Converter Form 128  

### CHAPTER X MECHANISMS OF BEHAVIOR  
- Turing Completeness 134  
- Soft Tokens 135  
- Robustness 137  
- Superposition 139  
- Strings 141  
- Patterns and Functions 142  
- Rule Pathways 143  
- Grammar Systems 145  
- Parse Trees 145  
- Generation Trees 147  
- Ladders 149  
- Automata 152  
- State Machines 153  
- Stack Machines 154  
- Generic Stack Machines 156  
- Expert Systems 160  
- Continuous Systems 163  
- Functional Relationships 163  
- Differential Equations 169  
- Feedback Control Systems 170  
- Artificial Neural Networks 171  
- Fuzzy Logic 172  
- Internal Control 174  
- Learning 175  
- Mind 178  

### CHAPTER XI DIRECTIONS  
- Why CO Technology Will Succeed 181  
- Why CO Technology Was Not Discovered Sooner 182  
- Summary of Characteristics 184  
  - The Correlithm Object 184  
  - Soft Tokens 184  
  - String COs 184  
  - Robustness 184  
  - Sampling 185  
  - Superposition 185  
  - Straddlers 185  
  - Statistical “Holograms” 185  
  - Virtual Systems 186  
  - Continuous Systems 186  
  - General Purpose Analog Computers 186  
  - Early Applications 186  
  - Enhancing Current Systems 186  
  - Cueing 187  
  - Data Mining 187  
  - Data Fusion 187  
  - Speech 187  
  - Natural Language 188  
  - Vision 188  
  - Handwriting 188  
  - Simple Robots 188  
  - Security 188  
- The Future 189  
- Tools 189  
- Learning 190  
- Quantum Information Systems 191  
- Ensembles 192  
- Conclusions 194  

### APPENDIX I  
- DERIVATION OF UNIT N-CUBE DISTANCES 197  
  - Random Point to Random Point 197  
  - The Problem 197  
  - The Solution 197  
  - Random Point to Midpoint 200  
  - The Problem 200  
  - The Solution 200  
  - Random Point to Corner 200  
  - The Problem 200  
  - The Solution 201  

### APPENDIX II  
- CORRELITHM OBJECTS AND NEUROPHYSIOLOGY 203  
  - Overview 203  
  - Representing Information 203  
  - Storing Information 204  
  - A Closer Look 206  
  - Manipulating Information 209  
  - Conclusion 210  

- GLOSSARY 211  
- INDEX 221  

---

## Preface  

The goal of this book is to present **Correlithm Object Technology** in a way that is accessible to a wide audience. I believe that this goal has been met.  

**College students**, and even advanced **high school students**, have successfully assimilated this material. Certainly anyone with a **technical degree** should have no trouble. Chapters II through X each begin with a general summary that describes the importance and the key results of the chapter and requires very little technical knowledge.  

Casual readers should read **Chapter I**, the summaries from Chapter II through Chapter X, and Chapter XI.  

---

It has been suggested that this material is **“difficult.”** I submit that the concepts are not difficult so much as **unfamiliar**. Most of us are not accustomed to dealing with **geometry in more than two or three dimensions**, yet the properties important here do not really exist in spaces with less than about twenty dimensions.  

The material is interdisciplinary, requiring concepts from **mathematics**, **computer science**, **electrical engineering**, and several other areas. Even so, I believe the book is reasonably **self-contained** because most of the necessary background material is provided. Careful and attentive reading will be well rewarded.  

---

The book is laid out in a **systematic manner**.  

- Chapters **I** and **XI** provide summary material.  
- Later chapters build on earlier ideas.  
- Chapter **I** provides a general overview and some background material.  
- Chapter **II** presents the key concepts required from the statistics of bounded, **high-dimensional spaces**.  
- Chapter **III** develops many of the useful mathematical relationships about these concepts.  
- Chapter **IV** presents the **correlithm object (CO) computing paradigm** and some important terminology.  
- Chapter **V** provides a model of the formal computing machine implied by the CO paradigm.  
- Chapter **VI** shows that **CO systems are Turing complete**, i.e., they can compute anything that can be computed.  
- Chapter **VII** extends the concept of the CO from points to higher-order objects, such as lines, surfaces, and volumes, greatly expanding its capabilities.  
- Chapter **VIII** introduces unique properties of CO computing systems, including **robustness**, **sampling**, **superposition**, and **holographic-like characteristics**.  
- Chapter **IX** discusses how to move real-world data into and out of CO systems.  
- Chapter **X** contains material on unique behavior inherent in CO systems.  
- Finally, Chapter **XI** summarizes the results and proposes some future directions.  

Appendix I presents derivations of some key properties of **unit N-cubes**. Appendix II outlines my theory of how information is represented, stored, and manipulated in living **neural systems**. This is a detailed, testable model that deserves investigation in the neurophysiology lab.  

---

The ramifications of **CO technology** are vast with implications and opportunities in nearly every field of endeavor. Everyone who has endeavored to understand this material has been stimulated by the way understanding has produced new insights, new directions for research and development, and seemingly endless ideas for applications and commercialization.  

The principal intellectual property rights are owned by **Lawrence Technologies, LLC** ([www.LT.com](http://www.LT.com)), focused on developing CO technology in every conceivable way. Please visit the website for more information.  

---

Several commercial applications have emerged from CO technology.  

- **Syngence, LLC** ([www.syngence.com](http://www.syngence.com)) utilizes licensed CO technology to provide unique **data mining capabilities** for the legal profession.  
- **CDX Gas, LLC** ([www.cdxgas.com](http://www.cdxgas.com)) has been developing applications of licensed CO technology in natural resources business.  
- Feasibility has been demonstrated for applications in **telecom**, **computer security**, and **intelligence work**.  

Some work was funded in part by the **United States Air Force** under contracts **F30602-02-C-0077** and **F30602-03-C-0051**.  

---

## Acknowledgements  

First, I thank **Warren McCullough**, **Murray Eden**, and **Jerome Lettvin** from my days at **MIT**, for many interesting and stimulating conversations when correlithm object technology was embryonic.  

I have bent the ears of many people over the years, too many to try to list, but I am grateful to each of you.  

Many people have contributed directly:  
- **Monty Rial**, **Dr. Eustace Winn**, **Steve Clary**, **Ray Davis**, and **Phil Whisenhunt** provided significant **financial support**.  
- **Doug Matzke, Ph.D.**, **Chandler Burgess**, **Ken Loafman**, **Steve Benno**, **Katrina Riehl**, and **Zack Loafman** made important **technical contributions**.  
- **Bryan Aldridge** and **Mike Lockerd** contributed in other areas.  
- **Merrill Nunnally** provided editorial review of the manuscript.  

Special thanks to my wife, **Connie**. The fine **graphic artwork** in this book is hers. I would be very grateful even if that were her only contribution, but without her **confidence**, **encouragement**, and **persistence**, it is hard to imagine that this book, nor correlithm object technology itself, would ever have seen the light of day.  

**Nick Lawrence**  
April 2004  

---

## Chapter I Overview  

**Correlithm object technology** lets you build systems that behave more like **living organisms** than machines. It is based on my discovery of remarkable and unexpected **statistical properties** of **bounded, high-dimensional geometrical spaces**. It offers capabilities quite distinct from those of traditional computing systems and opens new directions for science, engineering, and applications development.  

---

The author coined the term **“correlithm”** around **1975**. Here is the definition:  

**cor-re-lithm** (cor'e lith'em) n. [[comb **CORRELATION** + **ALGORITHM**]]  
1. **Math.** Any systematic method for solving problems by using similarities to known examples.  
2. **Comput.** An algorithm primarily based on the use of characterizing examples.  

**--cor're-lith'mic (adj.)**  

---

Correlithm object technology is a **computing paradigm** designed around an unusual means for representing information called a **correlithm object**, or **“CO.”**  

---

We derived COs from observations of **living neural systems**. We believe that COs are the primary data tokens in these systems, where they are central to essentially all high-level data representation, storage, and manipulation. To understand a living neural system as an information processing system, one must understand **CO technology**.  

---

However, this is not a book about the science of neurons. It is about using COs to engineer unique information processing systems.  

COs are important **mathematical objects** of the statistics of bounded, high-dimensional spaces. Whether or not COs play a role in living neurons is irrelevant here. We focus on the nature of COs and what can be done with them.  

---

### A Short Summary  

For those asking, “What is this all about,” here is a very short summary of a complex subject.  

Consider a list of **N real numbers**. Suppose each is independently selected at random from the range zero to one, with all values equally likely. The list can be interpreted as the coordinates of a random point in a **unit N-cube**.  

Suppose we pick two such points. The distance between them can be found from the **Pythagorean theorem** in **N dimensions**.

## Chapter I Overview

The **statistics of distances** in high-dimensional spaces assure us that two points will almost certainly **not be close together by accident**. When **N**, the number of dimensions, is no smaller than about **twenty**, points selected at random are all about the **same distance from each other**. This result is **counter-intuitive** and not widely known.

The volume of a unit **N-cube** is vast. For instance, if each of the **N coordinates** can only take two values, there are \(2^N\) points. When **N = 20**, this yields over **one million points**; when **N = 50**, there are over **one million billion points**. With real numbers, the number of points is unlimited. For **N = 100** and selecting ten values per coordinate, the number of points is \(10^{100}\), known in mathematics as a **googol**. There are far fewer than a googol particles in the known universe.

This vastness is critical. Suppose a process produces a known point in an N-cube, and we have a test point. The answer to whether the process or a random process produced the test point depends on the **distance** between them. Since the **N-cube is vast**, random points are about the same distance apart. If the points are about that distance, the test point is likely random. If they are closer, the test point probably came from the process, with the small separation likely due to slight variations or **noise**. This is a key principle of **correlithm object technology**.

Points in an N-cube serve as **data tokens**. Each known point is effectively a "**statistical landmark**." Because of the vastness, a randomly selected point rarely lies near any previously selected point. Points from unrelated processes will not be close together, so points arising from the same process tend to cluster. Thus, a point produced by a process can robustly represent that process as a **data token**.

Mapping these data tokens creates a **general-purpose computational paradigm**. These mappings can compute anything and produce behaviors more like those in living creatures than machines. This book explores these concepts in depth, starting with the properties of random points and unit N-cubes, developing formulas and nomenclature, describing information processing, proving the paradigm’s generality, and detailing unique capabilities of systems built this way.

By the book's end, it is expected that readers will agree that **correlithm object technology** holds **tremendous promise**.

---

## What Good Are CO Systems?

The **information processing characteristics** of traditional computers and living organisms differ greatly. 

- **Computers** are typically **precise, fast, and brittle**. They rarely make mistakes but are prone to complete failure with minor hardware or instruction errors. They do not fail **gracefully** and handle data with **noise** poorly.

- **Living organisms** are usually **imprecise, slow, and flexible**. They cope well with ambiguous, noisy data. Their biological hardware is highly reliable, and loss of some neurons is not critical. Living systems process complex, dynamic tasks quickly — like a cheetah chasing a gazelle — involving visual analysis, muscle coordination, planning, etc.

**CO systems** exhibit many behaviors of living organisms. These behaviors *emerge*, rather than being explicitly designed. This reflects that **CO systems capture the essence of living information processing**. This assertion is supported by proofs and examples in the book and the nature of CO system implementations.

The **correlithm object** is a new, useful tool that adds dimensions to familiar paradigms like expert systems, artificial neural networks, fuzzy logic, pattern recognition, and AI. It provides a missing framework for **data representation** that can integrate with these approaches.

CO systems capture **similarity**. They “see and do things like things previously seen and done.” A specific pattern or behavior provides a **prototype or template** for recognition or action over a range of similar inputs.

---

## Properties of CO Systems

- CO system implementations can be very **robust**.
- A single system can be **distributed** across many computing resources, such as **internet nodes**.
- Conceptually, imagine several “brain cells” on many computers linked through a network.
- Or consider a “group mind” across a swarm of robotic insects.
- Loss of many distributed resources usually does not cause total failure.
- CO systems have a **holographic** nature.

The potential applications of CO systems are broad. Wherever a living creature outperforms traditional computers in information processing is a likely domain for CO systems.

---

## Applications of CO Systems

- Traditional Mars robots are fragile and slow, moving inches every few hours due to communication delays. 
- CO systems could enable robots with biological-like **robustness and flexibility**, comparable to a cockroach’s or even a cat’s agility.
  
CO systems work well as **cueing systems for human experts**, directing attention to productive focus areas.

Examples include:

- **Petroleum exploration**
- **Legal discovery**
- **Medical imagery**
- **Network security**

CO technology is likely to become important in many fields, some surprising:

- **Neurophysiology and psychology** (ensembles of neurons)
- **Quantum Information Science** (ensembles of qubits and ebits)
- **Sociological behavior modeling** (ensembles of individuals and social groupings)

Ensembles are ubiquitous, making knowledge of CO technology essential for technical workers.

---

## How the Brain Works

### The Problem

How do living neural systems **represent, store, and manipulate information**? How can their unique capabilities be replicated in artificial systems?

The focus here is not the detailed **science of neurons**, but the **high-level information processing** of large neuronal ensembles.

### The Solution

- Every living neuron produces **pulses** that vary with time.
- The key is to focus on the **pulse rate** of each neuron, not individual pulses.
- Pulse rate varies continuously between **zero** and a fixed **maximum rate**.
- Pulse rates are normalized to a **real number between 0 and 1**.

An ensemble of neurons has a **state** at any time represented as a list of instantaneous pulse rates — interpretable as the coordinates of a single point in a **unit N-cube**. As the ensemble state changes, the point moves.

Statistically, points representing states at different times will not lie close together by accident. If they are close, they likely come from slight variations of the same process.

Living systems use the **statistical properties** of these high-dimensional points to robustly represent information.

---

### Neuronal Processing Using State Points

- Each neuron **watches** the ensemble state.
- When it detects an "**important**" state point, it pairs this point with its current output pulse rate and **stores the pair permanently**.
- Storage occurs in the neuron's **input dendritic tree** using properties of **pseudopods**, **axonal membranes**, and **correlithm object statistics** (see Appendix II).

Living systems store most of their permanent information this way.

---

### Information Manipulation in Living Systems

- Neurons use stored pairs to guide future behavior.
- A neuron "watches" the input for the ensemble's **state point** approaching a saved state point.
- When the current state point nears a saved state point, the neuron adopts the paired pulse rate.
- The **closer** the two points, the less noise affects the output pulse rate.
- If the points are far apart, the output pulse rate can deviate significantly.
- The deviation decreases as points get closer.

---

### Ensemble-to-Ensemble Mapping

An ensemble of neurons sets output pulse rates, producing another **state point** whose coordinates are these output pulse rates.

Effectively:

- A **state point from one neural ensemble maps to a state point in another ensemble**.
- These **statistical properties** of state points enable living systems’ info processing.
- Correlithm objects are extremely numerous and very **robust**.

---

## Correlithm Object Technology

**Correlithm objects** are the one feature that truly separates **CO technology** from all other computing paradigms.

---

## Chapter II Unit N-Cubes

### Visualizing N-Dimensional Cubes

Visualizing a cube that exceeds **20 dimensions** tests the imagination. That millions of random points within that figure measure the same distance apart with a very high degree of probability defies common sense and intuition. 

As ironic as it sounds, this extreme geometry results in an environment that produces highly reliable **uniform distances** between random points. The points, called **correlithm objects**, act as data representatives to achieve computing breakthroughs not possible in a **four dimensional universe**.

---

### Distances in Unit N-Cubes

**Geometric probability** is the branch of mathematics that studies the probabilities involved in geometric problems. We are interested here in the probabilities of certain distances in **real unit N-cubes**.

A *real unit N-cube* is an **N-dimensional cube** whose edges all have **real unit length**.

Traditional geometry tells us that a unit N-cube has:

- **2^N corners**
- The length of the straight line between two maximally distant or opposite corners (called the **major diagonal**) is \(\sqrt{N}\)
- The length of a straight line from the **center of the unit N-cube** (called the **midpoint**) to any corner is half of the major diagonal, or \(\frac{\sqrt{N}}{2}\)

The **Hamming distance** between two corners in a unit N-cube is defined as the cumulative length of the minimum number of edges...

## 1

The most recent book on this subject is **D. Klain, G. Rota, Introduction to Geometric Probability, Cambridge University Press, 1997**.

## 10 Correlithm Object Technology

The **Hamming distance** between two opposite corners of a unit **N-cube** is **N**. The number of corners that lie at a Hamming distance of **h** from a particular corner is given by the **binomial coefficients** as the combinations of **N** things taken **h** at a time, or **C(N, h)**.

What is the probability that two randomly selected corners of a unit **N-cube** lie at a Hamming distance of **h**? Since there are **2^N** corners and there are **C(N, h)** corners that lie at distance **h** from a given corner, the probability is:

- **C(N, h) / 2^N**

What is the expected Hamming distance between two randomly selected corners? We know that the mean of the binomial distribution lies at **h = N/2**. Thus, **N/2** is the expected Hamming distance.

**Pentti Kanerva** noted this geometric probability in his book. He also showed that the **standard deviation** of this expected Hamming distance is about **√N**.

What is the expected **Cartesian distance** between two randomly selected corners? The Cartesian distance is the length of a straight line connecting two points, namely the square root of the sum of the squares of the differences between the points' coordinates in each dimension.

Since we know that the expected Hamming distance between two such corners is **N/2** and each edge has length **1**, we can immediately obtain the expected Cartesian distance as **√N/2**.

Unless otherwise noted in the following, “distance” will mean **Cartesian distance**.

We will need the expected values of three other probabilistic distances in the unit **N-cube**. Suppose we select a point at random from the entire volume of the unit **N-cube**. To do this, we select an...

## 2 More Generally, the Hamming Distance Between Two Points in a Unit N-Cube

The **Hamming distance** between two points in a **unit N-cube** is defined as the sum of the lengths of the straight lines that must be traversed in each dimension to go from one point to the other.

---

## Chapter II Unit N-Cubes

We consider a point in the unit N-cube defined by an **independent uniformly distributed random number** between **0.0 and 1.0 inclusive** for each dimension. This set of N random numbers forms the coordinates of the desired random point, which we call **p1**. We ask what the expected distances are between:

- **p1 and the midpoint**
- **p1 and a randomly selected corner**
- **p1 and another point p2**, independently selected at random

Exact analytic solutions likely do not exist. Appendix I presents a derivational approach giving valuable expansions for these problems, but the math is challenging. Alternatively, simulation programs can approximate these statistics.

---

### Expected Distances in the Unit N-Cube

- The expected **Cartesian distance** between two random points in the entire unit N-cube is approximately  
  **N / 6**  
  We call this the **standard distance**.  
  The **standard deviation** of the standard distance is approximately **√7/120 ≈ 0.2415**.  
  The **standard distance grows with N**, but its standard deviation is **independent of N**. These form the foundation for **correlithm object theory**.

- The expected distance between a random point and the **midpoint** is approximately  
  **N / 12**  
  This is called the **standard radius**.  
  Its standard deviation is approximately **√1/60 ≈ 0.1291**.

- The expected distance between a random point and **any corner** is approximately  
  **N / 3**  
  This is called the **standard diameter**.  
  Its standard deviation is approximately **√1/15 ≈ 0.2582**.

---

### Summary of Distances (Table II-1)

| Type    | Property                  | Cartesian Distance (Unit Edge) | Standard Deviation (Unit Edge) | Cartesian Distance (Unit Radius) | Standard Deviation (Unit Radius) |
|---------|---------------------------|-------------------------------|-------------------------------|----------------------------------|----------------------------------|
| Exact   | Edge: length of side       | 1                             | 0                             | ≈ 1/2 N                         | ≈ 0                             |
| Exact   | Major diagonal             | N                             | 0                             | ≈ 1/2                           | ≈ 0                             |
| Prob    | Corner to corner (random)  | ≈ N/2                         | ≈ 1/9                         | ≈ 6                            | ≈ 4/3 N                        |
| Prob    | Standard diameter          | ≈ N/3                         | ≈ 1/15                        | ≈ 4                            | ≈ 4/5 N                        |
| Exact   | Half diagonal (midpoint to corner) | N/4                   | 0                             | ≈ 3                            | ≈ 0                             |
| Prob    | Standard distance          | ≈ N/6                         | ≈ √7/120                      | ≈ 2                            | ≈ 7/10 N                       |
| Prob    | Standard radius            | ≈ N/12                        | ≈ √1/60                      | 1                              | ≈ 1/5 N                        |

**Notes:**  
- Columns labeled **Unit Edge** assume an edge length of 1 for the N-cube.  
- Columns labeled **Unit Radius** normalize these distances by the length of the **standard radius (N/12)**.  
- Under **Unit Radius normalization**, exact geometric distances become approximate statistical distances.

---

### Properties of Unit N-Cubes

A **real unit N-cube** (and more generally any bounded N-space) has unfamiliar properties when **N > 20**. For smaller dimensions, the dropped terms in expansions are non-negligible.

As **N grows beyond 20**, the approximations in Table II-1 become accurate summaries.

- The ratio of **standard distance** to its **standard deviation** is about  
  \(\frac{20N}{7}\).
  
- For example, when **N = 35**, the ratio is approximately **10**.  
  This means that two randomly chosen points in the unit 35-cube will be about the same distance apart the vast majority of the time. It is a very rare event to find two points significantly closer or farther than this expected distance.

- As **N increases**, this ratio grows with \(\sqrt{N}\), leading to the extraordinary conclusion:  
  **Any two randomly chosen points in a suitably large unit N-cube are essentially the same distance apart.**

- Only **N+1** points can be arranged so that each point is exactly the same distance from every other point, e.g.:  
  - **Equilateral triangle** in 2 dimensions (unit square)  
  - **Tetrahedron** in 3 dimensions (unit cube)

---

### Example

Selecting **55 points at random** from a unit 54-cube:  
- Each mutual distance is about \( \frac{N}{6} = \frac{54}{6} = 9 \) units apart.  
- Standard deviation is about **0.2415**.  
- Distances as small as about **2.5** or as large as about **3.5** (±2 standard deviations) are increasingly rare.

If **1000 points** are selected, there might be more chances of extreme distances, but they remain rare overall with a **Gaussian distribution** centered at 3 with standard deviation ≈ 0.2415.

---

## Unfamiliar Geometry of Unit N-Cubes

Define key points:  
- **m**: midpoint of the cube  
- **p, q**: random points within the cube  
- **c, d**: random corners  
- **o**: corner opposite c

---

### Right Triangles Formed in the Unit N-Cube

The unit N-cube exhibits many **right triangles** formed by these points. These follow from distances given in Table II-1.

- **Triangle cdo:** Major diagonal \(co\) length = N  
  Legs \(cd\) and \(do\) each ≈ \(N/2\)  
  \(co^2 = cd^2 + do^2 = N\), so it's a right triangle.

- **Triangle cmd:**  
  Legs \(cm\) and \(md\) each have length \(N/4\)  
  Hypotenuse \(cd\) ≈ \(N/2\)  
  Right angle at midpoint \(m\).

- **Triangle cmp:**  
  Lengths:  
  \(cm = N/4\),  
  \(cp ≈ N/3\),  
  \(mp ≈ N/12\)  
  \(cp^2 = cm^2 + mp^2 => N/3 = N/4 + N/12\)  
  Right triangle with hypotenuse \(cp\).

- **Triangle pmq:**  
  Lengths:  
  \(pm ≈ N/12\),  
  \(mq ≈ N/12\),  
  \(pq ≈ N/6\)  
  \(pq^2 = pm^2 + mq^2\)  
  Right triangle with hypotenuse \(pq\).

---

### Unit Radius Normalization of Triangles

Using **Unit Radius normalization**, these right triangles satisfy:  

- Triangle cdo: \(2^2 = 6^2 + 6^2\)  
- Triangle cmd: \(6^2 = 3^2 + 3^2\)  
- Triangle cmp: \(4^2 = 3^2 + 1^2\)  
- Triangle pmq: \(2^2 = 1^2 + 1^2\)

This normalization often offers convenience over Unit Edge normalization.

---

### Schematic View of the Unit N-Cube (Figure II-1)

Points shown schematically include:

- **m**: midpoint  
- **c**: random corner  
- **o**: corner opposite c  
- **d**: another random corner  
- **p, q**: random points from the cube volume

Line segments and angles are to scale, with various right angles among them (e.g., pmq, pmc, cmd, cdo).

---

### Equatorial Properties of the Unit N-Cube (Figure II-2)

From the perspective of a particular corner **c**:

- Opposite corner **o** forms "poles" on the axis of an **N-sphere**.  
- All other corners lie on the **equator** of this N-sphere.  
- Each equatorial corner is at a **right angle** vertex to the poles.  
- Any two equatorial corners form an **equilateral triangle** with either pole.  
- The midpoint **m** lies at the center of the equatorial circle between the poles.

From the viewpoint of a random point **p** in the cube volume:

- All corners appear **equidistant** and equally spaced apart.  
- All other random points **q** also appear equally distant and spaced.  
- The view is similar regardless of the chosen point.

## Chapter II Unit N-Cubes

From the perspective of **c**, all corners appear to lie on the **equator of an N-sphere** of radius **3**, and all random points from the **N-cube’s volume** appear to lie on the equator of another **N-sphere** of radius **1**. All points in the **corner equator** are about distance **6** apart, and all points in the **random points equator** are about distance **2** apart.

From the perspective of the midpoint **m**, all corners are equally far away and equally spaced. Similarly, all points are equally far away and equally spaced.

Looking again from the perspective of a randomly chosen corner **c**, we see that all randomly chosen points appear to lie in a **ring on a plane perpendicular** to the major diagonal **co** that passes through the midpoint **m**. Further, the distance between every pair of points in the ring is about the same. This view is seen from any corner.

We remind the reader that we are dealing with **randomly chosen points and corners**. The **unit N-cube** has a **smooth, continuous distribution** of points. The geometrical properties we are describing only occur as a result of selecting points and corners at random. It is trivial to identify **non-random points and corners** where these relationships do not hold, but that does not invalidate our statistical results in any way. We are saying that if you select points and corners at random as we describe, then these properties exist.

The **right triangles** we have found above provide a useful tool. If we move the origin of our coordinates to the vertex of any right angle, we can treat the resulting coordinates of any point in the N-cube as a **vector** with respect to that origin. We know from vector analysis that the **inner product** of any two vectors that form a right angle is zero.

The inner product is simply the sum of the products of paired coordinates. For example, consider two random points, **p** and **q**, together with the midpoint **m**, called triangle **pmq** above. Up until now, we have been assuming that one corner of the unit **N-cube** serves as the origin and that all coordinates are positive between zero and one. Suppose instead we choose the midpoint as the origin. Then the inner product of the vectors **p** and **q**, namely **p·q**, is approximately **0**.

In other words, as seen from the midpoint, the vectors **p** and **q** are approximately **orthogonal**. Any pair of points selected at random from a unit N-cube has this property, which gives us a very large pool of essentially **orthogonal basis vectors**. These vectors have many uses.

These descriptions are all made from particular points of view. The viewpoint of an observer is a recurring and useful theme in the theory of **correlithm objects**.

---

## Additional Perspectives

Here are two additional images that may be useful in gaining familiarity with these unfamiliar statistical properties of geometry.

- The first is from the perspective of a **randomly selected point**.
- The second is from the perspective of the **midpoint of the N-cube**.

### Figure II-3  

Here is the distribution of random points (inner sphere) and corners (outer sphere), from the perspective of one randomly chosen point (the point at the center). The **radii are drawn to scale**. The thicknesses of the spheres reflect the **standard deviations** for both point distributions. These thicknesses are drawn to scale for **N=35**. They go to zero thickness quickly as N grows.

Figure II-3 shows a single randomly selected point surrounded by two spheres, with a cutout to better show structure. The **inner sphere** represents the volume of the unit N-cube sampled by random points, and the **outer sphere** represents all corners.

The radii represent the expected distances from the central point to the random points and to the corners respectively. The thicknesses of the sphere represent the bounds of **one standard deviation** for each of the expected distances.

Bear in mind that if any point from the inner sphere were placed at the center, the current center point would then (probably) be found within the inner sphere. Thus from the perspective of any random point in the unit N-cube, all other points—and all corners—appear to lie at **essentially constant distances**.

Said another way, the essential geometry of a high dimensional unit cube—in fact all high dimensional bounded geometries—is an approximate **N-hedron**, where all points lie about the same distance apart, an arrangement that approximates an **N-dimensional tetrahedron**.

### Figure II-4

Here is the distribution of random points (inner sphere) and corners (outer sphere), from the perspective of the **midpoint of the unit N-cube** (the point at the center). The radii are drawn to scale. The thickness of the inner sphere reflects the standard deviation for the random point distribution. This thickness is drawn to scale for **N=35**. The thickness goes to zero quickly as N grows.

The thickness of the outer sphere is actually zero, but shown non-zero here for visibility.

Figure II-4 shows the midpoint point of the N-cube surrounded also by two spheres:

- An **inner sphere**, which represents the volume of the N-cube sampled by random points.
- An **outer sphere**, which represents the corners.

The radius of the inner sphere represents the expected distance from the midpoint to the random points. The thickness of that sphere represents the bounds of **one standard deviation** of the expected distance.

The radius of the outer sphere represents the distance from the midpoint to any corner, which is geometrically exact and thus has a standard deviation of zero, but in order to make this sphere visible in the illustration, we have shown the outer sphere with a small, non-zero thickness.

This perspective is significant because placing the origin of the unit N-cube at the midpoint of the cube facilitates the use of the random points as **orthogonal basis vectors**. This is because essentially any pair of random points and the midpoint forms a triangle with a right angle at the midpoint.

Although the specifics will differ, these same kinds of statistical properties exist for any bounded N-space. Distances are all finite in bounded spaces, even if the bounds are not provided by an N-cube with unit edges.

As long as there is no infinite length in any direction in an N-dimensional object, in other words as long as the N-dimensional object is **finitely bounded**, then similar finite statistics exist.

For example, we have found important uses for N-cubes of **edge length two** and for **N-spheres of unit radius**.

Similar statistical properties exist not only for real bounded N-spaces, but also for bounded N-spaces of **any type of number**. Pentti Kanerva used **binary N-cubes** in his work (see Footnote 3). We use **real unit N-cubes** for the computational model described in this book. We have employed **complex unitary N-spheres** in our work in quantum information science (**QIS**). Additional useful forms undoubtedly exist.

Moreover, there is a geometrical form that is closely related to the cube that offers still more interesting aspects, namely the **torus**. An **N-torus** is simply an **N-cube** whose dimensions have each been “looped.” That is, each dimensional position is computed modulo the extent of the dimension.

For example, the value **0.99** in a particular dimension of a unit N-torus is an absolute distance of **0.02** away from the value **0.01** in the same dimension, rather than an absolute distance of **0.98**.

For what follows in this book, it is very important to understand that our intuition about squares (**unit 2-cubes**) and cubes (**unit 3-cubes**) does not serve us very well for **unit N-cubes**.

**Volume grows very rapidly** as dimensions increase, and we have seen that remarkable and unexpected statistics emerge. These notions are central to **correlithm object technology**.

---

## Chapter III Probability and Information

As long as the cube is larger than about **20 dimensions** (50 to 100 dimensions is preferable for creating a correlithm object system), a huge number of randomly chosen points can be “packed” into a **unit N-cube**, and they will all be nearly the same distance apart from one another. 

As paradoxical as it may seem, this property is statistically extremely reliable. The points retain this nearly uniform distance as long as the N-cube has enough dimensions, and there will always be plenty of points available to design a **correlithm object system**.

Because the random points are so reliably the same distance apart, any points that are **not about the same distance apart** are probably **not created by a random process**. Abnormally close points carry significant data that can be exploited to determine which process probably created the points. This information can then be used to build powerful **error correction**, **pattern recognition**, and **pattern generation systems**.

**Probability** and **information** are closely related. Our objective in this chapter is to explore further the geometric probabilities of unit N-cubes that were introduced in the previous chapter, to relate those probabilities to **information theoretic concepts** and to lay the foundation for the computational paradigm that we will explore in subsequent chapters.

We will need the ability to calculate various properties. First, we summarize the results, and then we explore their derivations.

---

## Summary

Here is a summary and short discussion of the important relationships that are developed in this chapter. Those who are not interested in the mathematical details can read just this summary and omit the rest of the chapter.

| Relationship             | Property                                           | Equation  |
|-------------------------|--------------------------------------------------|-----------|
| Distribution of Point-to-Point Distances | \(\displaystyle (2) \quad ( ) \quad 2 2 2 1 ( ) \sigma \mu \sigma \pi d \text{ normal } d e^{-\frac{(d-\mu)^2}{2\sigma^2}}\) | III-A |
| Probability of Proximity | \(\displaystyle 2 2 24.7( ) 7.45( ) ((1,2)) 2 10 d d \text{ prob dist } p_1 p_2 d < \approx \mu \mu\) | III-J |
| Bit Content of Proximity | \(\displaystyle 2 ( ) 6 \text{ BitContent}(d) \approx \frac{\mu - d}{\sigma}\) Approximation for \(d \approx 0: B0\sigma \approx N\) Rule of Thumb: 4.122 bits per dimension | III-I |
| Point Capacity          | \(\displaystyle 2 3.7( ) 10 d c \approx \mu\)           | III-Q     |

**Table III-1** Important probability and information relationships, where **N** is the number of dimensions, \(\mu \approx \frac{N}{6}\) and \(\sigma \approx \frac{7}{120} \approx 0.2415\).

The distance **d** between two randomly selected pair of points in a real **unit N-cube** follows a **normal distribution**, as shown in the first row of Table III-1.

Here **μ** represents the standard distance, \(\frac{N}{6}\), and **σ** is the standard deviation of that expected distance, or about \(\frac{7}{120}\).

The fact of normal distribution allows us to derive several approximations for the **probability of proximity**, which is the probability that two randomly selected points, **p₁** and **p₂**, will lie closer together than distance **d**.

This in turn leads to expressions for the **information content** or **bit content of proximity** and the **point capacity** of a unit N-cube, which is the number of points that we can select at random and still be essentially certain that no two of them will be closer together than distance **d**.

Here is the significance of these relationships. For a given real unit N-cube, one can select a truly huge number of points at random with near certainty that all pairs of them will lie very nearly the same distance apart.

The **proximity of points** that are not about the same distance apart carries significant amounts of information. These facts make available an enormous number of random points that can be used to represent information, and points that are found to be close to known points are almost certainly not random.

These ideas lie at the heart of the **correlithm object computing paradigm** that will be developed in the rest of this book.

## Distribution of Distances

The **distances** between pairs of points in a **unit N-cube** are **normally distributed**. The distribution is shown in **Equation III-A**:

**Equation III-A**  
\[
normal(d) = \frac{1}{\sigma \sqrt{2\pi}} e^{-\frac{(d-\mu)^2}{2\sigma^2}}
\]

where  
- \(d\) is the distance between two points, \(0 \leq d \leq N\)  
- \(\mu\) is the **mean** or **expected distance** between two points, approximately equal to \(\sqrt{N/6}\)  
- \(\sigma\) is the **standard deviation** of the expected distance, approximately equal to \(\sqrt{7/120}\)

**Figure III-1** provides examples for various values of **N** showing Gaussian distributions. The means grow with the square root of **N**, and the standard deviations are constant.

---

## Probability of Proximity

The **probability** that two points selected at random within the unit **N-cube** lie no further than distance **d** apart, called the **probability of proximity**, is given by **Equation III-B**:

**Equation III-B**  
\[
prob(dist(p_1, p_2) < d) = \int_0^d normal(x) \, dx
\]

where \(dist(p_1, p_2)\) is the distance between points \(p_1\) and \(p_2\) in the unit **N-cube**.

**Figure III-2** illustrates the probability as the area under the normal curve between zero and distance **d**. The **normal curve** peaks at **standard distance** (\(StdDist\)).

The integral of a normal distribution lacks a closed analytic form but is expressed using the **complementary error function (erfc)**, defined in **Equation III-C** (valid for \(x < 0\)):  

**Equation III-C**  
\[
normal(x) = \frac{1}{2} erfc\left(\frac{x}{\sqrt{2}}\right)
\]

where \(x = \frac{\mu - d}{\sigma}\) (units of standard deviation).

Substituting into Equation III-B gives **Equation III-D**:

**Equation III-D**  
\[
prob(dist(p_1, p_2) < d) \approx \frac{1}{2} \left[ erfc\left(-\frac{x}{\sqrt{2}}\right) - erfc\left(\frac{\mu}{\sqrt{2}\sigma}\right) \right]
\]

The second \(erfc\) term is negligible unless \(d\) is very close to zero, so it can often be ignored.

Various approximations to \(erfc\) exist. For regions where \(x\) is more than two or three standard deviations from the mean, **Equation III-E** provides a useful approximation:

**Equation III-E**  
\[
erfc(x) \approx \frac{e^{-x^2}}{x\sqrt{\pi}} \left(1 - \frac{1}{2x^2} + \frac{3}{4x^4} - \cdots \right)
\]

The middle expression's right-hand term approaches 1 as \(x \to -\infty\), so it can be set to 1 to simplify.

Substituting this into Equation III-D yields **Equation III-F**, a good approximation for the probability of proximity:

**Equation III-F**  
\[
prob(dist(p_1, p_2) < d) \approx \frac{\sigma}{\sqrt{2\pi}(\mu - d)} e^{-\frac{(\mu - d)^2}{2 \sigma^2}}
\]

---

## Information Content of Proximity

The **information content in bits** (or **bit content**) of an event with **probability** \(prob(x)\) is defined as \(-\log_2(prob(x))\).

The **bit content** for the probability that the distance between two points is less than **d** is given by **Equation III-G**:

**Equation III-G**  
\[
BitContent(d) = -\log_2 \left(prob(dist(p_1, p_2) < d) \right) = -\log_2 \left( \frac{\sigma}{\sqrt{2\pi}(\mu - d)} e^{-\frac{(\mu - d)^2}{2 \sigma^2}} \right)
\]

Equation III-G can be simplified assuming \(\sigma\) is nearly constant, leading to **Equation III-H**:

**Equation III-H**  
\[
BitContent(d) \approx \frac{(\mu - d)^2}{2 \sigma^2 \ln(2)}
\]

This holds true as long as \(d\) is not extremely close to zero.

For \(d \to 0\), the bit content approaches a maximum, estimated by Equation III-I:

**Equation III-I**  
\[
BitContent(d=0) \approx 4.122 \times N
\]

This is a **rule of thumb** implying each dimension can carry a little more than 4 bits.

Other useful forms for probability of proximity include **Equation III-J**:

**Equation III-J**  
\[
prob(dist(p_1, p_2) < d) \approx 7.45 \times 10^{-2} e^{-\frac{d^2}{24.7}}
\]

---

## Point Capacity

Suppose we select \(c\) random points in the unit **N-cube**. Define \(d_m\) as the **minimum distance** between any pair among these points.

The problem: How large must \(c\) be before the probability that \(d_m < d_0\) exceeds some value \(p\), e.g., 0.5?

This value \(c\) for probability \(p\) is a measure of the **point capacity** of the unit **N-cube**.

At each stage (step \(c\)):  
- The total number of distance comparisons is \(\frac{c(c-1)}{2}\)  
- Probability all pairs are **greater** than \(d_0\) is given by **Equation III-K**:

**Equation III-K**  
\[
prob(\text{all } d > d_0) = (1 - prob(d \leq d_0))^{\frac{c(c-1)}{2}}
\]

This has the form (Equation III-L):  

**Equation III-L**  
\[
p = (1 - a)^k
\]

where \(0 \leq p \leq 1\), \(a\) is small (probability that two points are closer than \(d_0\)), and \(k\) is a positive integer.

Expanding (Equation III-M):

**Equation III-M**  
\[
p = \sum_{i=0}^{k} \binom{k}{i} (-a)^i = 1 - k a + \text{higher order terms}
\]

Ignoring terms with \(a^2\) and above, solving for \(k\) gives (Equation III-N):

**Equation III-N**  
\[
k = \frac{1-p}{a}
\]

Substituting \(a\) from Equation III-J yields (Equation III-O):

**Equation III-O**  
\[
k = \frac{1-p}{7.45 \times 10^{-2} e^{-\frac{d^2}{24.7}}}
\]

Because \(k = \frac{c(c-1)}{2}\), for large \(k\) approximate as \(c^2/2\), giving (Equation III-P):

**Equation III-P**  
\[
c \approx \sqrt{ \frac{2(1-p)}{7.45 \times 10^{-2} e^{-\frac{d^2}{24.7}}} }
\]

This shows the number of points \(c\) that can be selected before probability \(p\) of finding two points closer than \(d\) becomes significant is **very large**.

---

### Example

For \(N=96\),  
\[
\mu \approx \sqrt{96/6} = 4
\]  
Taking \(d = 1.0\) and probability \(p = 0.01\),  
\[
c \approx 3.7 \times 10^{33}
\]  
This means selecting \(10^{33}\) points results in a 99% chance of **no pair** closer than distance 1. 

---

### Rule of Thumb for Dimensions

- Use **N ≥ 20** dimensions minimum  
- At \(N=20\), standard distance is about 1.83 (\(~7.56\) standard deviations)  
- To ensure points are at least **6 standard deviations** apart requires \(d \approx 0.38\) producing \(c \approx 60\) million points  
- Larger \(N\) such as 50 or 100 reduces confusion between **random** and **significant** proximity  
- Higher dimensionalities yield very large point capacities  

**Figure III-3** shows probable maximum point capacities against \(N\) in a logarithmic scale.

---

## Chapter IV Elements of Correlithm Object Computing

The **correlithm object (CO)** system exhibits **amazing power and flexibility**. This flexibility allows it to do things traditional systems reliant on exactitude cannot.

A traditional computer is analogous to a **standard automobile**, which runs well on roads but is nearly worthless **off-road**.

## Introduction to CO Systems

A **CO system** is more like an **SUV**, which works very well both **on the road and off-road**. Traditional machines are **precise**. CO systems can also be **precise**, but they are most at home with **ambiguity**.

The type of processing found in a **CO system** offers a broader spectrum of **problem-solving capabilities** in situations where behavior must be adapted to existing conditions and where **similarity must be judged**.

A **CO system** uses processes that are remarkably similar in nature to the human analytical capabilities of **deduction** and **induction**.  

## Key Elements of CO Systems

To understand **CO systems**, one must first understand key elements of these systems. In this chapter, we introduce the key elements of the **Correlithm Object (CO)** computing paradigm, a new approach to computation built on the concepts of the previous chapters.

The **CO computing paradigm** offers new and significant **capabilities**. We will explore these in some detail in later chapters, but first we must establish a common **vocabulary** and introduce some needed **notation**.

---

## Perspectives on CO Computing

There are **two ways** to think about **CO computing**, because there are two ways to think about a **point in space** (See Figure IV-1):

- In one sense, a **point** is a single **object**, a "**thing**" that can be treated as one **monolithic entity**.
- A **point** can also be viewed as a set of **coordinates**—a list of numbers—relative to a particular **coordinate system**.

Both of these perspectives are important, but they emphasize different things, and sometimes one of them may be more appropriate than the other.

---

### Figure IV-1: Point Representation in Space

- X
- Y
- Z

**Point**

## Figure IV-1  
**The same point viewed in object form and in coordinate form.**

We will employ both of these perspectives at various times. The **“point” perspective** turns out to be more of a **“software” perspective**. It focuses attention on what a **CO computing system** is accomplishing.

In a sense, the **“point” perspective** is at a higher level of abstraction than the **“coordinate” perspective**, which is more of a **“hardware” perspective** that tends to focus attention on how a **CO computing system** in fact accomplishes what it does.

The **CO computing paradigm** represents data using **high-dimensional points** that are typically selected at random in a bounded **N-space** and manipulates data by following known pathways that connect these points. 

A critical characteristic of the **CO paradigm** is its recognition that the **probability or information content** of data is as important as the data itself. The **CO computing paradigm** represents **probability** or **information content** by **proximity to known points**.

**Point = (X, Y, Z) = (0.5, 0.4, 0.6)**

---

## Chapter IV Elements of Correlithm Object Computing  

### Spaces

In its most general form, the **CO computing paradigm** is defined within a bounded **N-space**, which is simply a space of **N dimensions** where every possible straight line is of finite length. Here we will limit our attention to the particular bounded **N-space** called the **real unit N-cube**, an **N-dimensional cube** with an edge length of one.  

(See Figure IV-2.)

---

### Figure IV-2  
**Visualization of the N-dimensional unit cube**

- The **origin** is at the lower left corner.  
- The light gray lines radiating from the corner suggest the **N dimensions** that meet at each corner.  
- Two adjacent corners are shown with **unit length** between them.  
- The corner at the top of the figure is the **opposite corner to the origin**.  
- The gray perimeter lines and cloudy background suggest the **high-dimensional cubic shape**.  
- A randomly selected **interior point** is also shown.

---

For present purposes, we require that all **coordinates** of all points are members of the set of **real numbers between 0 and 1 inclusive**, and we place the **origin at one corner** of the **N-cube**.

Other choices for the type and range of numbers used to define the bounded **N-space** and for the location of the origin are also important in some forms of **CO computing**.  

- One important choice is to place the **origin at the midpoint** of the **N-cube**.  
- Other important choices are to employ **integers**, **complex numbers**, or even more exotic number types.  

We will not pursue such alternative choices here.

---

We focus on the **CO computing paradigm** in a **real unit N-cube**.  

- There are **N dimensions** available.  
- We often use “dimension” and **cell** interchangeably.  
- A particular position within a dimension is called a **coordinate**. Its value must be a **real number no less than zero and no greater than one**.

---

A **space** is any set of dimensions and a **subspace** is any subset of a space. Similarly, a **ply** is any set of cells and a **subply** is any subset of cells. 

- The **full space** is the set of all available dimensions.  
- The **full ply** is the set of all available cells.  

Often it is clear from context that a particular set of dimensions or cells is a subset of another set, and we omit the “sub” prefix.

We give names to spaces as we define them. These names may be anything, but often follow the common programming rules for identifiers.

---

We select a **space** by choosing a specified number of dimensions from the **full space**. For simplicity, all of the spaces we choose here will be **disjoint**, meaning they have no dimensions in common, unless otherwise stated.

Normally, we select a space by choosing a specified number of dimensions **at random** from the full space. This method can of course result in **overlap** (dimensions in common) between two or more spaces. 

Although it may appear otherwise, overlap is usually **not a problem**. In fact, we will see later that overlap can be a **useful property**.

When working with the **CO computing paradigm**, one should normally resist the urge to use **non-random methods**, because many of this paradigm’s primary properties emerge from **stochastic processes**. One should only use **deterministic methods** when it is very clear why stochastic methods are wrong for a particular case.

---

We will sometimes need to establish **set-theoretic relationships** among various plies.

We can define a **ply** as the **intersection** or **union** of (the dimensions of) two other plies.  

- For example, we might define ply **r** as the **intersection** of plies **p** and **q**.  
- Or we might define **r** as the **union** of plies **p** and **q**.  
- The **complement** or logical **NOT** of a ply is all defined dimensions that are **not found** in the ply.

For example, given plies **p** and **q**, we can define ply **r** as the **“complement of q with respect to p”**, or as the **“remainder of p minus q”**, or simply as **“p and not q”**. Thus **r** would be all of the dimensions of **p** that are not in **q**.

---

### Points

The most fundamental operation in the **CO computing paradigm** is the definition of a **point**. We often use **“point”** and **CO (Correlithm Object)** interchangeably.

To define a point, we specify:

1. The particular **space** that will contain the point.  
2. The **coordinates** of the point within that space.

Just as a space can be selected randomly or deterministically, so can the coordinates of a point. We usually select coordinates **randomly**, but note that there are important exceptions to this that will come up later.

---

We give names to points as we select them. As with spaces, these names may be anything, but often follow the common programming rules for identifiers.

Since we can define many points within a given space, it is convenient to use a notation that gives both the **name of the space** and the **name of the point** within that space.

---

For example, see Figure IV-3. Suppose we have two spaces (plies) named **p** and **q**.

- Let us identify two points in **p** named **c** and **d**  
- One point in **q** named **c**

Refer to these three points as **p.c**, **p.d**, and **q.c** respectively.

**Point q.c** is a different point than **point p.c** because they are in different spaces. The identification of a...

## 4 Case Example: Omitting the “sub” Prefix

This is an example of a case where it is clear from context that we mean **subspaces** or **subplies**, and the "**sub**" prefix is omitted.

---

## 4.2 Correlithm Object Technology

### Notation for Points in Spaces

A **particular point** requires both the **name of the space** and the **name of the point** within that space.

- **Full Ply**
- **Ply p**
- Points:  
  - **p.c**  
  - **Ply q**  
  - **p.d**  
  - **q.c**

**Figure IV-3** shows a full ply with two of its **subplies**, p and q, identified. Three points are shown: **p.c** and **p.d** in ply p, and **q.c** in ply q.

---

### Soft Tokens

Points in spaces (**COs in plies**) are used to represent **all data** in the **CO computing paradigm**. 

**Associating a CO with data** is a very **fundamental notion** and is the **sole means** available for representing data. Far from being limiting, this **one-data-type idea** supports a wealth of new computing capabilities.

A **CO** is analogous to a **storage location** in a traditional computer, with the associated data as the contents of the location. However, this analogy is misleading because the **CO does not store the data**; it **represents** the data.

A **CO that is representing data** is sometimes called a **soft token**.

As seen in **Chapter III**, it is very unlikely that two COs selected at random in the same ply will be in close proximity. If two COs are close, it is almost certainly not by random selection but due to some **non-random process** producing both, with noise accounting for small variations in position.

---

### Analogy of Close Proximity

This principle is analogous to a shooting gallery scenario:

- Two shooters: one **amateur** and one **expert**
- A shot hits very near the **bullseye** (Figure IV-4)
- It is more likely the expert fired the shot rather than the amateur, though not absolutely certain

Similarly, in a ply **p**, if we see CO **p.c** and another CO **p.c'** very near **p.c**, it is more likely the **same noisy deterministic process** produced both COs rather than one being picked at random.

---

### Figure IV-5: Proximity of COs

- Ply **p**
- Known CO: **p.c**
- Nearby CO: **p.c'**
- Question: Did **p.c'** come from a random process or the same noisy process as **p.c**?

---

### Quantitative Analysis (Chapter III)

Let the number of dimensions in ply **p** be **100**.

- Distance between **p.c** and **p.c'** is **0.5**
- Expected distance \( \mu \) between two random points in **p** is about \( \sqrt{100/6} \approx 4.08 \)

The probability of a random proximity of 0.5 or less is given by:

**Equation IV-A**  
\[
prob(dist(p.c, p.c') < 0.5) \approx 7.45 \times 10^{-10}
\]

This means the probability that **p.c'** came from the random process is very close to zero. Assuming the noisy deterministic process made the pick is almost always correct.

---

### Alternative Possibility

The shot or CO **p.c'** could have come from a **third, unknown source**, but in absence of information, this is not assumed. 

Given exactly two possible sources, probabilities are assigned using a **Bayesian-like approach**, extensively used in the CO computing paradigm.

---

### Zones of Influence and Soft Tokens

- **Random COs** essentially never appear near a particular CO **p.c** by accident.
- A CO **p.c'** found nearby is probably a **noisy version** of **p.c**.
- The **same process** likely produced both COs; their separation is due to noise.
- If **p.c** represents a particular data item, then **p.c'** probably does too.
- This gives **p.c** a kind of "**zone of influence**" (Figure IV-6).
  
#### Figure IV-6: Zone of Influence

- The **N-space** is shown as a 2D mottled plain.
- The **small sphere** at the center represents a **CO**.
- The **cone** grows quadratically in information bits as we approach the CO.
- The **closer** one gets, the more bits proximity is “worth.”

A CO represents data **not only at exact coordinates** but also to a **decreasing degree at nearby coordinates**. It establishes a region in its ply with **fading boundaries**, like gravity fading from a star.

COs are **tokens without identifiable edges**; instead, there is a **gradient of information relevance** that falls quadratically with distance (see **Equation III-I**).

---

### Known Points and State COs

- **Known points** in a space are called **pattern COs**.
- These are **landmark-like positions** and centers of known soft tokens.
- In a ply, there exists a **time-varying point** defined by the instantaneous values of the cells comprising the ply, called a **state CO**.
- The **state CO** can travel to any location in the ply over time.

---

### Nature and Power of Soft Tokens

The concept of a **soft token** is powerful:

- Data representations are no longer necessarily **exact**, **sharp-edged**, or **brittle**.
- Instead, **pattern COs** co-opt the regions surrounding them to represent data.
- These regions define an **information field** varying inversely with the square of distance, like a **gravitational field**.
- All data represented by a soft token has an **inherent probabilistic nature**.
- CO data representations are **approximate and robust**, **unambiguous yet in shades of gray**.
- They are inherently **soft and resilient**, not **hard and sharp-edged**.

Using soft tokens to represent data fundamentally changes the computation model from **brittle and exact** to one with **flexibility, accommodation, and forgiveness**.

---

### The “Field” and Data Confidence

- A CO represents a **data object**.
- The “field” around it defines its **territory**.
- Any CO found in this territory is **quantifiably likely** to represent the same data object.
- The "**softness**" adds **generality**: soft tokens can represent exact data, but the information field is always present, representing the data and **confidence** in that data.

---

## Rules

A **rule** is a **directed line segment** from a set of **soft tokens** called the **source** to a set of soft tokens called the **target**.

Rules perform **all data manipulation** in the CO computing paradigm.

Suppose we have two COs, **s.c** and **t.d** (Figure IV-7).

- **s.c** represents CO **c** in ply **s**.
- **t.d** represents CO **d** in ply **t**.
- Both **s** and **t** must have enough dimensions for CO stability.
- Points **c** and **d** are selected at random within **s** and **t**, respectively.

---

### Rule Notation

Consider the statement:

**Rule IV-1**  
\[
s.c >: t.d
\]

## 5 The Soft Tokens of the CO Computing Paradigm and the “Fuzzy Logic” of Zadeh

**Soft tokens** of the **CO computing paradigm** and the **“Fuzzy Logic”** of **Zadeh** have similarities.  
[Zadeh, L.A., "Fuzzy sets," Information and Control, Vol. 8, pp. 338-353, 1965, and subsequent literature.] Soft tokens can be used to implement **Fuzzy Logic**. But soft tokens form the basis for the **CO computing paradigm**, which is a complete computational model with significant new capabilities.

### Correlithm Object Technology

#### Rule Mapping

```
Space s         Space t
s.c             Rule t.d
```

Figure IV-7: Here is a rule that associates point **s.c** in a **source space** to point **t.d** in a **target space**. The operator **“>:”** is the **rule operator**. It separates the terms **s.c** and **t.d** in the rule.

- Terms on the **angle bracket side** of the operator are collectively called the **source** of the rule.
- Terms on the **colon side** of the operator are collectively called the **target** of the rule.

The statement in **Rule IV-2** is entirely equivalent to the one in **Rule IV-1**.

**Rule IV-2**  
`t.d :< s.c`

Some people prefer their rules to read from **left-to-right** as in **Rule IV-1**, and some prefer **right-to-left** as in **Rule IV-2**. This difference stems from the origin of this notation rooted in the productions of **formal grammars**. We will use the left-to-right form **Rule IV-1** here.

---

**Rule IV-1** instructs us to watch the region around **s.c**. If a point **s.c’** appears near **s.c**, we must generate a new point **t.d’** near **t.d**. (See Figure IV-8.)

#### Mapping

```
Rule

t.d’          t.d
            s.c’         s.c
```

Figure IV-8: The rule that maps point **s.c** to point **t.d** causes **s.c’** near **s.c** to map to **t.d’** near **t.d**.

It is common to call **t.d** the **name** of **s.c**. More generally, the **target** of a rule is the **name** of the **source** of the rule.

---

### Strings

We can define a set of **COs** in a **ply** by using one or more **subscripts**, as in `p.ci, i=1..k` or `p.di,j, i=1..k, j=1..m`. A set of COs with subscripts is called a **string CO** or simply a **string**.  

- **Subscripts** establish an **ordering** or **geometric relationship** among the COs.  
- One subscript implies a **one-dimensional** ordering, so the term "string" seems reasonable.  
- Two subscripts imply a **two-dimensional** relationship, where the term string is less applicable, but still used.  

Consider the 1-dimensional string CO `p.ci, i=1..k`. Suppose these COs are built such that the distance between `p.ci` and `p.ci+1` is **less than standard distance**, done so that it does not affect the standard distance between each of them and any other random CO in **p** such as `p.q`.  

This means two COs that are close together by **subscript** are also close together by **distance**. (See Figure IV-9.)

```
p.ci   p.ci+1
p.ci-1
Distance
```

Figure IV-9: Distance from **p.ci** to other members of the string CO, showing how distance in N-space increases to standard distance as distance in string CO sequence grows.

In general, **string COs** are used to **embed lower dimensional sampled geometries** into the higher-dimensional subspaces used in the CO computing paradigm. The **adjacency** found in the samples is reflected in the COs.

- A common use of the **1-dimensional string CO** is to capture the **time axis** in a series of time samples.  
- A common use of the **2-dimensional string CO** is to capture **proximity of objects in an image**.

---

It is common to set up rules such as:

**Rule IV-3**  
`p.ci >: q.d, i=1..k`

This rule is really a set of **k rules** that map every CO **ci** in **p** to the single CO **d** in **q**. (See Figure IV-10.)

```
p.ci
q.di
```

Figure IV-10: Here we associate the CO elements of a string CO in space **p** with an equivalent number of CO elements in space **q**.

This lets the single CO, **q.d**, stand for the entire string CO by making it stand for any of the members of the string. Thus:

- We can name a **single CO** with a CO as in Rule IV-1.  
- We can also name a **string** with a CO as in Rule IV-3.  

---

We normally expect COs adjacent by subscript in a string CO to be closer together than the standard distance, but:

- It is still a string CO if subscript-adjacent COs are at **standard distance**.  
- It is also a string CO if they are farther apart than standard distance.

---

### Sensors and Actors

A system built on the **CO computing paradigm** represents **all internal data using COs** and performs all **data manipulation using rules**. There are many kinds of data besides COs.

To get data into and out of a CO system, we must **convert between world data and COs**. This is done using **converters**:

- **Sensors**: Special rules that have **world data sources** and **CO targets**.  
- **Actors**: Special rules that have **CO sources** and **world data targets**.

Converters come in various types:

- **Cardinal, ordinal, and continuous converters** handle world data with these characteristics.  
- **Quantized converters** (and rules!) "error-correct" their world data to specific values.  
- **Interpolating converters** deal with both world and CO values between known values.

The subject of converters is extensive and will be revisited later.

---

### Systems

A **system** in the CO computing paradigm, typically called a **CO system**, is a **set of rules**. These typically include:

- **Converter rules**  
- **Internal rules** that map **COs to COs**.

Programming a CO system involves defining the **rules** and the **elements** that compose them.  

- Often details of the elements can be **defaulted** since many times the only requirements are that the elements be "large enough" and have required **amount and type of stochastic behavior**.  
- Building CO systems that deviate from these defaults is an advanced topic.

Rules are **chained together** to build programs. Consider the example:

**Rule IV-4**  
```
p.c >: q.d
q.d >: r.e
```

- If the first rule fires (meets execution conditions), then a point near **d** in space **q** comes into existence.
- This causes the second rule to fire, bringing a point near **e** in space **r** into existence.

This trivial example shows how **rule chains** are formed.

---

Rule sets can be much more complex:

- Rules with **two or more terms** in source and target (Rule IV-5).  
- **Feedback loops** are frequently employed (Rule IV-6).

**Rule IV-5**  
`p1.a p2.b p3.c >: q1.d q2.e q3.f`

**Rule IV-6**  
`p1.a p2.bi >: p2.bi+1, i=1..k`

---

### Miscellaneous

As CO systems become complex, it is often convenient to build **re-usable subsystems**, generically called **parts**.

- A **part** is a set of rules defined as a **single object**.  
- A part can be **instantiated repeatedly** and “wired into” the CO system.  
- A part can itself contain parts. This means a part can be **hierarchically composed**.

The notation for a part is a wrapper around a set of rules:

```
<part_name> { (one or more rules or parts) }
<instantiation_name> = <part_name>
```

- `<part_name>` is the name of the object class.  
- First example shows generic definition of a part.  
- Second shows instantiation.  
- `<instantiation_name>` can be used as a prefix to access terms of rules inside the part externally.

Examples:

```
part1 {s1.p1 >: s2.p2 }
part2 { s1.p1 >: s3.p3 }
part3 {
  pt1 = part1
  pt2 = part2
  s1.p1 >: pt1.s1.p1
  pt2.s3.p3 >: s2.p2
}
```

- `part1` and `part2` each define parts with one rule.  
- `part3` instantiates `pt1` and `pt2` as `part1` and `part2` respectively.  
- Points with prefixes reference unique points inside these parts.

A **lobe** is usually—but not always—a part with **no converter rules**.  
A **Synthorg®** is the term for a complete CO system, especially one that only accepts or produces **world data**; i.e., a CO system where all COs are internally hidden and only world data interfaces are visible.

---

## Chapter V Correlithm Object Machines

The level of the **computing machine model** presented here is roughly analogous to the **machine language level** of a traditional computer. The model is a **formal description** of a **CO computing system**. It is useful from both a **theoretical perspective** and as a basis for **engineering real CO systems**.

We have built and tested implementations of this model under actual use conditions with **outstanding results**.

Here we present a model of the computing machine implied by the **correlithm object (CO) computing paradigm** at a level roughly analogous to **machine language** of a traditional computer. (See Figure V-1.)

---

### Basic CO Machine Operation

```
Space S: State Point P(t)
Subspace s1
Subspace s2
Pattern Point s1.p1
Pattern Point s2.p2
Distance d1
Distance d2
Mapping Caused by Rule
s1.p(t) >: s2.p(t+1)
State Point s2.p(t+1)
Rule: s1.p1 >: s2.p2
State Point s1.p(t)
Space S: State Point P(t)
```

Figure V-1: The rule **s1.p1 >: s2.p2** maps **s1.p(t)** to **s2.p(t+1)**.

- A **single, time-varying N-dimensional point P(t)** carries the **state** of the machine.  
- This point can be specified as a time-varying set of **N coordinates** defined in a real unit **N-cube**, the bounded space **S**.  
- Any subset of these coordinates is a **subset of P(t)** viewed as state point **p(t)** in a **subspace s** of the N-cube, or **s.p(t)**.

---

### Rules and State Transition

- A **rule** maps **s1.p(t)**, the current state point in subspace **s1** (called the **source subspace**) to **s2.p(t+1)**, the next state point in subspace **s2** (called the **target subspace**).  
- It refers to two **time-invariant points**, **p1** and **p2** (called **pattern points**) in **s1** and **s2**, respectively — **s1.p1** and **s2.p2** — which are maintained within the rule.  
- Effectively, a rule **watches** the movement of the state point in its source and controls the movement of the next state point in its target.  
- The rule moves the controlled point so that its next distance **d2** to the target pattern point reflects the current distance **d1** between the watched source point and the source pattern point.  
- This movement mechanism will be discussed in more detail shortly.

---

### Cells and Execution

- A **cell** is the usual name for the storage of a single dimension of the N-cube.  
- A cell holds **one coordinate** of the state point **P(t)** at time **t**.  
- A cell acts like a **memory location** in a traditional computer that stores one real number in the range **0.0 to 1.0**.  

Each rule is a **self-contained, independent object** that operates on the cells independently of other rules. The machine is **clocked**, and during one clock cycle, **every rule is executed or “cycled” once**.

## Cycle Process in Rule Examination

During a **cycle**, a **rule** examines its **source subspace** or set of **cells**, compares the **state point** it finds there to its **source pattern point**, and generates a...

## 6 Real Numbers in CO Computing Machines

**Real numbers** are used in this particular **CO computing machine**. Almost any type of number may be used if used consistently, including:

- **Binary**
- **Integer**
- **Rational**
- **Complex**
- More exotic forms such as **qubits** and **ebits** from **QIS (Quantum Information Science)**

Some types have unique advantages.

---

## Chapter V Correlithm Object Machines

A **target state point** considers **source proximity** and its **target pattern point** and writes the generated state point to the cells of its **target subspace**.

A rule sets the **next time step values** of cells in its target from the **current time step values** of cells in its source. This apparently requires the existence of two cell arrays:

- A **current cell array**
- A **next cell array**

This prevents later-executing rules within one cycle from possibly using results produced by earlier-executing rules within the same cycle.

---

The correct overall behavior of a **CO machine** is often remarkably insensitive to the exact details of its operation. 

- Two cell arrays are not usually necessary.
- **Incremental update methods** exist.
- All rules do not have to fire within a given cycle.
- Rules can both read and write the same cells.
- They can fire with little regard to the time of firing of other rules, providing certain conditions are met.

These are advanced topics that will not be covered here. Instead, here we assume:

- Two cell arrays exist.
- Rules read from the **current array** and write to the **next array**.
- All cells execute exactly once in every **machine cycle**.

Shortly, we will consider what happens when two rules try to set the values of the same cells in the next array. Far from being a problem, what results is a valuable strength of a CO machine.

---

## Definition of a Correlithm Object Machine

Here is a formal definition of a **correlithm object machine**.

**Definition:**  
A correlithm object machine is a **tuple (c, r)**, where:

- **c** is a set of **n cells**, { ci }, i = 1..n  
- **r** is a set of **k rules**, { rj }, j = 1..k  
(See Figure V-2.)

Each cell has:

- A **unique address**, i  
- A **time-varying state**, ci(t), where **0.0 ≤ ci(t) ≤ 1**

A cell corresponds to one unique dimension of a bounded **N-space**. The **unit N-cube** is used here as the bounded N-space, though other bounded N-spaces could be used.

---

### Figure V-2: The Correlithm Object Machine

- All **rules** are executed in parallel at every time step.
- A rule reads a **state point** from a **source subset** of cells (such as s1) at time t.
- It writes a **state point** to a **target subset** of cells (such as s2) at time t+1.
- A rule writes a randomly selected state point in the target whose proximity to the target pattern point s2.p2 reflects the proximity of the source state point to the source pattern point s1.p1.

---

A **ply, p**, is set of **M cell (or dimension) addresses**, { am }, m = 1..M. The set of values in the particular cells referenced by a at time t, { ci(t) }, is called the **state CO** for ply p at time t.

**Pattern COs** are **time-invariant points**.

Rules store **pattern COs** internally and relate those pattern COs to **state COs**.

---

### Simplest Rule Syntax

```
s1.p1 >: s2.p2
```

This rule can be read as:  
**“If the state point at the current time t in the ply s1, s1.p(t), is ‘near’ the ply s1 pattern point, s1.p1, then build a new state point, s2.p(t+1), for ply s2 that is ‘similarly near’ the pattern point s2.p2 in ply s2.”**

To do this:

- Find the distance, **d1**, that s1 lies from s1.p1.
- Choose a new state point, s2.p(t+1), based on pattern point s2.p2 and that distance.

---

## Building a New State Point

There are various ways to use **s2.p2** and **distance** to choose the new state point:

- Choose s2.p(t+1) by randomly selecting a point in s2 at distance d1 from s2.p2.
- Express d1 in units of **standard deviation** and use that in s2.
  - This maintains **distance statistics** in s2, even if s2 has a different number of dimensions than s1.
- Maintain **f**, the fraction of standard distance in s1 represented by d1, by picking at random a point in s2 with the same fraction of standard distance from s2.p2.

Many other methods are possible.

---

## Interpolation Form of Rule

The distance d1 between s1.p(t) and s1.p1 in ply s1 of **D1 dimensions** is the square root of the sum of the squares of individual cell differences. (See Equation V-A.)

Similarly, the distance d2 between s2.p(t+1) and s2.p2 in ply s2 of **D2 dimensions** is shown in Equation V-B.

---

### Equation V-A

\[
d_1 = \sqrt{\sum_{i=1}^{D_1} (s1.p_i(t) - s1.p1_i)^2}
\]

---

### Equation V-B

\[
d_2 = \sqrt{\sum_{i=1}^{D_2} (s2.p_i(t+1) - s2.p2_i)^2}
\]

---

If the distance between s1.p(t) and s1.p1 is **d1**, suppose we pick a point for s2.p(t+1) at random from the set of points in s2 at about distance d1 from s2.p2.

---

### Standard Distance and Fraction f

- Standard distance in s2 is \( \sqrt{S_2/6} \), where \( S_2 \) is the number of dimensions in s2.
- Define fraction \( f = d_1 / \sqrt{S_2/6} \), where **f** is the fraction of standard distance represented by d1 in s2.

---

Suppose we generate a random point, s2.q, in s2, then interpolate from s2.p2 to s2.q by fraction f.

Just interpolate each corresponding dimension:

\[
s2.p_i(t+1) = s2.p2_i + f \cdot (s2.q_i - s2.p2_i), \quad i=1..S_2
\]

Then s2.p(t+1) will be at about the right distance, d1, from s2.p2.

---

### Notes on the Interpolation Form

This interpolation form of rule is simple but does **not maintain certain statistics**, namely the probability that a point p1 chosen at random in the space will lie at a distance from the pattern point p2 no greater than distance d.

From **Table III-1** in Chapter III, we have:

\[
\text{prob}(\text{dist}(p_1,p_2) < d) \simeq 2d - 4.7 d^2 + 7.45 d^4 - 10 d^7
\]

Recall that:

- \( \mu = \sqrt{N/6} \) is the **standard distance**, where N is the number of dimensions in the space.
- The probability will be maintained only if **s1 and s2 have the same number of dimensions**.

## 7 Cell Form of Rule

There are many ways to obtain a good point. This is not normally the best way. It is presented here primarily because of its **simplicity**, which is useful for illustration.

We can quite easily compute a distance **d2** that maintains the same probability in **s2** that **d1** produces in **s1**. We set the probabilities in the two spaces equal to each other and solve for **d2**:

\[
\text{prob}(\text{dist in } s2(p1, p2) < d2) = \text{prob}(\text{dist in } s1(p1, p2) < d1)
\]

\[
7.45(2-2) \cdot 10^{-\mu} = 10, \quad 6, \quad 7.45(1-1)
\]

\[
d_2 = 1 d_1 + \mu_2 - \mu_1
\]

A rule such as **“s1.p1 >: s2.p2”** compactly expresses a set of rules of the form:

- \( s1.p1 >: s_i.c, \quad i=1..S2 \)

Where **si** represents each of the dimensions in **s2** in turn. This form of rule causes a single cell to be viewed as a **function over time**. The cell “watches” the movement of the state point, **s1.p(t)**, in a given space such as **s1** and produces a new state value for itself that is close to the given target value, **si.c**, as the state point is close to the source point, **s1.p1**.

This “cell form” of rule highlights the fact that the values taken on by a cell depend only on the values of the **source sides** of the rules that affect the cell. A cell’s next state value clearly must **not** depend on the values of other target cells that may have similar source-side rules.

### Target Cell Independence

Both methods require that each **target cell** knows the number of dimensions in both **s1** and **s2**. Knowing the number of dimensions in **s1** is fine. Two target cells may both be defined on the same source cells and remain completely independent.

However, requiring knowledge of the number of dimensions in **s2** means that every target cell in **s2** depends on every other target cell in **s2**. If even one of the target cells were to fail, the distances and statistics in **s2** would change, and so would the states of every remaining cell.

It is highly desirable for **target cells to remain independent** because:

- **CO systems** are inherently **parallel** in several ways that can be exploited.
- Target cell independence is one of these exploitable parallelisms, perhaps the **most important** one.
- The CO computing paradigm is a **statistical computing paradigm**.
- We want behavior to **emerge** from statistical processes.
- We prefer to select target cells for rules **statistically**, not algorithmically.
- It is unlikely for two target cells to participate in exactly the same set of rules.

This concept is **workable** and often **highly desirable**. The question of how to compute target cell values will be revisited shortly.

### Dimensions Appearing in Multiple Rules

So far, we have assumed a rule has a **single source space** and a **single target space**, and a particular cell appears in exactly one place, either source or target.

Things are usually more complicated.

#### Syntax of a Rule

Consider the following syntax:

- `<Rule> ::= <Source> >: <Target>`
- `<Source> ::= <TermList>`
- `<Target> ::= <TermList>`
- `<TermList> ::= <Term> | <TermList> <Term>`
- `<Term> ::= <Space> . <PatternPoint>`

From a semantic perspective, a **Space** is a set of dimensions and a **PatternPoint** is one time-invariant dimension value for each dimension in the Space.

A particular dimension will appear **at most once** in a Space’s dimension set. Spaces dimension sets are usually selected independently and often statistically, so two Spaces may have dimensions in common.

#### Computing When the Same Dimension Appears Multiple Times

How are rules computed when the same dimension appears in multiple Terms in the Source and the Target of any number of Rules?

- A **rule does not set a value** into a target cell.
- It **weights** the target pattern value from the Rule for that cell.
- After all rules provide weights to their target cells, each target cell **independently computes** its own new state value from accumulated weights and pattern values.
- Each target cell then **reinitializes** its weights.

A rule passes the **same weight** to each of its target cells. The weight is often the **reciprocal of the probability of distance**, as used above (from Table III-1 in Chapter III).

Each cell computes a new value as the **first moment** of the target values from each rule, weighted by corresponding weights.

The formula for a target cell’s new state value is:

\[
\text{new cell value}_i = \frac{\sum_k \text{weight}_{ik} \times \text{pattern value}_{ik}}{\sum_k \text{weight}_{ik}}
\]

\[
\text{weight}_{ik} = \frac{1}{10^{7.45(d_{ik}-\mu)}}
\]

If only one rule contributes to the target cell’s new value (i.e., \(k=1\)), then the new cell value is the pattern value regardless of the weight.

In practical systems, a **noise floor** may be injected at a pre-set weight so exact matches occur only when the rule’s weight exceeds the noise floor.

This method allows **each target cell to be independent**. The cell needs only its target pattern values and weights from various rules.

### Example Rule with Multiple Terms

Consider the rule:

\[
1.c.S1.p1 \quad s2.p2 \quad \dots >: s3.p3 \quad s4.p4 \quad \dots
\]

The source contains multiple terms. To evaluate the source and obtain a distance, use:

\[
\text{dist}(s,p,t) = \sqrt{\sum_i (s1.p(i,t) - s1.p(i))^2 + \sum_i (s2.p(i,t) - s2.p(i))^2}
\]

The part inside the square root is a sum of **independent squared differences**.

Even if the same cell appears in more than one defined subspace (e.g., both **s1** and **s2**), the formula is **unambiguous**.

- The same cell is acceptable in multiple subspaces because each appears only once per subspace.
- Different subspaces compare their state points to **different pattern points**.

A cell appearing in source terms of different rules is similarly **no problem**. Each cell is evaluated within its rule separately.

When the target in the rule contains multiple terms, the target cell accumulates **incremental contributions** to its final value during a cycle.

Therefore:

- A cell appearing in more than one term does **not** introduce ambiguities.
- A cell appearing in the target side of different rules accumulates contributions and incorporates all at cycle end.

### Remarks on the Formula Choices

Some may find the formula choices arbitrary.

The CO computational paradigm is based on **averages and approximations**, tolerating what would seem like high noise.

Target cells must calculate their next value **independently** of other target cells.

These formulas have a **firm basis in statistics and information theory** and have been tested with **outstanding results**.

The community might eventually prove these are the "right" formulas or discover better ones, but the current approach is an **engineering approach** that works effectively.

---

## Chapter VI Completeness

**Completeness** is an important theoretical concept.

Computation processes can be compared to a system of **towns and roads**:

- If we can only travel by road, are there towns we cannot reach?
- If we compute only with a particular type of computer, are there programs we cannot run?

If a computer has the **“completeness”** property, it can **run any program**.

Both **traditional computers** and **correlithm object (CO) computers** have this crucial **completeness** property. 

They also have lesser completeness properties including:

- Ability to compute any **logical proposition**
- Absence of the **perceptron limitation**

### Traditional vs CO Machines

**Traditional computers** follow the **Von Neumann computing paradigm**, featuring:

- Single processing unit
- Random access memory
- Serial execution (one task at a time)

**CO architecture** machines are:

- Highly **parallel** and **redundant**
- Many things happen at once on several levels

Both Von Neumann and CO machines are **general-purpose computers**.

While they excel at different tasks, each possesses the crucial **completeness** property.

This chapter establishes this property for **CO machines**, along with other related completeness properties from the **Theory of Computation**.

### Logical “NOT”

One of the simplest logical operations is **logical “NOT”**.

This function maps:

- **True** → **False**
- **False** → **True**

To code this for a CO machine:

- Establish two spaces: **s1** and **s2**
- Place two points in each space, representing **True** and **False**
- Write rules to connect them (see Figure VI-1):

```
s1.True >: s2.False
s1.False >: s2.True
```

#### Figure VI-1 Description

- Black points and arrows illustrate **pattern points** in both spaces and their mapping by the rules.
- White points and arrow show how a state point in **s1** near **True (T)** maps to a state point in **s2** near **False (F)**.

#### Operation at Time \( t \)

- State point **s1.state(t)** exists in **s1**.
- Setting **s1.state(t)** to **s1.True** makes:

  - Distance between **s1.state(t)** and **s1.True** zero
  - Distance between **s1.state(t)** and **s1.False** large

- As a result:
  - The first rule's target point **s2.False** is weighted strongly.
  - The second rule's target point **s2.True** is weighted very little.

- Therefore, **s2** cells take the values of pattern point **s2.False**.

- The next state point in **s2**, **s2.state(t+1)**, has the value **False** at the next cycle start.

In other words, **True maps to False**.

## Logical NOT Function in CO Machines

Similarly, if we set **s1.state(t)** to **s1.False**, then **s2.state(t+1)** will be forced to **s2.True**, and **False** maps to **True**. These two results are all that we need to have a logical **“NOT”** function.

As a practical matter, the operation of these two rules is such that any point in **s1** that is not almost exactly halfway between **s1.True** and **s1.False** will force the state point of **s2** to exactly one or the other state. The entire **s1** space is thus essentially divided into two regions, the one that maps to **s2.True** and the one that maps to **s2.False**.

## Logical Completeness

**CO machines** are **“logically complete.”** Some people prefer the equivalent term **“Boolean complete.”** Either way, a system that has this property can implement every **logical expression** or **Boolean function**. Such a system can be used to build any logical function.

To show that a system is logically complete, we only need to show that the system can implement one set of logical functions that are known to be logically complete. That implementation can then be used to build any other logical function.

### Logical “NAND”

Several different sets of logical functions are commonly used as the basis for a claim that a system is logically complete. One such set is the logical **“AND,”** the logical **“OR,”** and the logical **“NOT.”**

We have already shown how to implement logical **“NOT”** using a CO machine. We could proceed by showing how to implement these other two logical functions, but this would require a total of twelve rules: two for the **“NOT”**, and four each for the **“AND”** and **“OR.”**

Another logically complete set of logical functions is the single logical function **“NOT-AND”** or **“NAND.”** Early computers were implemented largely with **“NAND”** functions because **“NAND”** functions were the simplest and easiest to build with the electronic components of the day.

Here is the **“NAND”** function as a set of CO rules. The pictorial equivalent is shown in Figure VI-2.

```
s1.F s2.F >: s3.T  
s1.F s2.T >: s3.T  
s1.T s2.F >: s3.T  
s1.T s2.T >: s3.F
```

### Figure VI-2

Here is the logical **NAND** function. Pairings of points in **s1** and **s2** map to points in **s3**. The white circles represent state points, illustrating here that **s1.T** with **s2.F** maps to **s3.T**.

The **“NAND”** code consists of four rules. Each rule has two tokens on the source side and one on the target side. Consider the first rule:

`s1.F s2.F >: s3.T`

- The first term on the source side of the rule, **s1.F**, represents a **soft token,** the randomly selected point **F** in the subspace **s1**.
- The second term, **s2.F**, represents a distinct soft token, another randomly selected point **F** in the subspace **s2**.
- Notice there is no ambiguity about calling both points by the same name, **“F,”** because they are in different spaces.
- Similarly, the term on the target side of the rule, **s3.T,** is a label for a third distinct soft token, a randomly selected point **T** in subspace **s3**.
- The rule instructs the system to watch for the appearance of both of the two source soft tokens in their respective subspaces. Whenever that happens, the rule causes the generation of the designated target soft token.

Similar descriptions apply to the other three rules. This set of four rules effectively implements the functionality of a **“NAND”** gate.

**CO machines** are therefore **logically (or Boolean) complete,** and any logical function can be built using a CO machine.

## No Perceptron Limitations

The **Perceptron** is an early computing paradigm with biological origins. In **1969**, it was shown that this model could not compute certain important functions.

Because the **CO architecture** also has biological origins, it is important to emphasize that it does **not** suffer from the limitations found in Perceptrons. These limitations derive directly from the fact that a Perceptron is a **linear device**.

It has been amply demonstrated in the field of **Connectionism** or **Artificial Neural Networks (ANNs)** that relaxing this linear constraint removes the limitations on what Perceptrons can do.

To show that a system does not have these Perceptron limitations, we only need to show that the system can implement any function that cannot be implemented by a Perceptron. The simplest example of such a function, and one that was cited in the book, is the logical **“Exclusive-OR”** or **“XOR”** function.

### Logical “XOR”

An **XOR** function cannot be implemented by a **linear system**. As a plausibility argument for this, consider the four coordinates (0,0), ...

## 8 Perceptrons: an Introduction to Computational Geometry  
**Marvin Minsky** and **Seymour Papert**, MIT Press 1969

The inputs (0,1), (1,0), and (1,1) represent the four possible inputs to an **XOR** function. To specify an XOR, we associate a **0** with both (0,0) and (1,1), and a **1** with both (0,1) and (1,0).  

It is **not possible** to place a single straight line in the plane of the input coordinates such that both 1s are on one side and both 0s are on the other. This suggests the **XOR function is not linearly partitionable** — a result that can be rigorously proven. See **Figure VI-3**.  

However, placing a **curved line** that partitions 0s and 1s is trivial if the linear constraint is relaxed. Any system that can implement curved lines can partition the outputs correctly.

---

### Figure VI-3: Logical XOR Function

- Black points represent output **0**  
- White points represent output **1**  
- Left: zeroes cannot be separated from ones by any straight line  
- Right: zeroes are separated from ones by a curved line  

---

We can show a **CO machine** (Correlithm Object machine) does not have the limitations of a **Perceptron** by coding the logical XOR function:

```
s1.Zero s2.Zero >: s3.Zero
s1.Zero s2.One >: s3.One
s1.One  s2.Zero >: s3.One
s1.One  s2.One  >: s3.Zero
```

---

## Turing Completeness

A **Von Neumann machine** is **Turing complete**, meaning it can compute anything that is computable. We argue that a **CO machine** is also **Turing complete**.  

This means anything achievable with one architecture can be done with the other. They are complementary in many areas despite their differences.

---

### Proof Outline for Turing Completeness

Proving **Turing completeness** rigorously involves showing the machine can implement a **Universal Turing Machine**. Our argument is simpler but correct.  

Today's digital computers are **Turing complete**. They are built from two devices:  
- **Passive Boolean logic**  
- One-bit memory devices called **flip-flops**

A CO machine can implement any Boolean logic function. To prove Turing completeness, it suffices to show that a CO machine can implement a **flip-flop**.

---

### Flip-Flop

- Memory advantage over simple Boolean logic  
- Can store a bit of data over multiple clock cycles  

Rules for CO flip-flop:

```
# CO flip-flop:
# state latch
s2.p1 >: s2.p1    # rule 1: remember s2.p1
s2.p0 >: s2.p0    # rule 2: remember s2.p0
# control (s1 must have more dimensions than s2)
s1.p1 >: s2.p1    # rule 3: force s2 state to s2.p1
s1.p0 >: s2.p0    # rule 4: force s2 state to s2.p0
```

---

### Figure VI-4: Conceptual Drawing of a Flip-Flop

- The **state point** in s2 represents the flip-flop state.  
- When the state point in **s1** is far from p0 or p1, latching rules (r1 and r2) in s2 cause rapid movement to the nearest of p0 or p1.  
- Since **s1** has more dimensions than s2, rules r3 or r4 can override the current state to flip the flip-flop state.  

---

### Memory Behavior

- Rules 1 and 2 implement a **two-state latch** in s2.  
- If s2.p(t) = s2.p1, rule 1 dominates and maintains this state; if s2.p(t) = s2.p0, rule 2 dominates.  
- The state point remains in these **attractor wells** until influenced by rules 3 or 4.  

---

### Influence of Control State (s1)

- If s2 is latched at s2.p0 and s1 is near s1.p1, rule 3 becomes strong and dominates rule 2, switching s2 to s2.p1.  
- If s2 and s1 have roughly equal dimensions, state may be unstable or chaotic.  
- Designing s1 with **significantly more dimensions** than s2 allows rules 3 or 4 to overpower latching rules.

---

### Dimensional Analysis

- Example: s2 has 100 dimensions, s1 has 200 dimensions.  
- Each dimension provides about **4 bits** of information (see Table III-1 in Chapter III).  
- Maximum weight for s2 is approx **2^(4*100) = 2^400**, for s1 it is **2^(4*200) = 2^800**.  
- Typical standard distances:  
  - s2: ~4.08  
  - s1: ~5.77  
- Rule 3 overwhelms rule 2 if s1’s match distance is less than 1.69.  

---

### Flip-Flop Functionality Summary

- Rule 3 dominance flips the state point in s2 from s2.p0 to s2.p1.  
- Rule 4 dominance flips it back to s2.p0.  
- The state remains until the opposite rule dominates again.  

---

### Conclusion on Turing Completeness

- The construction shows how to build a **one-bit CO flip-flop**.  
- By duplicating these rules and spaces, multiple flip-flops can be created.  
- Coupled with arbitrary Boolean logic implementation, CO machines can implement any current-day computer.  
- Since current computers are **Turing complete**, CO machines are also **Turing complete**.  

---

# Chapter VII Strings  

We have focused on individual points in a unit N-cube. In **geometry**, lines, shapes, and volumes are composed of points.  

Here, we explore **string correlithm objects (string COs)**, which are sets of points in a unit N-cube defined by geometrical shapes.

---

### Why String COs?

- Many patterns have a **geometric relationship** among features.  
- For example, a **face** is recognized by “two eyes, a nose, and a mouth” in correct positions.  
- Geometrical or sequence relationships (like order of words) are important.  
- String COs represent these relationships within a CO system.

---

### Extensions Beyond Points

- Up till now, focus was on **individual points** in high-dimensional bounded spaces.  
- Now, we include sets of points such as **lines, surfaces, and volumes**.  
- We call these sets **string COs**, and the topic is collectively “strings.”  
- The term “string” is just a label without usual literal meaning.  

---

### Complexity and Scope

- The subject of strings is extensive and deserves a full book.  
- This chapter offers a gentle introduction only.  

---

### Geometry and Embedding

- String COs allow embedding lower-order geometric objects (e.g., squares, spheres) within a higher-dimensional CO space.  
- Distance measured within the embedded object corresponds proportionally to distance in the CO space.  

Example:  
- Embedding a **checkers board** into CO space preserves relative distances between squares approximately.

---

### Mapping Uses

- Map an entire string CO to a **single point CO**: point CO serves as a **soft token** for pattern recognition.  
- Map a string CO to another string CO: perform mathematical **transformations or “morphing”** of shapes.  

---

### Applications of String COs

- Represent geometrical objects within CO space.  
- String COs have **capture zones** like point COs, capturing approximate shapes.  
- Any similar shape matches the string CO’s capture zone.  
- Can represent any set of features with geometrical relationship (e.g., color, texture, local shape).  
- Enables powerful **data fusion** capability—combining multiple feature types in relative positions.

---

### String Concepts

The fundamental idea is to define **mappings** between a geometrical object **G** and a single point **P**, where:  

- G: any set of points (line segments, surfaces, volumes, or arbitrary point collections), typically in low dimensions.  
- P: a CO point, often randomly chosen in a high-dimensional bounded space (e.g., unit N-cube).  

Mappings can be:  
- From G to P  
- From P to G  
- Bi-directional  

The mappings must ensure:  
- **Small deformations** in G map to **small distances** from P.  
- Degree of change in G is reflected as distance in P.

## String Correlithm Object Concept

The **deformations** may include **translations**, **rotations**, **magnifications**, **distortions**, or other small changes, or combinations of these. The deformations may also include **noise** in many forms. See Figure VII-1.

![Figure VII-1](image_path)  
*Figure VII-1 A string correlithm object maps a geometrical object to a random CO.*

Under the **string concept**, we are not simply trying to capture **membership in G**. We can already do that with **COs**. We do not need strings to accomplish membership. If **G** is a finite set of points, we can write an equal number of rules to map every one of those points to the single point **P**. If the state point in **G’s space** moves to a point of **G**, or even if it moves to a point very near to a point of **G**, then the rule associated with that point will become strong and the state point in **P’s space** will become **P** or very near to **P**.

With strings, we are trying to capture the **geometrical characteristics** of **G**. This is reminiscent of **Aristotle’s “formal cause”** in philosophy. Aristotle gives the example of the **“chair-ness”** of a chair. It is hard to specify exactly why a chair “looks like” a chair, but we humans **“know a chair when we see one.”** Similarly, in the string concept we are interested in **“G-ness.”** We want **G**, and anything that **“looks like G,”** to be associated with the single point **P**, or a point very near to **P**.

## Generating Geometrical Patterns with Strings

Let’s see what is involved in generating the geometrical pattern of a **face**, referred to as **G**, by capturing its geometrical characteristics in a point, **P**, and its related strings. Ultimately, the mapping between **G** and **P** will turn out to be a **hierarchy of mappings** between the more subtle and detailed aspects of the geometric form, or **“lesser Gs,”** and a string of **Ps** to represent those detailed, subtle characteristics, or **“lesser Ps.”**

More detailed levels of pattern generation will require more **P strings**.

## Example: The Face as a Geometric Pattern

A particular geometrical relationship between two eyes, a nose, and a mouth is a **“face”** to most people. (See Figure VII-2.) Let’s call the facial features **“elements.”**

- Small variations in the relative locations of the eyes, nose, and mouth can be made without destroying its **“face-ness.”**
- Larger variations in the positions of these elements will cause most people to stop calling the ensemble a **“face.”**

## Chapter VII Strings

### Figure VII-2: Recognition of Faces by Relative Location of Elements

Even though the **image elements** remain the same, if they move much from their “expected” relative locations, most people will fail to see a **face**. Most people describe a cartoon face as “two eyes about here and here, a nose about here, and a mouth about here.” 

Suppose we can recognize **eyes**, **noses**, and **mouths** anywhere in an image. We still need a way to capture their approximate relative locations — the idea of “**here**.” This is done with a string **CO**.

- The relative locations of the elements are assembled into a set of **point COs** that, together with their relative geometric coordinates, form an appropriate **string CO, G**.
- The string CO, G, is then mapped to a **single point CO, P**, that represents the ensemble.
- The point P is a single token at the top of an organizational hierarchy representing a face.

### String COs Represent Geometrical Relationships

- **String COs** represent geometrical relationships that exist among **point COs**.
- They capture geometrical relationships among any kind of elements, including **real-world objects**, point COs, and other string COs.

### Hierarchical Organization of a Face (See Figure VII-3)

- An **eye** can be defined as a particular geometric relationship among elements like eyelashes and pupils.
- The approximate relative locations of these elements define the eye.
- Moving up, a **human form** includes “a face here, two arms here and here,” etc.
- At the top of the hierarchy is a single point representing the entire arrangement named by a single token.
- At the bottom are countless tiny elements at the limits of perception.
- The unifying whole is the approximate relative **locations** of all these elements.

### Robustness of String COs

- String COs allow variation in position; some elements can be out of position or missing altogether.
- The ensemble still generates a **state point, P'**, near the pattern point P that represents the face.
- The problem of assessing geometrical similarity is transformed into measuring the distance between two points: the pattern point P and the state point P', representing the current arrangement.

### Pattern Recognition and Pattern Generation

- The hierarchy can be traversed upward (pattern recognition) and downward (pattern generation).
- **CO systems** can “see and do things like previously seen and done.”
- Pattern recognition fits patterns into known classes.
- Pattern generation produces an output pattern adapted to current conditions.
- String CO systems easily achieve adaptive pattern generation, which is hard by conventional means (e.g., robots walking on unfamiliar terrain).

### Figure VII-3: Hierarchical Image Composition

- Images are composed of a hierarchy of elements with particular geometric relationships.
- At each hierarchical level, string COs capture relationships.
- Both geometry and hierarchy are represented.

---

## Approximate Relative Location

### Understanding Relative Location

- An element can be relative to another element or a particular **reference point** or **origin**.
- Both forms are equivalent and convertible by simple geometrical mathematics.
- Choosing an origin relates all elements to a single point, simplifying the problem.

### Evidence of Origins in Living Systems

- The **human eye** moves to center the image it sees on "something interesting."
- The sensitive central point in the visual field is called the **foveal spot** or **fovea**.
- Humans do not recognize shapes, such as letters, well if they are a few degrees off the fovea (see Figure VII-4).
- Elements of a letter hierarchy relate by relative location to the origin at the center of the fovea.
- Many other fovea-like phenomena exist in human perception and form a widespread tool in our perceptual framework.

### Mapping Relative Location to an Origin

- It suffices to deal with relative location with respect to a previously selected origin.
- We assume the origin placement is done (e.g., center of fovea in the eye).
- A **coordinate system** centered on the origin defines locations relative to it.

### Problem Statement: Mapping Approximate Relative Location

- Given an origin, a coordinate system related to that origin, and some point **g** in the coordinate system:
  - Associate CO **p** with g,
  - Such that points near g associate continuously with points near p,
  - Points closer to g produce points closer to p.

### Example: Mapping Pattern Points g in s1 to p in s2 (See Figure VII-5)

- Consider space s1 with pattern point g on a **unit line**.
- Origin is at left end of the line.
- Associate pattern point p in a high-dimensional space s2.
- As state point **g'** in s1 moves along the line through g:
  - While g' is far from g, **p'** is at standard distance from p.
  - As g' closely approaches g, the distance between p' and p goes to zero.

---

### Using Sampling to Produce This Behavior

- Set up a series of **K rules** that sample this functional relationship:
  
  ```
  s1.h[i] >: s2.q[i], i=1..K
  s1.h[i=n] = s1.g
  s2.g[i=n] = s2.p
  ```

- The rule for i=n maps g to p.
- The exact rule is not strictly necessary; sampling produces rules close enough for adequate mapping.

### Behavior of the Sampling Rules

- Suppose points in both s1 and s2 are random COs.
- Set the state point g' in s1 to s1.h[i=n] = s1.g.
- Rules produce state point p' in s2 which is about standard distance from each s2.q[i] except at i=n where distance is zero.
- The desired functional relationship is nearly achieved, but the “notch” in distance near g could be wider.

---

### Constructing a String CO in s1

- Move points s1.h[i] closer together than standard distance.
- Pick s1.h[1] at random.
- Pick s1.h[2] closer to s1.h[1] than standard distance.
- Complexity in how to do this is left to subsequent work.
- Similarly move points s2.p[i] closer together.

---

### Effect of Moving Points Closer

- Distance between s1.h[i] and s1.h[i+1] is fraction **f** of standard distance in s1, where 0 < f ≤ 1.
- Set g' = s1.g.
- p' has zero distance to s2.p (the strongest rule s1.g >: s2.p).
- Distances to two points immediately on either side of s2.p are less than standard distance.
- The two rules controlling these points are strong because their s1 pattern points are closer than standard distance to s1.g:
  
  - s1.h[i=n-1] >: s2.q[i=n-1]
  - s1.h[i=n+1] >: s2.q[i=n+1]

- Distances further away increase asymptotically to standard distance.
- Smaller f produces a **wider notch** in the distance plot (see Figure VII-6).

---

### Summary of Notch Behavior

- The notch follows state point **g’** as it moves along s1 sample point path.
- If g’ is not on this path, then the bottom of the notch will...

## 9 A Few Comments About This Process

The problem of building **string COs** is subtler than it may appear. Many things one might initially try have unexpected and undesirable side effects. For example, selecting two random COs, **a** and **b** in space **s1**, then performing a simple linear interpolation of their coordinates to produce intermediate COs produces roughly the functional form we seek.

However, intermediate COs produced this way are closer than **standard distance** to any third CO, **c**, selected at random. This interpolation “takes a shortcut” through space **s1** that produces this behavior, which is almost certainly not desired.

---

## Chapter VII Strings

### Distance Behavior and the Notch Phenomenon

As **g’** moves to the value of a random CO in **s1**, the **notch disappears entirely** (see Figure VII-7).

**Figure VII-6** shows distance as a fraction, **f**, of standard distance in **s2** as rules get further away from strongest. Decreasing **f** makes the “notch” wider.

**Figure VII-7** illustrates how the depth of the notch shrinks as the state point moves away from the string CO. When the state point in **s1** is exactly on the string CO, it matches one of its rules and produces a state point in **s2** that is at **zero distance** from the pattern point of that rule. When the state point in **s1** is at **standard distance** from the string CO, all rules produce points in **s2** that are standard distance from the state point in **s2**.

---

### Ordinality

One use for such a one-dimensional **string CO** is **ordinality**. Suppose we build a one-dimensional string CO of twenty-six points and adjust their spacing so that the first and twenty-sixth are closer together than standard distance. Thus, the distance between any two points is less than standard distance, and distance increases uniformly both directions away from any particular string CO point.

- Label the first point with **"A"**, the second with **"B"**, up to the twenty-sixth with **"Z."**
- Set the **state point** in this space to the point corresponding to **"C."**
- The distance to each point then reflects our intuition about ordinal distance: **"B"** is closer to **"C"** than **"A,"** and **"D"** is closer to **"C"** than **"E."**

Similarly, define a string CO for the **Cyrillic alphabet** in the same space.

- All points in this Cyrillic string CO will be at **standard distance** from all points of the English string CO.
- Setting the state point to the Cyrillic character **"Җ"** (the “zh” sound) will have distances to other Cyrillic characters reflect ordinality.
- Distances to points in the English string CO will all be about **standard distance**.

Set up rules that:

- Map all English string CO points to a single random CO labeled **"English Letters"** in a new space.
- Map all Cyrillic string CO points to another single random CO labeled **"Cyrillic Letters"** in the new space.

If the state point gets near either string CO, the corresponding point in the new space will approach the corresponding CO, indicating whether we are seeing **English** or **Cyrillic characters**.

Add two new rules that:

- Map both random COs to a single random CO in a **third space**, approached whenever either an English or Cyrillic string CO point is approached in the first space.

This process is illustrated in **Figure VII-9**.

> 10 Remember that all members of a given string CO are at **standard distance** from every random CO in the space, except each other. This happens automatically due to the geometric probabilities involved. All COs are at about standard distance from one another unless we intervene.

---

### Figure VII-8 Description

Two string COs defined in the same space:

- One for the **English alphabet**.
- One for the **Cyrillic alphabet**.

Every point on each string CO is about **standard distance** from every point on the other string CO. Within each string CO, alphabetically adjacent points are very close together. Points more distant alphabetically are also more distant spatially.

---

## Higher Dimensions

Approximate relative location is useful beyond one dimension. For example:

- A face composed of eyes, nose, and mouth requires approximate relative location in **two dimensions** (drawing or photograph) or **three dimensions** (solid figure).
- Even higher dimensionalities are often needed.
- Higher dimensional string COs are straightforward extensions of the concepts presented.

---

### Figure VII-9 Description

- Every point in the string CO representing the **English alphabet** maps to the single CO labeled **"English."**
- This represents membership of the English letters in the alphabet.
- Similarly, Cyrillic letters map to the CO labeled **"Cyrillic."**
- Both target points are then mapped to a single CO labeled **"Alphabet,"** representing the knowledge that any letter of English or Cyrillic is alphabetic.
- Note: **"A"** is the first letter in both alphabets.
- Given **"A"**, rules cause both Cyrillic and English COs to be at equal strength.
- It requires additional information to distinguish which alphabet a particular letter belongs to.

---

### Coding Approximate Relative Location in Two Dimensions

Extend the previous one-dimensional problem formulation to two dimensions:

- Problem statement:  
  “Given an **origin**, a **coordinate system** related to that origin, and some point, **g**, in the coordinate system, associate CO **p** with **g** such that points near **g** are associated continuously with points near **p**, with points closer to **g** producing points closer to **p**.”
- This statement works for one dimension, two dimensions, or more.

For two dimensions:

- Consider a **square** with the origin at the lower left corner, edges of unit length (space **s1**).
- Place a pattern point **g** somewhere within the square.
- Associate a pattern point **p** in a suitable high-dimensional space (**s2**).
- Desired behavior is shown in **Figure VII-10**.

---

### Figure VII-10 Description

- Shows desired behavior for **2D approximate relative position mapping**.
- Point **g** is selected in the unit square of space **s1**.
- When the state point for **s1** is placed at **g**, distances to points in **s2** form a **“cone”** around **g**.
- The cone asymptotically approaches **standard distance**.
- Approximate contour lines of the statistical surface are shown in the square.
- The **z-axis origin** is well above the plane of the unit square to aid visualization.

## 94 Correlithm Object Technology

There are two coordinates for **g**, the **x coordinate** and the **y coordinate**. One possible strategy for defining rules is to set up a **one-dimensional string CO** for each coordinate, then concatenate them to form one point in **s2**.

**s1.(x,y) >: s2.gx[x] s2.gy[y], x=1..X, y=1..Y**

One of the things we want for the string COs in both spaces is for the "**cone**" of distance around a given point to be **symmetrical**. This means that if a given point **g** at (x,y) in **s1** maps to **p** in **s2**, then all points at the same radial distance, **r**, from **g** in **s1** will map to points in **s2** that are all about the same distance from **p**.

**Figure VII-11** These distance “troughs” arise when trying to build a 2D string CO by concatenating two 1D string COs that correspond respectively to the **x** and **y coordinates** of a point in the unit square.

This simple strategy **fails**. (See **Figure VII-11**.) Since directions parallel to the **x** and **y axes** in the unit square produce no changes in the values of the corresponding 1D string COs, distance in **s2** is not increased by that string CO. Only the other string CO contributes to distance in **s2** in this circumstance, and the result is decidedly **not symmetric**.

---

## Chapter VII Strings (continued)

**Figure VII-12** shows another approach that fails. A point in **s1** is coded into **s2** by building a **coarse grid** of random COs, then interpolating among the four that bound the point.

The "**sags**" are caused by the interpolation process, which “takes a shortcut” through the high dimensional space of **s2**.

Another simple strategy also fails. (See **Figure VII-12**.) We might try laying out a **coarse grid of random COs** across the unit square, then doing a **linear combination** of the random COs at the four corners that bound the point of interest, **g** at (x,y).

This indeed works reasonably well for points within those four corners, and the grid of random points itself does indeed produce **standard distances**, but there are **“sags”** in the surface. These are caused by the same problem that plagued interpolation in the 1D case, namely the fact that such interpolations produce points that “take a shortcut” through **s2**. The result is that interpolated points are not at about **standard distance**.

---

Rest assured that there is at least **one good solution** to this problem. It would take more space than is reasonable here to investigate it in detail, but the essence of the solution is to **distribute the kinds of problems and anomalies we have just seen randomly across the space to be coded**.

The result is **computationally efficient**, even though the algorithm is not simple to describe. But once again, **randomness “comes to the rescue.”**

---

### Comments

As promised, this has been a "**gentle**" introduction to a very complex subject. We have left out many things. For example, the simple **line segment** and **planar patch string COs** presented here can be "**looped**."

- The ends of the line segment can be joined to form a **continuous space**, a **ring**.
- Similarly, one or both of the edges of the planar patch can be joined to its opposite.
  - If this is done in one dimension only, a **tube** results.
  - If it is done in both dimensions, the result is a **torus**.

Any continuous object in any number of dimensions can be dealt with, so string CO coding is **very general**.

The ability to represent and relate **geometrical objects** in ways that map **deformations** of a given object to distances from the point CO that represents that given object provides a **powerful tool** for representing and generating **patterns**.

It gives us the ability to construct **hierarchies of patterns** by giving specific examples, and the security of knowing that anything “**like**” the examples we give will also be **robustly recognized**.

Such a hierarchy can be traversed:

- Toward its **root** to obtain **robust pattern classifications**
- Toward its **leaves** to produce **robust behavioral patterns** that accommodate variations in encountered situations.

Finally, it should now be obvious that **string COs**, and therefore **CO machines** in general, have a natural, innate ability to represent **continuous, analog processes**.

A **CO machine** is very different from a traditional computer, with different strengths and weaknesses, as we will see shortly.

---

## Chapter VIII Architecture

CO systems run best on **special multiple-machine computing resources**. When we make popcorn, we do not pop the kernels one at a time because it would take too long.

The tasks inside a **CO system** are like **kernels of corn**. We can “pop” them **all at once**. A standard digital computer runs **only one task at a time**, and it cannot finish the tasks of a CO system very quickly.

However, a CO system runs fast on any computing resource that runs **many tasks at once**, such as:

- A “grid” of internet computer sites
- A set of **digital signal processors (DSPs)**
- Even better designs on the drawing board

---

The combination of a **CO system** and one of these multiple-machine computing resources has many **unique properties**:

- The CO system itself keeps working correctly even if **large numbers of the multiple processors fail**.
- There is no need to **reconfigure the computing resource** or even to detect the failures.
- We can run **multiple CO systems at the same time** on the same set of machines without interference.
- This is not “timesharing” or “distributed processing” as commonly known, but something more like a **hologram**.
- We can establish a kind of “**group mind**” found in the determination and purpose of a **swarm of bees**.

---

**Correlithm Object (CO) systems** feature computing resources that are radically different from those of mainstream digital computers.

The resources are:

- Inherently **concurrent**
- Can benefit greatly from **concurrent implementations**
- Have unique properties of **robustness**, **sampling**, **superposition**, and even **holographic-like characteristics**

---

The fundamental architectural unit of a CO system is the **cell**, which carries a **time-varying state** that is an independent function defined on the states of some set of cells.

- Potentially, **no two cells have identically the same functionality**.
- System behavior emerges from **statistical properties** involving:
  - Individual cell functionality
  - Organization of the cells
  - Flow of their intercommunications

In a strong sense, each cell has a unique **“point of view.”** A cell:

- Observes the states of other cells
- Compares those states to **reference states** it has previously stored
- Generates its output as a **one-dimensional, time-varying “opinion”** about the current situation

From time to time, a cell may add to its pool of stored reference states under various circumstances.

---

To approach an understanding of the nature of CO systems, one should visualize a **living brain**.

- No single cell is ultimately important.
- Although macroscopic functionality can be assigned to various regions of cells in a brain, a detailed inspection looks like **noise and chaos**.
  
It is intuitively clear that each brain cell contributes something to the whole, but it is unclear what a particular cell actually does from an information processing perspective.

**CO systems are much the same**, derived from a theory of information processing in **living neural systems**, which remain a guiding light in the evolution of CO technology.

---

The **autonomy** of individual cells and the **emergent behavior** of cell ensembles present unique architectural capabilities and opportunities that exist essentially independent of what processes the CO system is actually performing.

Many of these properties are worthy of study and exploitation in their own right, not merely as a means to enhance system performance.

They offer **new resources** and **perspectives** useful in a wide range of applications, frequently providing **robust and satisfying solutions to long-standing, difficult problems**.

In addition, they offer new directions of investigation that promise even more capability.

---

## Chapter VIII Architecture (continued)

### Serial Implementations

Given the widespread availability of **digital computers**, it was inevitable that CO systems would appear first as programs for these machines.

- Current computers can be viewed as **serial systems**, although with some limited concurrency.
- Predominant behavior is the execution of a **single programmatic thread**.

A current digital computer is perhaps the **worst possible system** to host a CO process.

CO behaviors emerge from the statistics of ensembles. Producing these behaviors necessarily involves dealing with:

- Large numbers of **cells**
- **Rules**
- Communications channels

A **serial or single-thread computer** must process this workload **one element at a time**.

Worse, the elements are all essentially **independent at several logical levels** and fully capable of correct behavior with little synchronization, a situation that calls for:

- High levels of **concurrent processing**

A CO system will potentially run **many orders of magnitude faster** on appropriate concurrent hardware than on a serial machine.

See **Figure VIII-1**.

## Figure VIII-1: CO Systems and Concurrent Computing Resources

**CO systems** run faster on **concurrent computing resources**, such as the one represented on the right, because CO systems are inherently **concurrent**. 

Still, current computers are ubiquitous, and a CO system will execute correctly on one. But many of the interesting properties discussed below will be weak or unavailable on current computers. Properly **concurrent host computers** for CO systems need to be developed to take full advantage of these capabilities.

Fortunately, there are a few hardware opportunities available even today that are more appropriate hosts for CO systems, including:

- **DSP (Digital Signal Processing)** integrated circuits  
- **FPGA (Field Programmable Gate Array)** integrated circuits  
- Multi-CPU machines  
- Clusters and distributed computing  
- Grid computing  

In the future, we expect to see purpose-built CO integrated circuits and even the intriguing possibility of a purely **analog CO machine**.

---

## Concurrent Implementations

Within a CO system, there are at least **three levels of concurrency** available for exploitation. A CO system executes a set of **rules**. The possible concurrency levels include:

- All **rules** can be executed concurrently  
- All **rule source sides** can be executed concurrently  
- All **target cells** can be executed concurrently  

See **Figure VIII-2** for a visual representation.

### Figure VIII-2: Types of Concurrency in CO Systems

- **Concurrent Rules**  
  ```
  a[1].p >: b[1].q  
  a[2].p >: b[2].q  
  …  
  a[i].p >: b[i].q  
  ```

- **Concurrent Source Cells**  
  ```
  a.p >: b.q  
  a.state[1] vs. a.p[1]  
  a.state[2] vs. a.p[2]  
  …  
  a.state[j] vs. a.p[j]  
  ```

- **Concurrent Target Cells**  
  ```
  a.p >: b.q  
  a.state vs. a.p →  
  → b.q[1]  
  → b.q[2]  
  …  
  → b.q[k]  
  ```

CO systems have at least **three kinds of concurrency**:

- **Rules** can be concurrent because any rule can be executed independent of any other rule.  
- Within a rule, the comparison of the **pattern CO** with its **state CO** contains concurrency at the **cell level**.  
- **Target cells** can be set independently of each other.

### Fully Concurrent CO Systems

In the ultimately desirable CO system, all **rules** would operate **concurrently**, **asynchronously**, and **continuously**. There would be no CO machine “cycle,” no clock boundaries to trigger the execution of rule sets.

- A **cell** would emit a value that is a **continuous function of time**.  
- A rule’s source state point, comprised of the values of cells, would also be a continuous function of time.  
- A cell should continuously map the state it receives to its own **target value** according to the **source pattern points** it has stored.

---

### Figure VIII-3: Independence of Rule Operation

- Rules can be executed concurrently.  
- What a rule reads from a **source cell** is not affected by what any other rule reads from that cell.  
- The internal processing of a rule depends only on the values read from source cells.  
- Rules may try to write different values to the same target cells, but the formulas for computing new target cell values are deliberately **incremental**, guaranteeing that target cell values “nominated” for a cell by various rules are correctly handled.

From this, it should be clear that a rule’s operation is **completely independent** of the operation of any other rule.

- While the role that a rule plays in the overall functionality of a CO system is not independent of other rules, a rule’s **operation is**.  
- The sources and targets of rules may be interconnected in complex ways, but a given rule can always compute its target value solely from its **source state** and its **source pattern points**.

This means that *rules can be executed concurrently* without intrinsic rule execution order constraints.

### Rule Source Side Evaluation Concurrency

- The operation of a rule involves evaluating the **source**, distributing the resulting value to the **target cells**, and incorporating that value by the target cells into their target values.  
- **Source evaluation** involves comparing the **state point** for the aggregate source to the **aggregate pattern point**, developing a measure of proximity such as **distance**, **probability**, or **bits**.  
- The evaluation involves dealing with the coordinates of a **high-dimensional source point**. Typically:
  - The state value for each dimension is compared to the pattern value for the same dimension.  
  - When all comparisons are complete, an aggregation is performed to produce a single evaluation value.  

Clearly, the state-to-pattern comparisons in the dimensions can proceed **concurrently**.

---

### Figure VIII-4: Concurrent Source Pattern Comparison

- The squared difference is independent for each cell index, so all **j** of them can run concurrently in one step.  
- Finally, the square root of the sum of those results is taken.

---

### Source Evaluation Efficiency Considerations

- It is more efficient for the **source side** of a rule to be evaluated **once** rather than each target cell independently evaluating the same source.  
- However, for maximum **target cell autonomy**, each target cell can do its own source side processing if supplied with source state information.  
- This creates redundant work but eliminates the need for **synchronizing** their operation with a common source side evaluation.

See **Figure VIII-5** for details.

---

### Figure VIII-5: Source Evaluation Strategies

- **One Source Comparison for All Target Cells**  
  ```
  a.p >: b.q  
  a.state vs. a.p →  
  → b.q[1]  
  → b.q[2]  
  …  
  → b.q[k]  
  ```

- **Per Target Cell Source Comparison**  
  ```
  a.p >: b.q  
  a.state vs. a.p → b.q[1]  
  a.state vs. a.p → b.q[2]  
  …  
  a.state vs. a.p → b.q[k]  
  ```

The left shows the typical approach; the right shows an approach for maximum target cell concurrency.

---

### Target Cell Concurrency

- Target cells can be executed concurrently.  
- According to the **formal CO machine** definition, a target cell only needs to know its **pattern points** and the evaluation of its source to compute its next target value.  
- It does **not** need to know what other target cells of the rule are doing.  
- This is a design requirement to guarantee the ability to execute target cells concurrently.  
- Some CO machine definitions require target cells to coordinate their activities, but these are excluded to ensure concurrency.

---

### Figure VIII-6: CO Systems as Synthetic Organisms in Swarms

- CO systems often emulate **synthetic organisms** with information processing characteristics much like living creatures.  
- Often a **swarm** of such systems operate independently and **concurrently**, interacting among themselves through an **environment**.

---

### CO Systems Running on Concurrent Resources ("Straddlers")

- A **straddler** is a CO system running on a concurrent computing resource such as:
  - Multi-CPU computer  
  - Grid computer  
  - LAN  
  - Internet  
- The CO system “straddles” the resource by exploiting concurrency to reduce wall clock time.  
- Additional benefits arise beyond performance improvements.

Simple **hardware acceleration** can be an effective way to exploit concurrency:

- Software tools exist to translate C/C++ code into efficient executables for **DSPs (Digital Signal Processors)**.  
- Similar tools are emerging for **FPGAs (Field Programmable Gate Arrays)**.  
- These approaches yield significant performance gains compared to serial computers.

---

## Analog Implementations

**CO systems** have the ability to implement **continuous functions directly** as **analog systems**.

- Current digital computers operate circuitry to avoid its **active regions**.  
- Each digital component has a continuous "active" operating region bounded by two extreme states:  
  - **Full saturation** (conducting maximally)  
  - **Full cutoff** (conducting essentially nothing)  
- Digital computers avoid the active region, focusing on discrete states because digital computation theory handles discrete values, not continuous ones.

Analog computers:

- Employ the **active region** of components.  
- Historically, analog computers primarily solved **differential equations**, which limits widespread knowledge among computer scientists today.  
- Some **control systems** are still built using analog hardware.  
- Increasingly supplanted by digital systems that convert analog signals to digital, process digitally at high speed, and convert results back to analog form.

## Figure VIII-7 Analog Computers and Speed

**Analog computers** can be much faster than **digital computers**. A **cup of coffee** “solves” complex equations of **fluid dynamics** typically much faster than digital supercomputer simulations.

No **general-purpose analog computer** has ever gained prominence. In fact, no such system even comes to mind, yet such a system might be very desirable. **Analog processes** are often inherently fast, sometimes a lot faster than their digital counterparts.

Consider a cup of hot coffee. Suppose we stir it and then let a drop of cream fall into it. The cream will disperse rapidly through the hot coffee. Yet the discrete approximation that must be set up and solved to simulate this dispersal on a digital computer is daunting, and the computational time to produce a reasonable approximation to the solution will be long, even on our fastest digital hardware.

Clearly there is a lot of computational power in a simple cup of coffee. But how do we harness this power? How do we build a **general-purpose analog computer**?

## CO Technology and Analog Computation

**CO technology** provides a means. CO machines are inherently **analog in nature**. This seems reasonable since CO machines are modeled after **living neural systems** that are definitely analog.

The functionality of a rule involves only simple mathematical operations that can all be readily implemented in purely analog hardware. The basic computation is a **distance calculation** between a **state point** and a **pattern point**.

- The pattern point can be represented by preset analog quantities such as **voltage** or **current**.
- The state point varies over time but is similarly represented.
- Computing distance involves forming differences in each dimension, done with a **differential amplifier**.
- Then square each difference, sum the squares, and take a square root — all done with the same kind of hardware.
- Remaining steps are equally simple mathematical operations.
- Due to the strong **noise tolerance** of CO systems, the typical two or three-decimal-digit accuracy of such hardware is sufficient.

There appears to be no significant impediment to a purely analog implementation of rules.

We have already shown that **CO systems are Turing complete**, meaning that a CO system can implement any process that can be computed. So a **general-purpose computer** can apparently be built in purely analog hardware by utilizing CO technology.

## Direct Analog Function Implementation with CO Systems

CO systems can implement **analog functions directly**. Current digital computers implement analog functions by means of algorithms that manipulate numbers in digital form. But CO systems do their work by directly **mapping points between spaces**, a process that is essential to continuous functionality.

In the previous chapter on strings, we showed how to approach the mapping of **lines, curves, surfaces**, and so forth to themselves and similar objects.

It would be easy, for example, to construct **string COs** to implement a function such as **y = x²** through mapping, simply by building two lines in separate spaces and mapping points (x values) placed with linear separations on the first line to points (y values) placed with quadratic separations on the second line (see Figure VIII-8).

In this manner, arbitrarily complex functional relationships can be implemented directly by what is essentially “**table look-up and interpolation**.”

---

## Figure VIII-8 Quadratic Function Built With String COs

A quadratic function can be built by establishing two string COs and writing rules for the mapping. Here a string CO in space **s1** is mapped to another string CO in space **s2**.

---

## Increasing Digital Computer Speed: Limitations and CO Technology Potential

Currently, only two general techniques are available to computer science to make **digital computers run faster**:

1. Build the same thing out of faster parts.
2. Do more than one thing at a time.

This is the entire repertoire of the digital computer hardware architect. Every architectural enhancement of digital computer technology applies one or both principles.

CO technology may allow us to add a third principle:

- The direct use of the inherent speed of **analog processes**.

---

## Properties and Capabilities of CO Systems

Here are some of the many new and unique things that one can do with a CO system. The focus here is on properties and capabilities of the **CO machine architecture**, not on the CO processing itself (which will be discussed in Chapter X).

### Sampling

A **sampled rule** will operate correctly. This means if a rule operates correctly, another rule whose spaces are **subspaces** of the first rule—a sampled rule—will also operate correctly, provided the subspaces are not too small.

---

## Figure VIII-9 Sampling Concept

Two points are shown in their respective three-dimensional spaces, and their shadows are shown in corresponding two-dimensional spaces.

- The left point is mapped in 3-space to the right point.
- The shadow of the left point is correspondingly mapped in 2-space to the shadow of the right point.

We can think of the shadow mapping as a "**sampling**" of the point mapping.

---

A CO derives its primary properties from **statistics defined on its many dimensions**. Properties found in high-dimensional COs begin to emerge in COs that have only about 20 dimensions, and those properties rapidly become **statistically robust** as dimensionality grows.

To illustrate the correct operation of a sampled rule, consider the following two rules:

- **s1.p1 >: s3.p3**
- **s2.p2 >: s3.p3**

Both of these rules relate a **source point** to the same **target point**.

- Let **s1** have 2N dimensions and **s2** have N dimensions.
- Suppose that space **s2** is a randomly drawn subset of space **s1**, and point **s2.p2** is a corresponding subset of point **s1.p1**.
- The source side of the second rule is, therefore, a sample of the source side of the first rule (see Figure VIII-9).

Either rule could be used to perform the same mapping function in most any CO system.

- The first rule is potentially stronger than the second.
- A near-perfect match of **s1.p1** carries about **2Nσ bits** of information.
- A near-perfect match of **s2.p2** carries about **Nσ bits** of information.

If **N** is “big enough”—at least 20 and preferably 50 or 100—either rule should work satisfactorily for most purposes.

The sampling principle also applies to **sampled target spaces**. If a space is sampled in either the source or the target, or both, the result is a **sampled rule**.

---

### Importance of Sampled Rules

Sampled rules matter because:

- CO systems will likely continue to operate correctly even with **partial hardware failures**.
- Suppose we run the first rule above, and some of the cells of the underlying hardware fail, leaving only the random subspace **s3**.
- This might happen if the cells of **s1** are allocated to 2N different physical computers, perhaps across the internet, and N of those machines become unavailable.
- The rule will continue to perform its function but with reduced **information content**.
- It will **not fail**, even in the presence of massive hardware failure.

Careful design of the CO system is still needed to obtain the full benefit of this **fail-soft** advantage. The sampling principle, an inherent characteristic of CO technology, makes **fail-soft** a viable possibility.

---

## Figure VIII-10 Superposition Concept in COs

The concept of **superposition** allows correlithm objects (COs) to coexist in the same space. 

- This is illustrated by having three geometrical objects co-existing in the same three-dimensional space: a cube, a cone, and a cylinder.
- Just as the three shapes can easily be discerned, so correlithm objects can continue to function correctly while in superposition.
- In both cases, the objects have reduced information content because the other objects in the space are “noise” with respect to any object.
- But enough information content remains to allow each object to retain its own identity.

---

### Superposition

**COs can be superposed**. A single **state point** can be built from multiple COs in one space so that the COs coexist in the state point and can be recovered and even used simultaneously.

This means multiple, independent rules can be defined in the same spaces and still run correctly.

As a simple (and flawed) example of superposed COs, imagine the images of a cube, a cone, and a cylinder all trying to occupy the same space (see Figure VIII-10). It is possible to see all three objects distinctly, even though they share the pixels of the image.

Consider a space, **s1**, of **N dimensions**.

- Suppose we define two random COs in **s1**, namely **s1.p1** and **s1.p2**.
- Their distance apart is about **standard distance**, or \( N^{1/2} \).
- Suppose we build a state point, **s1.p**, that is the simple average of **s1.p1** and **s1.p2**, by adding the values in each dimension and dividing by two.

It is easy to show:

- The distance from **s1.p** to either **s1.p1** or **s1.p2** is about \( \frac{1}{\sqrt{2}} * N^{1/2} \), or about 0.707 times standard distance for **N** dimensions.
- Expressed in bits, a near perfect match of either pattern point is worth about **Nσ bits**, where \( σ \approx \frac{7}{120} \), the standard deviation.
- The match of either pattern point with **s1.p** is worth about **\(\frac{1}{2} Nσ\) bits**.

In general, building a **superposed state point** by averaging **K** random COs will make each match of one of the pattern points with the state point worth about **1/K** of the bits it would be worth in the absence of superposition.

As usual, if **N** is “large enough,” the probability that a random state point will inadvertently match well with a given pattern point can be kept small enough to be irrelevant.

---

*Note: This approximation should be used with care.*

## 11 Superposition Property in CO Spaces

The **superposition property** is available in any **CO space**, not just the real unit **N-cubes** considered in this book. It is particularly interesting in **Hilbert spaces** (coordinate systems of complex numbers), where it has important implications for **quantum systems**.  

Cells in the **CO computing paradigm** can be of any numeric type, including **binary, integer, real, complex**, or more exotic forms. All such systems exhibit the superposition property. All properties of the CO computing paradigm exist regardless of the **numeric type** of the cells or coordinates.

---

### Implications of Rule Overlaps

Consider these two rules:

- `s1.p1 >: s2.p3`
- `s1.p2 >: s2.p4`

Both rules map points in the same **source space**, `s1`, to points in the same **target space**, `s2`. The analysis suggests each rule carries at most half the bits it would if it operated alone. Therefore, both rules can **share spaces and operate reliably** if the number of dimensions in each space is sufficiently large.

This has powerful implications:

- If **N** (dimensions) for each space is kept "large enough," spaces can be allocated for rules and mapped into hardware without concern for dimension sharing.
- Spaces are usually allocated randomly from a large pool of cells, and overlap is rare.
- Superposition allows overlap to be generally ignored.

In some cases, deliberate overlay of many rules on the same space is desirable—for example, **frequency division multiple access** in telecom, where each frequency corresponds to a dimension. The principle of superposed COs allows large numbers of channels and tokens.

---

### Statistical “Holograms”

The sampling and superposition principles derive from the **statistical nature** of CO systems and the independence of their **cells**. Each target cell in a rule "watches" a set of source cells and compares their state CO to known source pattern COs, similar to how a **photographic plate** supports a **hologram**.

- A **hologram** is a 3D image formed by **holography**.
- A **holograph** is the photographic plate that carries the interference pattern defining the hologram.

A holograph is made by **"freezing light"** (see Figure VIII-11). Each **grain** of the emulsion captures a unique point of view of the real object by capturing information from light. Grains vary from fully **transparent to opaque**, normalized as a **real value** between zero and one.

Each grain can reconstruct the captured information and send nearly identical light onward, hence "freezing light" increases the **resolution** of the 3D image.

---

#### Figure VIII-11: A holograph "freezes" light

Under the right conditions, light from the object is captured on film, freezing the wavefront. Later, the same wavefront is sent onward, enabling an image—the hologram—to be formed.

---

A CO system has **similar properties to a holograph**:

- Each **target cell** is a unique "point of view" about the match between **source state points** and **source pattern points**.
- When source points match, the rule causes the target state point to match the **target pattern point**.
- Increasing the number of **dimensions** increases the process’s **resolution**, **robustness**, and **noise immunity**.

---

### Virtual Digital Computer as a CO System

Suppose we build a set of rules implementing a **digital computer**. This can be done by building **logic gates** and **flip-flops** as described in Chapter VI, assembling them into a digital computer design.

We can visualize this as a plane of physical CO cells with the **virtual digital computer hovering over them like a hologram** (see Figure VIII-12).

---

#### Figure VIII-12: Virtual digital computer implemented by a straddler

The virtual digital computer can run programs exactly like a real one, with virtual memory and disks. If we copy the hardware exactly, we can even install standard software like **operating systems**, **accounting programs**, etc.

---

### Physical Dispersion of Virtual Components

In the virtual digital computer:

- Components like the **CPU**, **memory locations**, and **disk drives** are physically dispersed across all CO hardware cells.
- Small components (e.g., flip-flops or gates) are composed of many cells scattered randomly across the entire CO system.
- Larger virtual components are assembled from smaller ones across larger sets of cells.
- The entire virtual digital computer is dispersed across all cells, effectively "nowhere and everywhere" in the concurrent resource.

---

### Robustness to Cell Loss

The loss of a single cell has **essentially no effect** because:

- A cell participates in many rules, each depending on many other cells.
- Even significant random loss of cells would not cause failure unless too many rules fall below reliable levels.
- The system can be designed to minimize failure probabilities.

---

### Analogy to Brain Functionality

This suggests why mapping brain functionality is difficult:

- Large regions like **speech** or **vision centers** can be identified because they involve many cells.
- If CO-style rules are responsible for brain processing, brain function emerges statistically from **cell ensembles**.
- The **mind** is a virtual object dispersed across cell ensembles like the virtual digital computer.
- Cells are neither especially numerous in small regions nor physically close, and rarely perform exactly the same function.

---

#### Figure VIII-13: Robustness of a CO system across a swarm

A CO "mind" can run on cells spread across a swarm of vehicles, sustaining major damage without loss of functionality.

---

## Straddlers

**Straddlers** are CO systems running on **concurrent resources**. They are valuable wherever computing resources must not fail.

- Hosting CO systems on concurrent resources allows high robustness.
- Virtual implementations of hardware/software are possible.
- The main benefits of CO system operations will be discussed in Chapter X.

---

### Swarms

A **swarm** consists of many tiny vehicles (land, sea, air), each hosting some CO cells:

- Each vehicle has internal rules controlling it.
- Vehicles communicate low-bandwidth broadcasts of cell values to create rules across vehicles.
- This forms a **swarm mind** orchestrating the group.
- High-level rules can be superposed maximally across all cells and vehicles.
- The swarm mind is invulnerable to loss of individual vehicles, as no single point is vulnerable.
- No reconfiguration needed after vehicle losses; loss causes only resolution degradation, not loss of function.
- Remaining vehicles continue high-level behavior.

---

## Chapter IX Converters

Moving data into and out of **correlithm object space** requires **converters**.

- **Soft tokens** or points in high-dimensional spaces represent all data within a CO system.
- Data outside a CO system (**world data**) can be numbers, words, images, or any form.
- A **converter** translates between world data and soft tokens.
  - Called a **sensor** if translating from world data to soft tokens.
  - Called an **actor** if translating from soft tokens to world data.

---

Although there are many possible converters, most derive behavior from a few categories. This allows creation of **generic converters** adaptable to any world data, simplifying translation.

Rules discussed so far deal with mapping points to points (COs to COs) in high-dimensional spaces. Building practical CO systems requires moving data **into** and **out of** CO space, which is done by converters:

- Moving data **into** CO space: **sensor**
- Moving data **out of** CO space: **actor**

(See Figure IX-1.)

---

There would appear to be a need for endless converter types for all data. However, a few simple converter types handle a remarkably large number of requirements. Three broad categories of converters, **cardinal**, will be defined next.

## 12 Actors and Actuators

Some might prefer the term **actuator** to **actor**, but **actuator** normally implies that **mechanical motion** results. We have chosen the term **actor**, because it carries the more general connotation of **doer**.

## IX Converters

Converters come in various types: **cardinal converters**, **continuous converters**, and **ordinal converters**. These are useful in various **dimensionalities** and appear in two forms: **open** and **ring**.

In most cases, converters can be used as either **sensors** or **actors**, depending on the desired **direction of conversion**.

### Figure IX-1

All data representation within a **CO (Correlithm Object)** system is in the form of **COs**. **Sensors** convert world data into COs, and **actors** convert COs into world data.

---

## Cardinal Converters

A **cardinal set** is a set of objects with no relationship other than **set membership**. In particular, there is no **order** associated with a cardinal set.

An important example is a set of **random COs**, all defined in the same space. These COs are essentially the same distance apart, which makes them useful in **CO theory**.

A **cardinal converter** is a set of rules that maps a set of **cardinal objects** one-to-one onto a set of same-space COs. (See Figure IX-2.)

### Figure IX-2

A **cardinal converter** maps a set of **unordered objects** from the world outside of the CO system to a set of random COs within it.

For example, consider a cardinal converter for the logical values **True** and **False**. The sensor part has this functionality:

- **True >: s.T**
- **False >: s.F**

These rules state that if the data **"True"** arrives from the world outside, then a random CO, **s.T**, will be generated in the target cells of space **s**. Similarly for **False** and **s.F**.

The exact mechanism of how **True** and **False** arrive is irrelevant here. The presence of either as input is considered an exact match. The number of bits this match is worth is a detail that does not matter here.

Similarly, the actor definition is:

- **s.T >: True**
- **s.F >: False**

If the state CO in space **s** matches **s.T**, then **"True"** will be sent to the world; if it matches **s.F**, then **"False"** will be sent. Matching a CO is usually approximate — both **True** and **False** are always sent, but with strengths of the matches.

This cardinal converter maps external data elements onto the **same space s**, which is standard practice.

---

## Continuous Converters

A **continuous converter** maps a **continuous variable** to another continuous variable.

The name is somewhat misleading because there are currently no truly **continuous converters** available. What we use are rules that **sample continuous functions** and use schemes to **interpolate**.

### Figure IX-3

A continuous sensor maps the **unit line segment** into a CO system by mapping three world points 0.0, 0.5, and 1.0 to the **string CO**. Intermediate points like 0.4 are mapped automatically to interpolated CO positions.

Suppose desired data is a floating-point variable in the range [0, 1]. We map the unit line segment **linearly** onto a one-dimensional **string CO**.

As a sensor, the converter accepts values like 0.4 and produces a state point in target cells from the position on the string CO that is four-tenths along the string from zero to one.

The state point will be **exactly on the string CO**—since the input is one-dimensional, the state point cannot be elsewhere.

As an actor, the converter finds the point on the string CO closest to the current state point and provides the corresponding data value. It also provides a **strength** measuring the proximity of the match. (See Figure IX-4.)

### Figure IX-4

The continuous sensor maps a string CO to a world unit line. The **state CO** normally lies off the exact line between string CO elements. The closest point is chosen and mapped to the world, with a strength reflecting closeness.

Usually, this kind of **continuous linear converter** suffices for many CO system applications. Nonlinearities can be handled internally by CO system means such as string COs.

---

## Ordinal Converters

An **ordinal converter** is a cross between a cardinal and continuous converter. An **ordinal set** is a cardinal set with an **order** among members, e.g., the **alphabet**.

To build an ordinal converter, we create one rule for each ordinal member—as with cardinal converters—but map ordinal members **evenly spaced** along the **string CO** of a continuous converter. (See Figure IX-5.)

### Figure IX-5

This sensor maps the **alphabet**, an ordinal set, onto a **string CO**.

As a sensor, the ordinal converter accepts an ordinal member from the world; the rules match strongly, and the target state CO takes the corresponding string CO point.

Nearby string CO rules also match with strengths decreasing with **ordinal distance**. (See Figure IX-6.)

### Figure IX-6

Points in the plane illustrate the sequential relationship of string CO elements. Height represents distance in CO space from the reference CO (white). This distance relationship applies to any string CO element.

Nearby points influence the target state CO, but less strongly. Thus, ordinality is represented as **superposed state COs** with strengths diminishing with distance.

As an actor, the ordinal converter accepts a source state CO and produces match strengths with every string CO point paired with an ordinal value. These strengths are provided to the world, and external programs decide how to use them.

---

## Higher Dimensional Converters

Higher dimensionality makes sense only for **continuous** and **ordinal converters**. Cardinal converter data are discrete and unordered, so imposing higher dimensional order is inappropriate.

- A **one-dimensional continuous converter** maps a data line segment to a one-dimensional string CO.
- A **two-dimensional continuous converter** maps a planar segment to a two-dimensional string CO, and so forth.

Rules are placed at selected points to sample and define the mapping. (See Figure IX-7.)

### Figure IX-7

A two-dimensional world region is mapped into a CO system. Four black points in 2D world map to four members of a 2D string CO. Distances between world points correspond to distances between CO points. Gray points in the world map to proportionate points on the string CO.

Ordinal converters are defined similarly, sampling mappings of continuous world objects like line or planar segments onto corresponding string COs in the required dimensionality.

Continuous and ordinal converters need not be limited to regular geometric figures—they can handle curved lines, deformed surfaces, and irregular solids.

---

## Converter Form

Revisiting the simple continuous converter (Figure IX-3) that linearly maps [0, 1] to a 1D string CO: What if we want to connect 0 and 1 to form a **ring**?

A one-dimensional string CO does not have this property; its endpoints are likely at **standard distance** from each other.

The solution is to use a **ring CO**, which joins the endpoints of a string CO to form a ring (see Figure IX-8). This was mentioned briefly at the end of Chapter VII.

---

### Figure IX-8

- World points 0.0, 0.2, 0.4, 0.6, 0.75, 0.8, 1.0 map onto ring CO points labeled **a** and **b** as well as intermediate points.  
- Ring CO provides a continuous cyclic representation, allowing ends to be adjacent while maintaining CO structure.

## 1.0/0.0 to 0.8  
- **0.2**  
- **0.4**  
- **0.6**  
- **0.75**  
- **0.8**

## World Ring CO  
**b/a**

### Figure IX-8  
On the left is a **continuous converter** that maps the **real number line** from zero to one onto a **string CO**, first shown in **Figure IX-3**.  

The string CO element labeled "**a**" is probably at **standard distance** from the string CO element labeled "**b**." But we can make "**a**" and "**b**" be the same string CO element, as shown on the right, by using a **ring CO**.

### Figure IX-9  
Converters can map any **geometrical relationship** into or out of a CO system. This includes familiar shapes such as the **sphere** and **torus**. It also includes more exotic shapes such as the **Möbius strip** and the **Klein bottle**.  

Sometimes it is important to maintain and manipulate **geometrical relationships entirely within a CO system**. This, too, can be done with **string COs**.

---

## Correlithm Object Technology  

Any **geometrical object** can be mapped into **CO space**. The only requirement is that the CO space be of suitably **higher dimensionality** than the object.  

The mapping can get very complex, but it is always possible. This includes both **finite and infinite objects**, **fractal objects**, and objects from **alternative geometries** such as **Riemann space** and **Lobachevsky space**. (See Figure IX-9.)

---

### Form and Topology  
The **general problem of form** refers to whether or not the **endpoints are joined**.  

- Joining the endpoints of a line segment produces a **ring**.  
- If endpoints are not joined, it is an **open line segment**.  
- Join one pair of opposite edges in a rectangle and the result is a **tube**.  
- Join the other pair of opposite edges, or equivalently the ends of the tube, and you get a **torus**.

> **Note 14:** The joined edge can be twisted before joining.  
> For example, in a **positive unit cube**, we could “roll the y-axis” by joining (0,0) to (0,1), (1,0) to (1,1) etc., to form a tube.  
> We could also “twist and roll the y-axis” by joining (0,0) to (1,1), (1,0) to (0,1) to form a **Möbius strip**.  
> Although not discussed in detail here, such forms present no significant problems. If the **topology** can be defined, it can be mapped onto a suitable **CO space**.

---

# Chapter X Mechanisms of Behavior  

A **dog** and a **laptop computer** both process information but excel at very different things.  

- Dogs are good at physical tasks like **fetching sticks**.  
- Computers excel at tasks like **balancing a checkbook**.  

These differences are well known and reflect how they interact with the world and the kinds of processing they perform.

---

Many projects have tried to model **animal behavior** in automated processing, mostly with **disappointing results**.  

**Correlithm object (CO) systems** and animals share many behaviors, but these are not artificially added—they **“fall out”** naturally.  

When built from **CO soft tokens**, these behaviors are simply present, possibly due to the CO soft token’s origin in **modeling neurons**.

---

## Implications and Applications  

CO systems “see and do things like things previously seen and done,” just like animals.  

You will find **applications for CO systems** anywhere a **human, animal, or insect currently outperforms a computer**.  

- The obvious area is **robotics**, but just as lasers found applications beyond “ray guns,” CO systems will grow beyond initial expectations.  
- Today, lasers: play music, clean teeth, survey land, carry phone calls, etc.  

Imagine a world where:  

- Every watch, key chain, phone, kitchen appliance, and automobile is as smart as a **hunting dog**.  
- Your computer behaves more like a **friend than a machine**.  
- **Supersmart CO systems** advance knowledge frontiers.

---

We already understand many **mechanisms of behavior in CO systems** well enough to forecast confidently.

---

### Focus of This Chapter  
In Chapter VIII, **CO machine architecture** properties were discussed. Now we focus on:

- **CO processing itself**  
- **Programming with rules** to build a CO system running on a CO machine.

---

### Figure X-1 Synthorg  
A **synthorg®** has:  

- **Sensors** (e.g., antennas, eyes)  
- **Actors** (e.g., wings, legs)  
- An internal **CO system** functioning as its **brain**  

Through its sensors and actors, a synthorg interacts with its environment like a living creature. (See Figure X-1.)

---

### Behavior and Mechanisms  

- The **behavior** of a synthorg applies hierarchically: from the whole organism to groups of related rules to individual rules.  
- **Mechanisms** are **CO programs**, sets of rules running on a CO machine, or methods of writing rules to obtain behavior.

---

### Example Rule  
Consider the rule:  
`s1.p1 >: s2.p2`  

- It relates point **s1.p1** in **space s1** to point **s2.p2** in **space s2**.  
- The **mechanism** compares the source state point in **s1** to pattern point **s1.p1** and influences the position of the target state point in **s2** with reference to **s2.p2**.  
- The **behavior** is that the closer the source state point gets to **s1.p1**, the closer the nominated target state point gets to **s2.p2**.

---

### Purpose of This Chapter  
This chapter examines a few **mechanisms** and their corresponding **behaviors**.  

- It is not a tutorial for building synthorgs but a sampling to illustrate what synthorgs can do.  
- It aims to compare and contrast **synthorgs with traditional computers** and stimulate understanding.

---

## Turing Completeness  

From Chapter VI:  

- **CO systems are Turing complete**—a **general-purpose computing paradigm** capable of computing anything computable.  

- CO systems and **traditional digital computers** have different strengths and weaknesses, but neither is strictly more powerful.

---

### Strengths of Digital Computers  
- Excel at **precision**.  
- Tasks like adding 1 million 10-digit numbers are native strengths.  
- Work best with **crisp, clean, and accurate data** and **exact sequences**.

---

### Strengths of CO Systems  
- Excel at **similarity**.  
- Ideal for tasks like causing a robot to sit in an **unfamiliar chair**.  
- Judging **“chair-ness”** and adapting behavior to the specific chair.  
- Good at **adaptation**, **seeing**, and **doing things like things previously seen and done**.  
- Work well with **approximate data**, **similar circumstances**, and **noisy data**. (See Figure X-2.)

---

### Figure X-2 Analogy  
- Traditional computers process **binary data** effectively.  
- CO systems assess **“chair-ness”**, gauging similarity rather than exact matches.

---

## Soft Tokens  

### Hard Tokens (Digital Computer)  
- Represent all data with **hard tokens**.  
- A token either represents a piece of data or not—**sharp edges**, no intermediate states.

---

### Soft Tokens (CO Systems)  
- Represent all data with **soft tokens**.  
- A token can represent data in a **continuous range** from “fully represented” to “not represented.”  
- Tokens have **soft edges** and an **influence zone** around them.  
- The closer a state point comes to a CO, the more it represents the data. (See Figure X-3.)

---

Within a space, **soft tokens** have **equal stature**. They form a set of **independent objects with no intrinsic ordering**.

---

### Figure X-3 Token Comparison  
- **Traditional hard tokens** have **sharp, well-defined boundaries**.  
- **Soft tokens** of correlithm object technology have **no actual boundaries**, only regions of influence.  
- These influence zones vary in strength like **gravitational**, **electromagnetic fields**, or **quantum wave functions**.

---

### Token Capacity  

- The number of **soft tokens** available depends on how important it is to avoid **random matching errors**.  
- If an error must be at least **six standard deviations from the mean** (gold standard in manufacturing):  
  - A **50-dimensional space** yields about **10 million usable soft tokens**.  
  - A **100-dimensional space** yields about **10^17** soft tokens.  

- Conversely, if only **1 million soft tokens** are needed in a 100-dimensional space, errors would be even less likely (more than **11 standard deviations from the mean**).

## 15 String COs

**String COs** are composed of **COs** that do not have equal stature. These form **soft string tokens**.

---

## Chapter X Mechanisms of Behavior

### Token Properties

**Figure X-4** illustrates the tokens used to represent data in **CO systems**, which have three important properties:

- They all have a **“soft” nature**
- They are all **equal in stature**
- A **huge number** of them are available

These are inherent properties of the underlying mathematics and were not “engineered” into **CO systems**. They rank as the most important **mechanisms of behavior** in CO systems. Without these properties, the study of CO systems might not exist.

---

### Robustness

**Rules built with soft tokens** are inherently **error correcting** and **robust** in the presence of noise.

- Traditional error detection and correction codes select **uniformly sparse points** in high-dimensional spaces like **N-bit binary spaces**.
- These points are sparse enough that small errors create points closer to one selected point than any other.
- Randomly selected points in high-dimensional space also exhibit similar or better behavior because they tend to be sparse and uniformly spaced.

**Figure X-5** shows:

- In a high-dimensional space, any three random points are about the same distance apart.
- Two points close together are unlikely to be random; they likely represent the same thing with some noise.
- Points A and B were probably picked by unrelated processes, while A and C result from the same process plus noise.

We can retain noise to preserve **confidence in data**, as shown by the rule:

``` 
s.p >: t.q
```

- A state point in space **s** near pattern point **s.p** will nominate a state point in space **t** near pattern point **t.q**.
- The noise in s.p is transferred to t.q, preserving both **identification** and **confidence level**.
- If the state point in **t** were corrected exactly to **t.q**, confidence information would be lost.

---

### Superposition

The **superposition mechanism** of CO systems allows a state point in a space to represent **two or more things simultaneously**.

- A state point can be near multiple pattern points at once, defining regions with various distance ratios to pattern points.
- These regions result from intersections of “soft spheres” surrounding pattern points.

**Figure X-6** example:

- Points **s.p1**, **s.p2**, and **s.p3** are about the same distance apart.
- Point **s.state** is about the same distance from **s.p3**, but roughly half the distance from **s.p1** and **s.p2**.
- **s.state** can simultaneously represent **s.p1** and **s.p2**.

This mechanism means:

- A state point can represent any **ratio of soft tokens** by residing in the right region.
- Called **superposition** because one state point represents all the soft tokens and their weight ratios.
- This allows representing any subset of pattern points with one state point.

An example involves mapping points arranged in the shape of the letter **“A”** to a space **s**. A superposition of all pattern points **s.Ai** forms a single state point **s.A** representing the entire geometric pattern. Minor shifts or missing points still result in a state point close to **s.A**, illustrating strong **data fusion** capabilities.

---

### Strings

Most mechanisms available with point COs also apply to **string COs**.

- A string CO is a path surrounded by an **influence tunnel**.
- Thinking of a string as a path swept out by a point, the tunnel is swept out by the point's influence zone.
- A state point near a string CO is likely a noisy version of a position along that string.

Thus:

- A point CO represents a **soft token**.
- A string CO represents a **soft sequence**.
- **CO systems** can represent, abstract, and approximate both soft tokens and soft sequences.

**Figure X-7**: A soft string CO is a series of closely placed COs with overlapping influence zones forming an influence tunnel to represent soft sequences.

---

### Patterns and Functions

Most **mechanisms** are crucial for both **pattern recognition** and **function generation**.

- We are familiar with recognizing **patterns similar to known ones**.
- Generating behaviors that are **like known behaviors** is less familiar but feasible in CO systems.
- CO systems can adapt previously learned **actions or skills** to current situations.

Example code:

```
s1.pattern s2.context1 >: s3.action1
s1.pattern s2.context2 >: s3.action2
```

- Given state point in space **s1** near **s1.pattern** and state point in **s2** near different contexts, the system nominates different state points near **s3.action1** or **s3.action2**.
- The generated behavior adapts based on context.
- This easily generalizes to complex behaviors.
- For instance, a robot programmed to walk on flat ground can adapt to uneven or sloped terrain using the same small set of rules.

This capacity to **adapt function generation** distinguishes CO systems from traditional computer systems.

---

### Rule Pathways

**Sets of rules define pathways** through a CO system.

Example rules:

```
a.p >: b.q
b.q >: c.r
```

Compact pathway notation:

```
a.p >: b.q >: c.r
```

Adding another pathway:

```
a.p >: b.q >: c.r   # previous pathway
d.s >: b.q >: e.t   # new pathway
```

- Both pathways share the pattern point **b.q**.
- State points near **a.p** or **d.s** cause nomination of state points near **b.q**.
- These nominated points are combined into a single state point in **b** via **superposition**, carrying forward proximity confidences.
- This combined state point in **b** nominates state points near **c.r** and **e.t** accordingly.

## Process Pathways Interaction

In effect, processes following both **pathways** will pass through **b.q** and continue on. But there will be **interaction between the pathways**.

Suppose the **state point** in **a** is close to **a.p**, while the state point in **d** is about **standard distance** from **d.s**.

The nominated state point in **b** will be close to **b.q** because of the state point in **a**. But this proximity will cause the nominated state points in both **c** and **e** to be near **c.r** and **e.t** respectively.

The situation is actually even more complex than this, because we could arrange to **“favor”** for example **c.r** over **e.t** in this circumstance.

**Converging pathways** can be complex, so be sure your **code** does what you intend.

## Simplified Notation for Pathways

The preceding narrative is complex due to the operations implied by the notation. It should be understood how rules deal with **pattern points** and **state points**. To simplify, we describe the rules as:  
**“a.p or d.s drives b.q and b.q drives c.r and e.t.”**

We also adopt a simpler notation for pathways:

- `p1 >: p2 >: p3` where: **p1 = a.p**, **p2 = b.q**, **p3 = c.r**  
- `p4 >: p2 >: p5` where: **p4 = d.s**, **p5 = e.t**

Since all five spaces and points are distinct, these substitutions are unambiguous. For example, **p1** means “a random point in a random space,” and it is not important which points are in which spaces, only that they differ.

---

## Chapter X Mechanisms of Behavior

### Grammar Systems

The rules of **CO systems** can build production systems of **formal grammars**. The strategy is to:

- Use **converters** to get terminals (world data) into and out of the system  
- Use **soft tokens** to code all non-terminals  
- Code the actual productions almost directly as CO rules  

Some unique phenomena happen in this approach, especially concerning **trees**.

---

### Parse Trees

Using the simplified notation, consider the parse tree shown in **Figure X-8**.  

- If **p1** or **p2** match, then **p5** will be built.  
- If **p3** or **p4** match, then **p6** will be built.  
- If **p5** or **p6** match, then **p7** will be built.

Key parts of the tree:  
- Leaves: **p1, p2, p3, p4**  
- Root: **p7**  
- Intermediate nodes: **p5, p6**

Typically, one leaf (e.g. **p1**) instantiates, causing **p5** and then **p7** to instantiate. This shows **p1 is a p5, which is a p7**, so **p1** is a member of the set that can instantiate **p7**.

```
p1 >: p5  
p2 >: p5  
p3 >: p6  
p4 >: p6  
p5 >: p7  
p6 >: p7  
```

---

### Differences Between Traditional and CO Parse Trees

- Traditional systems require **exact matches** at each node.  
- CO systems require only **like matches**.  

In CO systems:  
- State points for each new tree level are a **superposition** of state points below.  
- **p7** instantiates to the strength of the strongest matching pathway from any leaf.  
- CO systems implement **soft parse trees**, while traditional systems implement **hard parse trees**.

---

### Superposition and Concurrency in Recognition Trees

All four pathways from leaves to root are active **concurrently** in a CO system:

```
p1 >: p5 >: p7  
p2 >: p5 >: p7  
p3 >: p6 >: p7  
p4 >: p6 >: p7  
```

- The strongest pathway dominates the state point at **p7**.  
- This enables **concurrent context sensitive searching**, an advantage over traditional systems which may require exhaustive search.

---

### Generation Trees

Generation trees start at the **root** and proceed to the **leaves** (called derivation or generation).

Example shown in **Figure X-9**:

```
q1 >: q2 >: q4  
q1 >: q2 >: q5  
q1 >: q3 >: q6  
q1 >: q3 >: q7  
```

If **q1** matches, all others (**q2, q3, q4, q5, q6, q7**) get built. Usually, only **one** of these four pathways should dominate.

---

### Statistical Competition of Pathways

- All four pathways can be approximately equal strength initially.  
- Because building state points is statistical, one pathway tends to be slightly stronger than others.  
- This creates a natural bias that grows stronger through subsequent nodes.

Example:

- If **q2** is stronger than **q3**, then **q4** and **q5** will be favored over **q6** and **q7**.  
- Further bias between **q4** and **q5** emerges statistically.

---

### Using Attractors to Choose Winners

To choose a clear winner, build an **attractor**:

```
q4 s.q4 >: s.q4  
q5 s.q5 >: s.q5  
q6 s.q6 >: s.q6  
q7 s.q7 >: s.q7  
```

- Space **s** has four points (**s.q4** to **s.q7**) representing leaf nodes.  
- The strongest leaf triggers the corresponding rule, **latching** into its region in **s**.  
- This **latch** remains stable until influenced outside.

---

### Using Context Rules to Bias Pathways

Adding **context rules**:

```
t.q8 >: q4    # first new rule  
t.q9 >: q5    # second new rule  
```

- Space **t** has two points: **t.q8** and **t.q9**, which are **cardinal** (one strong means the other is weak).  
- **t.q8** and **t.q9** bias which of the **q2** pathways is favored.  
- By managing size or strength of **t** points, pathway competition can be controlled.  
- Similar context rules can be added for **q3** or all **q1** pathways.  

Example equivalence with bit comparison:  
```
q2 >: q4  
t.q8 >: q4  
```
is equivalent to  
```
q2 t.q8 >: q4  
```

Bits are assumed unless otherwise stated.

---

## Ladders

### Overview

Rule pathways can branch and converge arbitrarily.  
A common higher-level structure is the **ladder** (see Figure X-10).

The ladder mechanism provides:  
- **Hierarchical pattern recognition** via through-feed paths  
- **Function generation** via through-feed paths  
- **Attractors as short-term temporal memory** in feedback paths  
- **Context** in cross-feed paths  

---

### Ladder Structure and Rules

Each space takes input from up to three spaces:

- **Feedback space** (itself)  
- **Through-feed space**  
- **Cross-feed space**

Example (rules r1 to r8):

```
pr0.s in.s fg0.s >: pr0.s # r1  
pr1.s pr0.s fg1.s >: pr1.s # r2  
pr2.s pr1.s fg2.s >: pr2.s # r3  
pr2.s x.s >: x.s       # r4  
x.s fg2.s >: fg2.s     # r5  
pr1.s fg2.s fg1.s >: fg1.s # r6  
pr0.s fg1.s fg0.s >: fg0.s # r7  
fg0.s >: out.s           # r8  
```

- Points named with **“s”** are preceded by a space name, unique per rule.  
- Points like **fg2.s** appear in multiple rules but correspond to the same point within their space.

---

### Pattern Recognition and Function Generation

- Left path (pattern recognition): **in, pr0, pr1, pr2, x**  
- Right path (function generation): **x, fg2, fg1, fg0, out**

Both paths are typically trees rooted at **x**.  
Cross-feed paths provide contextual links anywhere needed, resulting in complex behavior.

---

### Summary of Ladder Function

- Information is hierarchically summarized in pattern recognition paths into **soft classes**.  
- Functions are hierarchically derived in function generation paths into **soft actions**.  
- Feedback paths provide **hierarchical short-term temporal memory**.  
- Cross-feed paths supply **context** for recognition and generation.  

---

### Complexity of CO Systems

- Rule pathways can be arbitrarily complex.  
- Each **dimension** in the entire CO system can have a unique set of rules and a unique "point of view".  
- Dimensions are often organized into spaces for simplicity, but this is not required in the **CO computing paradigm**.

## Concurrency and Superposition in CO Systems

Indeed, the **concurrency** and **superposition** properties of **CO systems** encourage the opposite. The **state point** in the space **x** is the most general **soft token** in a CO system. It represents a summary of the entire state of the system, including all concurrent and superposed **pattern** and **action classes**.

Recalling from Chapter III that each dimension carries a little more than **four bits** of information, even a relatively small space, **x**, has a state point with considerable **information content** and room for substantial numbers of concurrent **rules** in its **feedback loop**, all in superposition.

The **ladder model** illustrates how **simulation** can take place in a CO system. Input and output to the system can be suppressed by maintaining appropriate **attractor latch states** in spaces at the level of **pr0** and **fg0**, on orders from higher-level spaces. The **cross-feed loops** can then present superposed patterns and actions corresponding to known input and output patterns and actions.

Higher levels can evaluate these simulated interactions as though they were actually coming from the environment. This mechanism is **hierarchical**. Cross-feed paths can establish these conditions at any level, in fact at multiple levels concurrently thanks to superposition.

When a suitable course of action is observed at any level, it can be pursued. We simply release the **latch states** and “play back” the solution. This **simulation capability** allows examining options and selecting actions in very general ways that employ the full representational power of CO systems.

---

## Automata

It is well known that **finite automata** and **formal grammars** have strong equivalences. For example, a **Turing machine** is formally equivalent to a **Type Zero grammar**. We can often benefit by choosing to implement a particular functionality in one equivalent form rather than the other.

For **Turing machines** and **Type Zero grammars**, it does not matter because neither are of much practical use. In the discussion of trees above, we touched on **context sensitivity**.

**Context sensitive grammars** all have equivalent **linear bounded automata**, but we are better off working with grammars, for example as trees, rather than attempting to use the equivalent automata. 

The situation is reversed for **context free grammars** and **linear grammars**. The equivalent **stack machines** and **state machines**, respectively, are significantly easier to work with in most cases. We will examine simple cases of these two important behavioral classes.

---

## State Machines

A **state machine** implements the notion that a system can exist in **one and only one** of a number of discrete **states**. A state machine accepts input that causes it either to stay in the same state or change to a new state. It also generates an output each time it examines its state. (See Figure X-11.)

### Example Description

- The two circles represent the two possible **states**.
- The arrows indicate possible **state transitions**.
- The notation of the form **X/Y** near each arrow means: “if we are in the nearby state and we see an input of **X**, generate an output of **Y**, and follow the nearby state transition arrow.”

Assume we start in the **zero state** on the left. If the next input is **zero**, we emit a zero and transition to the same zero state. If the input is **one**, we emit a zero and transition to the one state. If the input is then one, we emit a zero and stay in the one state. If the input is zero, we emit a one and transition back to the zero state.

In other words, whenever we are in **state one** and receive a **zero**, we emit a one. Otherwise, we emit a zero no matter what we receive.

### CO Rules Equivalent

The CO rules on the right are equivalent. We use the same mechanism we employed in Chapter VI to implement a **flip-flop**, namely two spaces of different size, notated here as **“s1 >> s2.”**

- The space **s2** will be our **latch space**, and s1 will control **state transitions**.
- The notation **out(X)** invokes an actor that will generate **X** to the outside world.
- The rules follow the description in the previous paragraph.

Points such as **s1.0** and **s1.1** are not zero and one, but **CO pattern points** that represent zero and one in their respective spaces. If these soft tokens actually represent the numerical values of zero and one, then the CO system implementation shown is equivalent to a traditional state machine implementation.

But suppose **s1.0** and **s1.1** in fact represent "**red**" and "**green**." If the input mechanism gives us exactly red or green as input, then the behavior is the same as when the soft tokens represent the numbers zero and one.

What would happen if the input is "**pink**," in other words a "**little bit of red**"? Then we would get a **state point** in **s1** that is significantly close to **s1.0** and essentially standard distance from **s1.1**. These rules would operate "**weakly, but correctly**" on such data.

We would need an input of pink that is indistinguishable from white for this system to fail. Such behavior is not available in a traditional state machine implementation. The CO system will continue to provide the programmed state machine behavior even if the tokens contain substantial noise.

---

## Stack Machines

A **stack machine** is a **state machine** plus a new device, the **stack**. A stack accepts an arbitrary number of discrete tokens, one at a time, and returns the tokens in the **reverse order of receipt**.

- We **push** a token onto the top of a stack.
- We **pop** the top token off of a stack.

Suppose we push token **A** onto stack **S**, then push token **B**, then push **C**. The topmost token will be **C**. If we pop the stack, the topmost token will be **B**. If we pop it again, the topmost will be **A**.

### CO Rules for Stack Machine (Two-Level Example)

To code a stack machine using CO rules, we need:

- A finite number of **stack positions**
- A finite number of **token types** that we can put on the stack

We illustrate a simple example with a **two-element stack** (empty plus two slots) and **two tokens**. (See Figure X-12.)

- The current top of the stack is held in space **s1**, which has the pattern points **empty**, **L1**, and **L2**.
- For each level of the stack, a separate state machine is created to hold one of the possible stack tokens at that level.

Assume that the stack starts in the empty state, **s1.empty**.

- When we receive **s2.push**, we go to state **s1.L1**.
- Another **s2.push** command takes us to state **s1.L2**.
- Subsequent push commands do nothing.
- If we receive **s2.pop** while in **s1.L2**, we go to **s1.L1**.
- If we then receive **s2.pop**, we go to **s1.empty**.

Suppose we are in **s1.empty** and we receive **s2.push** and simultaneously receive the soft token that represents **“A”** from the sensor **in()**.

- We move to **s1.L1** in the stack, and we set the space **L1 point** to **L1.A**.
- If we receive **s2.pop** now, we will move to stack level **s1.empty** and simultaneously emit an **“A”** through actor **out(A)**.
- If instead we receive **s2.push** and simultaneously receive the soft token that represents "**B**" from the sensor **in()**, then we move to **s1.L2** and set **L2.B**.
- Two consecutive **s2.pop** commands would then cause the **out()** actor to emit **B**, then **A**, and we would end up in **s1.empty**.

---

## Generic Stack Machines

This approach could obviously be extended to deal with a stack with any specific number of **levels** and **tokens**.

But suppose we do not want to specify beforehand the maximum number of levels that a stack may have. Consider the following code:

```
s2 >> s1
s1.L[i] s2.push >: s1.L[i+1] (s1.L[i+1] s2.pop >: s1.L[i])
```

- Look at **s1.L[i]**. The point **L** in **s1** has a subscript, **i**. This is simply notation. 
- It does not matter to a CO system whether we say **s1.a**, **s1.b**, **s1.c** or we say **s1.L[0]**, **s1.L[1]**, **s1.L[2]**.
- Either way, we have defined three pattern points in space **s1**.
- By using a subscript, we avoid picking endless pattern point names.
- We assume here that the subscript starts with zero.

Now look at this portion of the code:

- `s1.L[i] s2.push >: s1.L[i+1]`

This instructs the CO system to move to state **s1.L[i+1]** if it is currently in state **s1.L[i]** and also receives command **s2.push**.

But suppose we have not previously defined **s1.L[i+1]**. The system simply creates a pattern point called **s1.L[i+1]** in **s1** during execution.

Remember, all randomly selected points in a space are essentially equal for our purposes, and there are a huge number of such points available in any suitable space.

Contrast this to:

- `s1.a s2.push >: s1.b`

where both pattern points must be predefined.

Now consider the entire line of code:

```
s1.L[i] s2.push >: s1.L[i+1] (s1.L[i+1] s2.pop >: s1.L[i])
```

- The parentheses on the right end define a **temporary rule**.
- This is in fact a **short-term memory mechanism** for CO systems.
- When the left-hand side of the rule, **s1.L[i] s2.push**, is matched, the temporary rule is instantiated during execution as a new rule.
  
Afterwards, the system actually has two rules:

- `s1.L[i] s2.push >: s1.L[i+1]`
- `s1.L[i+1] s2.pop >: s1.L[i]`

Together, these two rules form a **push-pop connection** between two levels of a state machine defined in space **s1**.

If we are in state **s1.L[i]** and we receive command **s2.push**, then we move to state...

## 16 The Details of Temporary Rules

The details of **temporary rules** are more complex than can be fully addressed here. The simplification above captures the **essence** of the idea.

## 158 Correlithm Object Technology

### Stack State Machine and Commands

- The state machine is extended with states such as **s1.L[i+1]**.
- If in state **s1.L[i+1]** and the command **s2.pop** is received, the system moves back to state **s1.L[i]**.
- Every time the **s2.push** command appears, it:
  - Establishes a new **state or stack level**.
  - Creates a new, never-before-generated **pattern point** in **s1**.
  - Adds a temporary rule allowing the **s2.pop** command to navigate back to the previous state.

- When the **s2.pop** command is issued at a given level:
  - The new pattern point **s1.L[i+1]** is abandoned.
  - The new rule is **not deleted** but will never activate again.
  - This is because there are many random pattern points in **s1**, making it unlikely for the same or nearby point to be generated again.

### Adding an Arbitrary Number of Tokens at Each Level

- Now the system can generate a stack with an **arbitrary number of levels**.
- The goal is to add an **arbitrary number of tokens** at each level.

```plaintext
s1.L[i] s2.push >: # the source side of the rule
s1.L[i+1] S[i+1].L[i+1](state) # the target side of the rule
(s1.L[i+1] s2.pop >: s1.L[i] S[i+1].L[i+1]); # the temporary rule
```

- The space **S** code is added to the previous code.
- The second line introduces:  
  **s[i+1].L[i+1](state)**

### Purpose of (state) Annotation

- This instructs the **CO system** to:
  - Capture the current state point in space **S[i+1]**.
  - Give it the name **S[i+1].L[i+1]**.
- Leaving **(state)** off would cause the CO system to generate a **random pattern point**.
- The **(state)** annotation is frequently necessary in a **CO system** to:
  - Capture the current state point.
  - Embed it explicitly in a rule.

This technique provides a method to **capture and utilize state points** within the rule system.

## 17 Physical Implementation of a CO Machine

In a physical implementation of a **CO machine**, there are often means to select subsets of **rules**, deactivate, or even delete rules. Otherwise, the fact that all rules are evaluated all of the time puts an increasing burden on the machine the longer a **stack** is used. But in the abstract, a **CO system** does not need to delete such rules.

Notice that we are selecting a random space, **S[i+1]**, as well. We did this just to illustrate that it can be done. It is not needed for a simple **stack**. In this case, it also serves the purpose of guaranteeing that the **state point** will be a random point in the space. If we just captured the **state point** in the same space, it might not be random, which might or might not be what we want. But by generating a new space every time, the **state point** in that space will be random.

The third line creates a "**temporary**" rule that watches for **s2.pop** to appear in the context of state **s1.L[i+1]** and when it does, the previous state, **s1.L[i]**, appears. The rule also nominates the **state point** that we captured in **S[i+1]**.

Thus we can build a **stack of arbitrary depth** that can hold an arbitrary set of tokens. We have also shown here one way to capture the current **state point** in some part of a **CO system**. This is important in **CO systems** that can **learn**, because we cannot predict ahead of time what **state points** a **sensor** might produce from environmental data or what **state points** high levels of the **CO system** might produce once **learning** begins.

We remind you again that **rule sets** are **concurrent** and **superposed**. A small set of rules may be driving a large number of behaviors that **co-exist** at varying strengths and levels of instantiation.

We can build an arbitrary **random-access memory** using these same methods. Every time we pick a random space, we in effect implement one "**storage location**." By instantiating a **state point** in the space, we "**load**" that storage location.

A **CO system** has a lot of **storage capacity**. The number of different things that we can "load" into a space is the number of usable (**adequately distinguishable**) points in the space, which we know is very large even for small spaces. The number of usable spaces available in a given **CO system** is also quite large.

Simplistically, if there are a total of **N dimensions** available in the **CO system** and the average space utilizes **M** of those dimensions, then there are at least **N/M** spaces available. But that assumes that the spaces do not overlap.

We know that **superposition** allows multiple processes to be running even in the same space, so we can overlap the spaces extensively, and the number of available spaces is more like the **combinations of N things taken M at a time**. Even for "**tiny**" **CO systems**, this is a big number.

---

## Expert Systems

An **expert system** can be more robust and generalized if they are implemented as a **CO system**.

Expert systems are sometimes called **rule-based systems**, but the rules in expert systems are not the same kind of **rules** that we have in **CO systems**.

An expert system is also sometimes called a **decision tree**, because it asks a question and makes a decision about what question to ask next depending on the answer. Still another term for an expert system is an **if-then-else system**, referring to one way in which the decision tree can be implemented.

Here is a simple expert system about **having lunch**:

- **If hungry then**
  - **If prefer soup then eat soup**
  - **Else eat sandwich**

Expert systems have been used successfully to implement a variety of systems. They have long been touted as a means to capture the often-arcane knowledge of **human experts** in particular fields. There are many interesting cases on record where this has been done.

Expert systems, however, all suffer from one particular difficulty. They do not **generalize well**. One individual has compared an expert system to a **locomotive**. It runs extremely well as long as it is on rails, but get it even a small distance off the rails, and it is useless.

In my college days, a chemistry professor once gave my class a rule of thumb. A student asked, “Professor, when does this rule of thumb work?”  
“It works for the cases to which it applies,” he informed us, “and it does not work for the cases to which it does not apply,” and he turned back to his lecture.

**Expert systems** behave in these ways.

---

## CO System Implementation of Expert System

Let us code our simple expert system as a set of **CO system rules**:

```
s1 >> s2
s1.not_hungry >: s2.do_not_eat
s1.hungry >: s2.soup s2.sandwich
s2.do_not_eat >: s2.do_not_eat
s2.soup >: s2.soup
s2.sandwich >: s2.sandwich
```

Suppose we start off with the **state point** in **s2** latched to **s2.do_not_eat** and the **state point** in **s1** a random point. The match of the **s1** state point with either of the **s1** pattern points will not result in a very strong nomination in **s2**, certainly not enough to break the latch.

If the **state point** in **s1** gets near enough to **s1.hungry**, due no doubt to other rules that set it, then its rule nominates at equal strength two **state points** in **s2**. The resulting **s2 state point** is a balanced **superposition** of **s2.soup** and **s2.sandwich**. In other words, the system is trying to pick one or the other, but it literally cannot decide between them.

The **s2.do_not_eat latch** contributes a nominated **state point** for **s2** with a weight that is some number of bits, and this may add a slight bias that favors either **s2.soup** or **s2.sandwich**. But this breaks the balance the same way every time.

There is actually a low level of **noise** in **CO systems**, and it becomes important in situations such as this one.

The **target state point resolution logic** is usually set to inject a small amount of noise, typically a few bits, during the process of forming a new **state point** from nominations on the target side. We do this by nominating a random **state point** and giving it a tiny weight. This random nomination breaks the balance randomly by randomly placing the resulting **state point** slightly closer to one or the other of the two **s2 pattern points**.

The **attractors** in the last two lines do the rest. The **state point** rapidly moves to the nearer of the two, and the system decides what to have for lunch.

---

## Characteristics of CO-based Expert Systems

This **CO system** will not pick the same thing for lunch every day. What it picks depends on the slight **noise** that breaks the balance.

But this characteristic does not make **CO-based expert systems** exceptionally interesting, because proponents of traditional expert systems can and do employ similar random techniques.

A **CO-based expert system** is interesting for the same reasons that we found trees to be interesting earlier in the chapter. A set of **if-then-else rules** is a **tree structure**.

The inherent **CO system** properties of **superposition** and **concurrency** allow us to search a tree exhaustively, without pruning anything, and to check all of the paths at the same time.

At each node of every path, the matching process makes a **gray-scale decision**, not a black-and-white decision. The result is that each overall path through the tree is optimally matched to its available data, including any noise or imprecision, and the final result reflects all of this partial information.

If the available data “gets off the track” somewhat, the process does not break. It simply reduces the information content in the corresponding paths.

The result is a "**best available**" or "**fail-soft**" fit of the data to the expert model, and a quantitative measure of how good the fit really is.

---

## Flexible Inputs in CO-based Expert Systems

Now let us return to our **CO-based expert system** example with this perspective.

No longer are we limited to the black-and-white "**hungry**" or "**not hungry**" as inputs. We can have those inputs or anything in between.

Suppose our hunger is "**moderate**." This will be reflected as a **state point** in **s1** that is somewhere between **s1.not_hungry** and **s1.hungry**.

That **state point** may or may not be close enough to **s1.hungry** so that its nominated value in **s2** outweighs **s2.do_not_eat**.

Whether the system actually decides to **eat** (and maybe even how much) depends on the exact position of the **state point**.

If it decides to eat, whether it chooses **s2.soup** or **s2.sandwich** depends on **random noise**. And all of this evaluation is happening simultaneously on all paths through the **CO system**.

These characteristics are intriguing even in this tiny **CO system** example. In a large **CO-based expert system**, they become a dominant system characteristic.

From this, the reader may begin to understand why it is that we claim that the behaviors of **CO systems** are much more akin to those of **living systems** than those of traditional computers.

---

## Continuous Systems

**Correlithm object technology** is inherently a **continuous functionality**.

Because of this, it is straightforward to implement **continuous functional relationships**, including **differential** and **integral equations** and continuous **feedback control systems**.

We have previously seen examples of the implementation of discrete digital components such as **NAND gates** and **flip-flops** using **CO technology**.

Thus, **CO systems** have the ability to implement either or both **continuous** and **discrete technology** in the same system.

In the following three sections, we will begin exploring **continuous functionality** in a **CO system**.

---

### Why Build Continuous Systems Using CO Technology?

Why would we want to build continuous systems using **CO technology**?

Again, the **CO computing paradigm** is fundamentally **continuous**.

Continuous systems can directly exploit this inherent **continuous functionality** as well as the **robustness**, **concurrency**, and **superposition** of **CO systems**.

We intend to build **analog CO hardware** at some point. Continuous systems are the natural partner for **analog hardware**.

Failure to build continuous systems using **CO technology** would be a failure to fully utilize the **CO technology**.

---

## Functional Relationships

We can build arbitrary **continuous** (and **piecewise continuous**) functional relationships by exploiting the **interpolation capabilities** inherent in **rules** and their execution.

Consider the following code:

```
s1.p1 >: s2.q1
s1.p2 >: s2.q2
```

As usual, **s1** and **s2** are randomly selected spaces of suitable sizes, each with two randomly selected **pattern points** defined.

Suppose the state variable, **s1.state**, is caused to move from **s1.p1** toward **s1.p2** by distance **f**, where **0.0 <= f <= 1.0**, which is a fraction of the standard distance in **s1**. Denote this as **s1.state(f)**.

Let **distance** be the comparison metric in this case.

The state in **s2**, **s2.state**, that is nominated by **s1.state(f)** will be the proportion **f** of the distance in **s2** from **s2.q1** to **s2.q2**.

The first rule will nominate **s2.q1** in **s2** with weight **(1-f)**, and the second rule will nominate **s2.q2** in **s2** with weight **f**.

Recall that **first moments** are used to resolve rule nominations within a space.

The first moment of these two weighted nominations in **s2** is:

\[
\frac{(1-f) s2.q1 + f s2.q2}{(1-f) + f} = s2.q1 + f (s2.q2 - s2.q1)
\]

which is clearly a **linear expression** of the form **m * f + b**, where **m** is the slope, **b** is the intercept and **f** is the independent variable.

Thus, the two rules together will map positions of **s1.state** along a line between **s1.p1** and **s1.p2** to linearly proportional positions of state **s2.state** along a line between **s2.q1** and **s2.q2**.

_(See Figure X-13.)_

---

### Non-linear Relationships

We can also implement any **non-linear relationship** to arbitrary precision.

There are a variety of means available to do this.

Here is an example of one of them. Consider the following rules:

```
s.p1 >: t.q1
s.p2 >: t.q2
s.p3 >: t.q3
s.p4 >: t.q4
```

---

## Figure X-13: Linear Interpolation Example

Suppose we have a rule that maps **s1.p1** to **s2.q1** and a second rule that maps **s1.p2** to **s2.q2**.

Then, **state points** in **s1** along a line from **s1.p1** to **s1.p2** will map proportionately to **state points** along a line from **s2.q1** to **s2.q2**.

![Figure X-13 Linear Interpolation Example](image-placeholder)

## String CO Points and Floor Mechanism

Suppose we pick **s.p1** at random in **s**, then select **s.p2** randomly from the set of points that are about **10 percent of standard distance** from **s.p1**. We likewise select **s.p3** 10 percent from **s.p2** and **s.p4** 10 percent from **s.p3**. These four points form a string **CO**.

Next, we select all four indicated points in **t** at random and instantiate the rules. When evaluating these rules, we will use a **floor mechanism**. The target nomination of any rule whose pattern point is more than a given percentage of standard distance from **s.state** will receive a **zero weight**. The percentage criterion is the "**floor**." This will have the effect of selecting which rules to include in target nominations.

For example, suppose **s.state** is about halfway between **s.p2** and **s.p3**. Then the distance to each will be about 5 percent of standard distance. The distance from **s.state** to **s.p1** or **s.p4** will probably be less than 15 percent of standard distance, but greater than 10 percent. Suppose we select a floor of **10 percent**.

Then the nominations of **s.p2** and **s.p3** will receive **non-zero weights**—equal weights in this case—while the nominations of **s.p1** and **s.p4** will receive **zero weights**. Any nominations involving pattern points in **s** that are further away will also receive zero weights.

This means we can use a **piece-wise linear approximation** for any non-linear function. We have just shown how to isolate the line between **s.p2** and **s.p3** from any other points in **s** by using a floor.

Our previous example showed how to do **linear approximation** between two such points. Similarly, we can isolate any two adjacent points in the string **CO**, for example, **s.p1** and **s.p2**, or **s.p3** and **s.p4**, then do a different linear approximation for each segment.

We might, for example, extend these rules with sensors and actors as follows:

- `in(0) >: s.p1 >: t.q1 >: out(0)`
- `in(1) >: s.p2 >: t.q2 >: out(1)`
- `in(2) >: s.p3 >: t.q3 >: out(4)`
- `in(3) >: s.p4 >: t.q4 >: out(9)`

The first pair of rules maps the interval from zero to one **linearly** to the interval zero to one. The second pair of rules maps one to two linearly to one to four, and the third pair maps two to three linearly to four to nine. (See Figure X-14.)

Clearly, we can implement **any function of one variable** with this floor technique. We can extend it easily to functions of multiple variables by using string **COs** to embed a higher-dimensional function domain or range in a **CO space**. The local bounding point COs of the string CO can be selected by the floor, then used for linear approximation.

Controlling the floor value in a CO system is an advanced topic that we will touch on in the section called "**IntraCO System Control**" below.

---

### Figure X-14

**String CO points** in space **s** representing the integers 0, 1, 2, and 3 are piecewise linearly mapped to random **CO points** in space **t** representing the integers 0, 1, 4, and 9 respectively. Using a **floor** allows only the nearest two string CO points in **s** to be involved. Intermediate points in **s** are mapped linearly to corresponding points in **t**.

---

## Quantization Mechanism

The **quantization mechanism** is another way to implement arbitrary functions. Suppose we have the same rules as defined above, namely:

- `in(0) >: s.p1 >: t.q1 >: out(0)`
- `in(1) >: s.p2 >: t.q2 >: out(1)`
- `in(2) >: s.p3 >: t.q3 >: out(4)`
- `in(3) >: s.p4 >: t.q4 >: out(9)`

Again, suppose that the four points in space **s** form a string **CO** with uniform spacing of **ten percent of standard distance**.

We will use **inverse-probability weighting**, that is, each rule will use **2 bits** as its weight for its nomination, where the value of bits is computed from the distance metric as shown in **Chapter III**. This effectively "compartmentalizes" space **s** into regions with sharp boundaries around each pattern point.

For example, suppose **s.state** moves from **s.p2** to **s.p3**. Initially, **t.q2** will have a huge weight compared to any of the other target pattern points. In a very small interval around the halfway point between **s.p2** and **s.p3**, the target pattern point will switch to **t.q3**.

Suppose space **s** has **100 dimensions**. If **s.state** is essentially at **s.p2**, then the weight on the nomination **t.q2** will be roughly **2414**, the weights on **t.q1** and **t.q3** will each be roughly **2394** and the weight on **t.q4** will be something like **2374**.

The first moment of the nominations with those weights will essentially ignore all but **t.q2**. As **s.state** moves to the halfway point, the weight on **t.q2** will drop to around **2404**, the weight on **t.q3** will grow to the same number, and the other two weights will both be about **2384** or so. As **s.state** passes the halfway point, the situation is symmetric to the first case but favors **t.q3**. (See Figure X-15.)

---

### Figure X-15

The **quantization mechanism** has abrupt transitions between regions. As the state point in space **s** moves along the horizontal axis, the state point in space **t** “snaps” along the vertical axis from the immediate vicinity of one pattern point to the immediate vicinity of another.

---

This method is probably more useful for **sampling purposes** than for interpolation. It produces tightly quantized regions of space with **sharp boundaries**.

The quantization mechanism should appeal to traditional **hardware designers** because it makes a CO system behave much more like traditional **digital hardware**, where state transitions are traversed quickly and states are driven hard into either **saturation** or **cutoff**.

---

## Differential Equations

The preceding section shows that we can implement arbitrary functions as **continuous CO systems**. If we can show how to build a **differentiator** and an **integrator**, then we can extend the utility of continuous CO systems to include **differential** and **integral equations**.

A continuous differentiator can be built by using a **delay**. Consider this rule:

- `s.p1 >: r.p1`

Suppose the rule takes a finite amount of time, **∆t**, to map its input to its output. If **s.state** approaches **s.p1** at time **t**, then at time **t + ∆t** **r.state** will be near **r.p1** in space **r**.

If we write a set of rules of the form:

- `s.p[] r.p[] >: v.p[]`

we can map all interesting values of a variable, represented by **s.p[]**, and its previous value, represented by **r.p[]**, to another variable, represented by **v.p[]**.

In other words, we can build:

**y(t) = D(x(t), x(t - ∆t))**

as a series of rules with (for example) floors that provide samples across the interesting domain and range of the function, and use linear approximation in between samples.

In particular, the function, **D**, could be designed as a **piece-wise linear approximation** to the derivative of **x(t)**.

A continuous integrator can be built in the same fashion. Recognizing that **z(t)**, the integral of **x(t)**, can be realized as **z(t - ∆t)**, which is the integral up to **x(t - ∆t)**, plus **x(t)**, or:

**z(t) = I(z(t - ∆t), x(t))**

we can use rules of the form:

- `s.p[] r.p[] >: r.p[]`

Again, we will provide rules that sample the interesting domain and range of the function, in this case function **I**.

These rules comprise a **state machine** that uses the current state in space **r** and a new state in space **s** to produce the next state in space **r**. This gives us our desired continuous integrator.

For the most part, we have assumed that rules execute in a **discrete fashion**. We often talk about executing all of the rules during one cycle of the CO machine. But this is merely the reflection of a need to do things discretely if we are to emulate a CO machine on a current digital computer. There is no intrinsic need for cycles.

Rules can be executed **continuously** as continuous mappings on suitable hardware. If this is done, then the above arguments still hold, because no physical hardware, continuous or otherwise, can execute a rule in zero time.

There will be a **non-zero time delay** from the time a state point in a source space assumes a particular value until the image of that state point is developed in a target space.

This time delay can be the **∆t** described above that facilitates the implementation of continuous functions, differentiators, and integrators.

---

## Feedback Control Systems

We can build arbitrary **feedback control systems** using **CO technology**.

This follows immediately from the ability discussed above to build **continuous functions** and especially **differentiators** and **integrators** as CO systems.

The various other components that are frequently required, such as **adders**, **multipliers**, or more complex functions such as the **square root function** or various **trigonometric functions** can all be implemented with what has already been demonstrated.

To get any feedback control system we want, we simply create and assemble the necessary CO components to form still larger CO components, until we arrive at the required system.

---

## Artificial Neural Networks

An **artificial neural network** is a system that relates two spaces. In the usual formulation, we have space **A** and space **B** and there is a function, **F()**, that maps **A** to **B**, or in other words:

**B = F(A)**

Initially, **F()** is unknown. We are given a set **T** of point pairs,

**T = {(aᵏ, bᵏ) | k = 1..K}** 

that are (possibly noisy) samples of **F()** such that **bᵏ = F(aᵏ)**, or at least **bᵏ ≈ F(aᵏ)**.

Two phases are defined: a **training phase** and an **execution phase**.

During the **training phase**, we use the training set **T** to obtain an estimate, **F'()**, of **F()**. During the **execution phase** we use **F'()** to map arbitrary points in space **A** to their images in space **B**.

If you think that this sounds a bit like a **CO system**, you are right. There are some similarities. But there are also important differences.

Both **CO systems** and **artificial neural networks** map points from one space to another. But much of the focus in artificial neural networks centers around how to obtain **F'()**.

Many clever techniques have been developed. Once **F'()** has been obtained, the interest shifts to using **F'()** for estimation work. Sometimes the interest is focused on certain internal details.

**Artificial neural networks** usually have what are called **hidden layers**. The size and number of these is often a central issue.

**CO systems** normally have nothing similar.

A CO system is **not an artificial neural network**.

At the heart of the difference are the **correlithm object** and the statistics of randomly selected points in **high-dimensional bounded spaces**.

Artificial neural networks do **not** use the **CO soft token**. They take their data as they find it. The training set **T** is typically generated by systems external to the artificial neural network.

That data may be normalized or otherwise prepared, but it is **not mapped to correlithm objects before use**, and the properties of correlithm objects are not central to the operation of the system.

This is a profound difference, because it is the properties of **correlithm objects** and the statistics of **high-dimensional spaces** that enable the **CO computing paradigm**.

If these properties did not exist, the paradigm would not exist either.

We could use artificial neural networks internally to implement the mappings defined by **CO system rules**. In fact, in a sense, we do.

There is a calculation in current CO machines that is a very simple-minded artificial neural network, one that is "**good enough**."

One big reason not to use mainstream artificial neural networks is that most of them take a **long time to obtain F'()**.

The method we use is **very fast**, and CO systems do not need any additional accuracy in **F'()** that might be available from mainstream artificial neural networks.

We can certainly implement artificial neural networks using **CO systems**. The training set, **T**, is a set of pairs of points.

We can easily implement these as **CO rules**.

## Training Point Pair and CO System Rule

If we have the training point pair, **(a,b)**, we simply write the rule:  
**s1.b >: s2.a**

We must arrange for **s1.b** and **s2.a** to be actual data points and not random points. Once the rule is written, any **s1** state point that approaches **s1.b** will be mapped to an **s2** state point correspondingly close to **s2.a**.

The "**simple-minded artificial neural network**" inside the CO machine performs all the work. Implementing an artificial neural network system as a CO system means that all of the inherent properties of CO systems may be available, including **concurrency** and **superposition**.

However, these properties derive directly from the use of **correlithm objects** and the statistics of **high-dimensional spaces**. Hence, we say they **may** be available. Concurrency and superposition will be present but may not perform as expected.

---

## Fuzzy Logic

**Fuzzy logic** was developed by **Dr. Lotfi Zadeh** of **U.C. Berkeley** in the **1960s**. It is a superset of conventional logic extended to deal with the concept of **partial truth**, that is, values of truth between completely true and completely false.

Typically, the logical variables of fuzzy logic are **real numbers bounded by zero and one**. **Membership functions** are defined that capture “conversational” concepts such as **“small,” “medium,”** and **“large.”** These functions typically have a central peak that descends toward zero as distance from the central peak increases.

Often very simple membership functions are used to minimize computational complexity, such as a linear **“inverted V”** or **“triangular”** membership function.

A strong appeal of fuzzy logic is its use of **natural language descriptions**. It can be argued that natural language has evolved into a highly optimized means for representing the information structures found in human minds.

Another appealing aspect of fuzzy logic is its **simplicity**. Complex, effective control systems can be described with a comparatively tiny set of fuzzy logic variables and rules, and there is typically no need to invoke deep mathematics. Adjustments can be made by overlaying additional variables and rules without revisiting the entire fundamental design.

There are strong parallels between the concepts of **fuzzy logic** and those of **CO technology**:

- Both have the concept of **fuzzy or soft representations of data**  
- Both use rules that **map data to data**  
- Both use similar means to **evaluate and combine rules**, for example the **centroid**  
- The **membership functions** of fuzzy logic and the **influence zones** of point and string COs are similar in shape and purpose  

Other parallels can be drawn.

---

### Differences Between Fuzzy Logic and CO Technology

- The biggest difference is the **correlithm object** itself, which emerges from the statistics of points in **high-dimensional spaces**. Nothing similar exists in fuzzy logic.  
- Fuzzy logic is not normally tied to a specific architecture, so the advantages of CO system architecture are absent from fuzzy logic systems.  
- Both fuzzy logic and CO systems were developed to better understand the workings of the **human mind**.  
- They may offer mutual benefits and **collaboration between fuzzy logic and CO systems** could be a fruitful direction for future research.  

---

## Internal Control

At times, different **modes of operation** are needed within a CO system. For example, in the section on Continuous Systems, a **floor mechanism** was used to ensure only particular rules provided non-zero weights to their target cells.

Variations such as distance, bits, and inverse-probability have been used as metrics to compare state points with pattern points.

---

### Implementing Operational Variations

How do we implement these variations? What mechanisms do we use? Living systems face similar problems.

Evidence shows natural **chemical substances** modify the behavior of their own neural systems. **Short-term memory** probably has a chemical component.

Not every need in living neural systems is handled directly by neural information processing; some are handled indirectly. We will take the same approach.

A CO system excels at recognizing situations and acting appropriately.

If a set of rules needs to switch between "normal" and "floor" operating modes, the need will have a **describable situation** that triggers it. We build rules to watch the situation, which direct an **actor**. The world-side of the actor performs the required mode change.

---

### Figure X-16: CO System Control Mechanism

**CO System** → **Control Panel** → **Actor**

The actor operates the control panel of the CO system itself. This can be used to perform **learning** within the CO system or to modify various parameters.

This simple but powerful mechanism extends **rule-level feedback** by including an actor to instruct the external world to modify the CO system.

Alternatively, one could use alternative sets of rules with permanent modes and invoke the right set depending on the need. But using an actor to modify rule processing is a simpler and more elegant solution.

---

## Learning

CO systems must be able to **learn**, meaning they must create their own appropriate rules.

So far, we have primarily dealt with CO systems whose rules are provided by human programmers. Only the **Generic Stack Machine** adds its own rules.

More general learning mechanisms are needed for CO systems to achieve their full potential.

---

### Current State of Learning Mechanisms

Learning mechanisms for CO systems are still in their infancy. Fortunately, advances in **general learning theory**, especially **reinforcement learning**, can be exploited.

The combination of these advances and CO technology promises important future developments, but for now, only introductory material is available.

---

### Limitations of Learning

It is crucial to understand what learning can and cannot do:

- A system can learn to use the information it gathers but **cannot learn to gather new kinds of information** from the outside world.
- Animals, including humans, **cannot see radio waves** even though they can learn to interpret devices that convert radio waves to visible light.
- Similarly, no system can learn to directly affect its environment if its **actors do not affect the relevant parameters**.
- Humans cannot directly learn to fly but can learn to build and fly airplanes, if this potential exists in the environment.
- Learning excludes the creation of **new converters**. CO systems can be provided with new converters, but pure learning won't transform unused converters into relevant ones.

---

### Learning by Adding Rules and States

CO systems learn by adding **rules** and **states**. One key mechanism is the **unfamiliarity mechanism**, where a system watches for **familiar situations** by observing how near state points come to pattern points.

If no rule produces a near match, a new rule should be captured based on the current source and target state points.

---

### Novelty Detector Example

```
# “novelty” detector
latch < input   # this “<” is not a mapping operator

# “forever” latch
latch.otherwise >: latch.otherwise

# “novelty” assessment
latch.otherwise >: novelty.novel
input.known1 >: novelty.known
input.known2 >: novelty.known
…
input.knownK >: novelty.known
```

- The space **latch** must be smaller than the space **input** so known input points can be stronger when matched than a perfectly matched **latch.otherwise**.
- The **state point** in **latch** matches **latch.otherwise** perpetually.
- If any known input points are strongly matched, **novelty.known** will be instantiated; otherwise, **novelty.novel** will be instantiated.
- This structure indicates when an **unfamiliar input** is received.

---

### Integration with Learning Techniques

- The unique example-based processing of CO systems can combine with any **learning technique**.
- CO systems are **general-purpose programming systems**, so any learning technique can be supported.
- A few new rules can provide disproportionate benefits.
- Due to CO systems' inherent adaptability, only a small number of rules may be necessary to deal with complex situations.

---

## Mind

The ultimate goal is to build systems that behave like **living systems**, not just like traditional computer systems.

- Traditional computer systems are precise.  
- Living systems are flexible.  
- Each excels where the other struggles.

---

### Can We Build Minds with CO Technology?

This is an **engineering question**, not a philosophical one.

Can CO technology create systems exhibiting the complex, useful behaviors of living systems? The answer is a resounding **“Yes!”**

---

### Supporting Points

- CO technology was modeled after **living neural systems** and rooted in breakthroughs in the **mathematics of high-level neural ensemble behavior**.
- Many of the **right properties** emerge inherently from the CO model.
- Rule pathways in CO systems have much in common with **thought processes** observable in human minds.
- The most compelling evidence is the **results** obtained through CO systems.

## Chapter X Mechanisms of Behavior

**CO systems** behave more like **living systems** than traditional computers. To build “minds,” one must integrate all of **traditional computer science** into the new **CO computing paradigm**. Although initial steps have been taken, the work is just beginning, with significant benefits expected.

CO technology promises effective solutions to important problems such as:

- **Speech recognition and generation**
- **Vision**
- **Natural language processing**
- **Robotic control**
- **Data fusion**

There is potential for truly **intelligent systems** to emerge from CO technology. Development will be hierarchical and exponential, as more intelligent systems can bootstrap the next level faster. 

In the mid-20th century, Isaac Asimov wrote about intelligent robots with “**positronic pathways**.” This dream may soon be realized, but with **correlithm objects** instead of positrons.

---

## Chapter XI Directions

We argue that **correlithm object technology (CO Technology)** offers new tools for processing information and will lead to what we call "**The Other Computer Industry**." This new industry will create systems behaving more like **living organisms** than machines, affecting all aspects of life profoundly.

### The Other Computer Industry

- Started in the mid-20th century with many projects and failures.
- Nearly every major corporation and many government branches attempted **AI** or **pattern recognition** projects, often with unsatisfying results.
- True success has been rare and elusive.

### Why CO Technology Will Succeed

- Based on a breakthrough in understanding **living neural systems**.
- Emergence of a new, **simple and non-obvious mathematics** describing how data is represented, stored, and manipulated in living systems.
- Incorporating these notions in systems leads to behaviors only previously seen in living neural systems.
- No other competing technology offers similar explanations or results.

### Why CO Technology Was Not Discovered Sooner

- Many behaviors of living systems emerge from the statistics of **high-dimensional bounded spaces**.
- Understanding requires the concept of the **ensemble** — the state as a point in such spaces and the distribution of points.
- The key idea that **random points in a bounded high-dimensional space are all about the same distance apart** is counter-intuitive.
- Fields like **Neurophysiology** focus on **molecular-level biochemistry**, not information processing.
- Neurophysiologists rarely view **pulse rate** as an alternative, equivalent neuron output key to information processing.
- Attempts to analyze individual circuits have failed to produce universal principles.
- Most **computer scientists** focus on simple local properties, which do not capture the global information nature of living neural systems.
- Reliable information arises only when ensembles are **large enough** to stabilize correlithm objects' statistics.
- The mathematics of **correlithm objects** is rooted in **geometric probability**, an abstract and underappreciated field often considered “recreational” mathematics.
- These factors contributed to the late discovery of correlithm objects.

---

### Summary of Characteristics

Here are key characteristics that distinguish **CO systems**:

#### The Correlithm Object

- The cornerstone of CO technology.
- Absent in other computing forms.
- Key to the unique behaviors of living neural systems.

#### Soft Tokens

- Represent data in two ways: as tokens and by capturing the **strength** of the data representation.
- Called “soft” tokens because they represent data to varying degrees.

#### String COs

- Represent geometrical relationships.
- Help recognize words spoken at different speeds or letters distorted by noise.

#### Robustness

- Exists in both the cellular implementation and CO programs.
- The system continues functioning correctly even if many cells fail.
- CO objects have inherent **error-correcting properties** ensuring graceful degradation under noise.

#### Sampling

- Any subset (“sample”) of a correlithm object still represents the data with diminished robustness.
- If a CO object has **N** dimensions, any **M** (20 < M < N) subset also represents the same datum.

#### Superposition

- Two or more COs can share the same dimensions without losing their identity.
- Enables overlaying rules and powerful new data fusion methods.
- Removes the need for careful, non-overlapping system design.

#### Straddlers

- Exploit multiple concurrency levels.
- CO systems can be distributed across thousands of internet nodes or robot swarms.
- Exhibit a “group mind” that is resistant to individual element failures.
- Difficult to understand by observing only a few nodes.
- Exist “nowhere and everywhere” simultaneously.

#### Statistical “Holograms”

- Similarities to holograms built from grains of emulsion.
- Resolution increases with more cells or grains.
- CO systems arise from **statistically emergent properties** rather than light interference.
- Easier to create than light holograms.

#### Virtual Systems

- CO systems can emulate any computing system, e.g., a desktop computer.
- Such emulations, or **virtual machines**, have no physical parts but can still run programs.
- Running on a straddler, virtual machines are **highly robust** and ideal for mission-critical applications.

---

## Continuous Systems

**CO systems** are inherently **continuous systems**, not discrete systems. Any continuous functionality can be built as a **CO system**.

### General Purpose Analog Computers

A **CO system** can be implemented on **analog hardware**. These systems are **general-purpose computers**, meaning a general-purpose computer can be built entirely from **analog hardware**. Some potential advantages include:

- Lower power requirements  
- Smaller device sizes  
- Easier and possibly less expensive fabrication  
- Potentially superior speed  

### Early Applications

Applications for **CO technology** are numerous. Tasks where a person, animal, or insect performs better than a computer system are potential candidates. While **CO technology** may not surpass organisms at these tasks, it can be nearly as good and better than traditional computer systems. Start with simple tasks before addressing complex ones.

### Enhancing Current Systems

Examples where **CO systems** can enhance current computing include:

- Expert systems, which do not degrade gracefully, but **CO expert systems** would  
- Artificial neural networks and fuzzy logic systems lacking convenient ways to tokenize information—**correlithm objects** provide soft tokens and general-purpose computing  

Almost all major computing technologies should be re-examined with **CO technology** in mind.

### Cueing

**Cueing systems** prepare and present data to human experts in probable order of relevance. Experts working where large amounts of irrelevant data must be sifted will benefit from **CO-based cueing systems**.

### Data Mining

Primary shortcomings in current **data mining systems** include:

- Dealing mainly with lexical and syntactic elements instead of the data's **meaning**  
- Difficulty handling noisy data  

There is strong evidence that **CO technology** can overcome these issues.

### Data Fusion

Animals use all their senses to evaluate situations. Traditional computers struggle with fusing diverse information sources into a single coherent situation. **Hierarchies of COs** provide natural ways to perform data fusion, with **superposition** offering important new tools.

### Speech

In the late twentieth century, it was often claimed that "speech recognition is a solved problem." However, this is false. Reliable **speech recognition** requires performance comparable to humans under difficult conditions. It involves signal analysis, syntactic and semantic elements, context, and more. Tools like **string COs** offer significant improvement opportunities.

### Natural Language

Many attempts have aimed to create systems capable of intelligent conversation or accepting natural language instructions. These systems work only in very limited and unambiguous contexts. The primary problem is the absence of a strong, usable model of **meaning**. **CO systems** provide important tools to address this.

### Vision

Animals perform many visual tasks difficult or impossible for traditional computers. **CO systems** offer a fresh approach and new tools including the important **string CO**. Rapid major advances are expected.

### Handwriting

Reading **handwriting** remains a challenging visual task. Despite extensive research, it is far from completely solved. **CO technology** brings new capabilities inspired by living organisms' visual systems and promises quick advances in handwriting recognition.

### Simple Robots

Coordinated behaviors in even simple robots are challenging to program. While **fuzzy logic** has helped, it is insufficient. **CO systems** inherently provide information processing similar to living organisms, potentially improving robotics.

### Security

A crucial goal in security is **anomaly detection**. Security systems generate vast amounts of routine data, making it challenging to detect rare, anomalous events. Traditional computers struggle with "out of the ordinary" detection, but humans excel at it. Similarly, **CO systems** are effective at detecting anomalies.

## The Future

This book serves as a **primer** on major **correlithm object system** concepts, with future volumes planned for more detailed topics.

### Tools

Living neural systems appear complex at microscopic levels; **CO systems** share this complexity in their implementation. Key points include:

- Every data object is a long list of random numbers—**CO data objects** are statistical entities.  
- Source and target relationships among cells are statistical, so no final wiring diagram exists.  
- Functional rules are distributed statistically across cell relationships, increasing complexity.  

High-level tools are essential to handle this complexity. Automation should manage most complexity unless developers must access lower levels. Current challenges:

- Lack of a formal high-level language for **CO systems**  
- Extensions to existing languages help but need improvement  
- Graphical user interfaces exist but require advancement  
- Minimal exploitation of inherent **CO concurrency**  

Rapid progress in tool development is expected due to growing interest in **CO technology**.

### Learning

Learning is a natural progression for **CO technology** but requires prior understanding of **CO systems**. Key points:

- Learning technology is critical for expanding system capabilities  
- Human limitations restrict complexity in traditional systems  
- A **CO system** that learns is limited only by computing resources and infrastructure  
- Resources can grow to support larger **CO systems** without restarting  
- Combining **CO technology** with existing learning methods like **reinforcement learning** is promising  
- Simple learning mechanisms implied by **CO technology** itself should be explored early  

Learning might be the most important future direction in **CO technology**.

### Quantum Information Systems

A particular area of interest is **Quantum Information Science (QIS)** and its application of **correlithm objects (COs)**.

- COs can be imposed on and recovered from **qubits** and **ebits**  
- COs survive quantum measurement  
- The statistics of bounded, high-dimensional **Hilbert spaces** differ from real spaces but remain well-defined and usable  

When applying **COs** to **QIS**:

- The bounded **N-dimensional space** consists of complex numbers with real and imaginary parts between -1 and 1  
- The origin is at the midpoint, doubling the **N-cube** edge lengths  
- Each point is a true vector from the origin to the point  

Quantum physics introduces the **unitarity constraint**:

- Vectors must be of **unit magnitude**  
- All relevant points lie on an **N-sphere (N-hedron)** with unit radius centered at the origin  
- This reduces system dimensionality from **N to N-1**  

With these parameters:

- The **radius of the N-sphere** is the natural unit of measure  
- As **N** grows, the edge length of the unit **N-cube** shrinks toward zero  
- The standard deviation of distances between random points shrinks toward zero, approaching a constant expected value  

These geometrical statistics are consistent with earlier studied concepts.

## 18 U.S. Air Force Contracts

- **Contract Nos. F30602-02-C-0077** and **F30602-03-C-0051**.

## 19 Table II-1 in Chapter II

- Shows a comparison of **unit edge** versus **unit radius** statistics for the real **unit N-cube**.
- Similar numbers are obtained for **QIS systems**.
- Statistics are analyzed from the perspective of a **unit radius** rather than a unit edge.
- All of the **Correlithm Object (CO)** statistics still hold true with this perspective.

An interesting observation:

- All points in a bounded, **high-dimensional space** tend to be about the same distance apart.
- When points are viewed as vectors, they tend to be **orthogonal**.
- Multiplying two random vectors (inner product) yields a result essentially zero every time.
- Points are nearly at **right angles** to each other through the midpoint of the space.
- Random points can be used as nearly **orthogonal basis vectors**.
- Although only **N orthogonal basis vectors** can be exactly defined, "essentially exact" orthogonality suffices in many cases.
- There is a huge number of such basis vectors available in any given **N-space**.

This opens many issues and opportunities across various fields.

## Future Work

- A book devoted exclusively to **quantum correlithm objects** is expected to be published.

## Ensembles

The **ensemble** is central to **correlithm object technology** (CO technology).

- An ensemble is any group modeled using CO concepts.
- In this book, the focus is on ensembles of **neurons**.
- Each neuron has an axonal or output **pulse rate**, a real number varying between zero and one.
- Only requiring an ensemble where each member independently produces the same bounded variable.
- The collection of these values across **N members** defines a point in a bounded **N-space**.
- Resulting in emergence of **correlithm object statistics**.

### Applications in Neurophysiology

- These ensemble concepts apply to **neurophysiological investigations and modeling**.
- Believed to be the primary mechanism for **representation, manipulation, and storage of information** in neural systems.
- Significant opportunity remains for research in:
  - **Neurodiagnostics**
  - **Neurostimulation**
  - **Prosthetics**

Key questions include:

- Can these principles be established in laboratories?
- Will mysterious neural behavior become better understood?
- Will significant applications be developed? The answer is believed to be **yes**.

### Psychological Implications

- CO technology offers a new tool to model the **human mind**.
- Potential to evaluate psychological issues via CO mechanisms.
- Possibility to test treatment alternatives and methodologies.
- Can it help understand **mind, emotion, and consciousness**? Believed so.

### Other Suitable Ensembles

- The link between **quantum information science** and CO technology is recognized.
- An ensemble of **photons**, using phase angle as the unifying variable, exemplifies this.
- Other particles like **electrons** also possess suitable bounded variables under the right conditions.
- Questions raised:
  - What can CO technology reveal about photon or electron-based systems?
  - Does this help as devices enter quantum-scale effects?
  - Are there applications involving **light, lasers**, or **nanotechnology**? Likely yes.

### DNA and CO Technology

- DNA strings are composed of four-valued or two-bit elements.
- Treating a DNA pattern as a point in an **N-cube** where each dimension has four possible values reveals CO statistics.
- Each type of DNA is represented as a **single point** in this N-cube, representing the organism.
- A living cell is a computer utilizing DNA as its primary program.
- CO technology may provide new possibilities in **DNA and cellular research**.

### Sociology as a Suitable Ensemble

- Consider an ensemble of people.
- Choosing a spectrum of opinion as the ensemble variable.
- Each person holds a particular, possibly time-varying, opinion strength.
- Such groups define a **correlithm object in N-space**.
- People's opinions influence others, creating ongoing mappings of COs in human populations.
- This leads to modeling a kind of **group mind** and making predictions.

## Conclusions

- The book introduces major concepts of **correlithm object technology**.
- The field is vast, with more discoveries emerging the deeper one explores.
- CO technology today is comparable to where **computer science** was in the late **1940s**.
- Technology development is accelerated, and CO may mature sooner.
- Correlithm object technology might represent "**the other computer industry**".
- There are vast differences between information processing in living neural systems and traditional computers.
- The presented theory models **macroscopic information representation, storage, and manipulation** in living neurons based on simple first principles.
- Emergent properties resemble those observed in living organisms.

## 20

We cannot resist pointing out the parallels between this concept and the **“psychohistory”** described by **Isaac Asimov** in his **Foundation Trilogy**.

Simple and elegant. Many people find it very satisfying. And it produces insights and predictions that not only can be tested and verified, but also lead directly to engineering implementations that do new and useful things. 

We think that **correlithm object technology** will usher in a major new direction for information processing systems, namely systems that behave more like living creatures than machines. We call this direction **“the other computer industry.”**

Whether or not we are right that correlithm object technology “explains” how the brain works, the fact remains that it has already produced many new and powerful concepts. **Correlithm object technology** provides a strong theoretical foundation that can be used to make progress in many diverse directions. We have already begun to explore these new worlds. And there is no end to them in sight!

---

## Appendix I Derivation of Unit NCube Distances

### Random Point to Random Point

#### The Problem  
Find analytic expressions for the **mean and standard deviation** of a variable, **Z**, the Cartesian distance between two points selected at random within a **unit N-cube**.

#### The Solution  
While closed-form expressions for the mean and standard deviation of **Z** as a function of **N** remain elusive, we can obtain good expansions.

Note that:

\[
Z^2 = \sum_{i=1}^{N} V_i^2
\]

where the \(V_i\) are copies of random variables \(V\):

\[
V^2 = (X_1 - X_2)^2
\]

with \(X_1\) and \(X_2\) uniform independent initial deviates on \([0,1]\).

We have for all positive integers \(m\):

\[
E[V^m] = \int_0^1 \int_0^1 |x - y|^{m} \, dx \, dy = \frac{1}{(m+1)(m+2)}
\]

In particular,

\[
E[V^2] = \frac{1}{6}
\]

So,

\[
E[Z^2] = N E[V^2] = \frac{N}{6}
\]

and if we knew the mean \(\mu = E[Z]\), the variance would be:

\[
\sigma^2 = E[Z^2] - (E[Z])^2 = \frac{N}{6} - \mu^2
\]

For an approximation to \(E[Z]\), write the ansatz:

\[
Z = \sqrt{N} Y^{1/2}, \quad \text{where} \quad Y = 1 - \frac{c}{N} + \cdots
\]

Then we obtain expansions for \(E[Y^m]\) from the exponential generating function:

\[
E[e^{tV}] = \text{(given by Mathematica® with erfi function)}
\]

Expanding these series yields approximations for \(E[Z]\) and \(\sigma\) as functions of \(N\) with higher-order terms shown. As \(N \to \infty\), the results simplify to:

\[
E[Z] \approx \sqrt{\frac{N}{6}}, \quad \sigma \approx \frac{1}{\sqrt{120}}
\]

---

### Random Point to Midpoint

#### The Problem  
Find analytic expressions for the **mean and standard deviation** of a variable, **Z**, the Cartesian distance between a point selected at random and the **midpoint** within a unit **N-cube**.

#### The Solution  
Derivation proceeds similarly. The expansions for the mean and standard deviation yield:

\[
E[Z] = \frac{\sqrt{N}}{12}, \quad \sigma = \frac{1}{\sqrt{60}}
\]

as \(N \to \infty\).

---

### Random Point to Corner

#### The Problem  
Find analytic expressions for the **mean and standard deviation** of a variable, **Z**, the Cartesian distance between a random point and any **corner** within a unit **N-cube**.

#### The Solution  
Again, the expansions are similar, giving:

\[
E[Z] = \frac{\sqrt{N}}{3}, \quad \sigma = \frac{1}{\sqrt{15}}
\]

as \(N \to \infty\).

---

## Appendix II Correlithm Objects and Neurophysiology

### Overview

**Correlithm Object (CO) technology** provides a framework for understanding the functionality of living **neural systems**.

- The dynamic state of a **neuron** is the time-varying rate of pulses found on its **axon**.
- The state of a group of **N neurons** can be viewed as a point in a **unit N-cube**.
- COs have unique properties and are the primary **tokens** used to represent data in living neural systems.
- Important tokens are stored in the **synapses** as CO **pairings**, capturing the state CO of one group of neurons relative to another.
- These pairings cause the instantiation of a CO in one group to instantiate the corresponding CO in another.
- The model is **Turing complete**, capable of learning from experience, and highly robust.
- It exhibits many properties uncommon in traditional computers but widespread in living information processing systems.

---

### Representing Information

- The **state of a neuron** is its **pulse rate**, a time-varying, continuous, bounded variable.
- Minimum pulse rate: **0 pulses/sec**; maximum: about **200 pulses/sec** in mammalian neurons.
- These bounds can be normalized to the interval **[0,1]**.
- The information in pulses is therefore **real and continuous, not digital**.
- A neuron’s state at time \(t\) is represented as a coordinate on the real line from 0 to 1.
- The state of a group of **N neurons** at time \(t\) is a set of coordinates defining a point in a **unit N-cube**.
- These points are called **Correlithm Objects (COs)**.
- COs are the primary tokens for representing information in living neural systems.

---

### Storing Information

Information is stored as COs in a cell’s **input dendritic tree**.

- Each input cell tries to impose a particular **pulse rate** on the output cell, but not necessarily its own pulse rate.
- There is a learned **functional relationship** visualized as isolated "spots" on photographic film (see **Figure AII-1**).

#### Figure AII-1: Functional relationship in the dendritic tree

- The horizontal axis: **Input Pulse Rate** (min to max).
- The vertical axis: **Output Pulse Rate** (max to min).
- Spots reflect sparse, typically **non-linear** mappings.
- Spot size indicates **synaptic strength**.
- Input cells strongly influence output cells only on or near these spots.
- When the input pulse rate intersects a spot, the corresponding output pulse rate is **“nominated”** with strength proportional to spot size.
- Spots are modeled as 2D Gaussians.

The aggregate of nominations from all input cells determines the output cell's pulse rate:

- Each nominated value, weighted by synaptic strength, combines to produce a **weighted average**.
- The output cell’s current pulse rate is combined with this weighted average and noise to continuously produce a new pulse rate.
- Effectively, the CO in input cells is compared to stored COs and their paired output values to generate the output cell's new pulse rate (see **Figure AII-2**).

#### Figure AII-2: CO storage and retrieval

- Input pulse rates can be any values at the time a CO is stored.
- These are paired with the current output pulse rate to form spot locations.
- When those input pulse rates reoccur, spots nominate the same output values consistently.

---

### A Closer Look

A **conical neuronal membrane**, such as a **tapering dendrite**, acts as a **spatial pulse rate analyzer**.

- Maximum sustainable pulse rate in a tubular neuronal membrane is proportional to the **diameter** of the tube.
- In a conical membrane, pulses travel until the diameter drops below the point that supports that pulse rate.
- Pulses die out where the membrane enters **relax**.
- The dendrite has three regions corresponding to pulse propagation (see **Figure AII-3**):
  - **Conduction region**: pulses fully pass.
  - **Transition region**: pulses begin to attenuate.
  - **Cutoff region**: pulses are blocked.

#### Figure AII-3: Conical membrane maps pulse rate to physical position

- Small diameters support only low pulse rates.
- Large diameters support high pulse rates.
- Mid-range pulse rates die out at intermediate points.

#### Figure AII-4: Regions in conical membrane response

- Left to right: conduction, transition, cutoff regions.
- Pulses travel fully through conduction region.
- Pulses are blocked in cutoff region.
- Transition region shows graded attenuation.

---

### Synaptic Transmission

- A **synapse** transmits effectively only if its input originates in the dendrite's **transition region**.
- Synapses in the **cutoff region** have too little signal to drive pulses.
- Synapses in the **conduction region** have too much signal; no pulses are produced.
- Only synapses in the **small transition region** reliably transmit signals.
- Thus, only a small percentage of all synapses transmit effectively at any given time (see **Figure AII-5**).

#### Figure AII-5: Effective synaptic transmission regions

- Most synapses ineffective in conduction or cutoff regions.
- Only transition region synapses pass signals.

An active synapse stimulates its target membrane to pulse at the rate corresponding to that target membrane's diameter, regardless of the source pulse rate. This means:

- A synapse can **interface any source membrane pulse rate to any target membrane pulse rate** (see **Figure AII-6**).

Revisiting **Figure AII-1**, only synapses in the **spot regions** are conductive:

- Spot location indicates what input rate produces what output rate.
- Spot size shows synaptic strength to impose the output rate.

This dendrite-synapse arrangement allows a cell to store a **CO** comprised of the pulse rates of cells it monitors, paired with the cell's own pulse rate.

## Appendix II Correlithm Objects and Neurophysiology

Later, when the **watched cells** produce a similar **CO**, then the cell takes on a value near the one it paired with the original **CO**. When a set of **target cells** all watch the same source cells, the effect is to causally relate the **CO** in the source cells to a corresponding **CO** in the target cells.

### Figure AII-6: Synapse Rate Mapping

- A **synapse** can carry any source rate to any target rate.
- Here a medium rate in the **source membrane** activates synapses stimulating a low rate region of the **target membrane**.
- The source membrane’s **transition region** determines which synapses are active.
- The connection location to the target membrane determines the produced **pulse rate**.
- **Learning** involves changing the **conductivity** of synapses.
- For a cell to retain the **CO** it currently receives, it increases conductivity of only the currently active synapses.
- This produces spots that pair the incoming **CO** with the current cell pulse rate.
- The learning process is probably mediated chemically.

### Manipulating Information

- **Neurons** manipulate information by mapping **correlithm objects (COs)** to COs.
- A neuron "watches" the **CO** or output of an ensemble of neurons.
- It compares the input **CO** with **COs** stored in its input dendritic tree.
- It generates a new output **pulse rate** based on the comparison.
- If the input **CO** is very close to a stored **CO**, the output pulse rate will be close to the one present when that stored **CO** was captured.
- If several stored **COs** are close, the output pulse rate is a weighted average of corresponding stored outputs.
- If no close **COs**, output pulse rate drifts in a random walk.
- The outputs of a set of cells form a **CO**.
- Each cell watches the **CO** in some input cells and sets its output pulse rate accordingly.
- The aggregate output pulse rates form an output **CO**, mapping input **CO** to output **CO**.
- **CO mapping is Turing complete**, shown in Chapter VI.

### Conclusion

We have presented a model of the information processing characteristics of **living neural systems**.

- Data is represented by **COs** in pulse rates across ensembles of neurons.
- Data is stored by associating **COs** within the dendritic tree interface.
- Data is manipulated by mapping input **COs** to output **COs**.
- This model should be studied, understood, and tested in the field.
- It offers potentially valuable insights into neural information processing.

---

## Glossary

- **actor:** A converter that maps world data to **COs**.
- **Behavior:** The way in which a **CO system** functions or operates.
- **bit content:** See **information content**.
- **bounded N-space:** A space of **N dimensions** where every possible straight line is of finite length.
- **cardinal converter:** A set of rules mapping one-to-one between a **cardinal set** of world objects and a set of same space **COs**.
- **cardinal objects:** Objects with no order or relation except membership.
- **cell:** Storage for the coordinate value of one dimension in a **CO machine**. “Cell” and “dimension” often used interchangeably but a **dimension** has no storage.
- **CO:** See **correlithm object**.
- **CO computing paradigm:** Computation approach representing data as high-dimensional points from **bounded N-spaces** and manipulating data by following known pathways connecting these points.
- **CO field:** Region of space near a **CO**; also called a **zone of influence**.
- **CO machine:** The computing machine implied by the **CO computing paradigm**, used theoretically and practically.
- **CO system:** A computational structure using **COs**.
- **complementary error function:** Integral of the "tails" of the normal distribution (Gaussian).
- **concurrency:** Property allowing more than one process to operate simultaneously.
- **continuous converter:** Rules mapping continuous world variable to a string **CO**.
- **converter:** Means to get world data *into* and *out of* a **CO system**.
- **correlithm:**
  1. (Mathematics) Method for solving problems using similarities to known examples.
  2. (Computing) Algorithm based on characterizing examples.
- **correlithm object:** A point in high-dimensional bounded space.
- **correlithm object machine:** Tuple *(c,r)* where *c* is set of cells and *r* is set of rules.
- **ensemble:** Group of N objects each with a value of a property seen as coordinates in bounded N-space.
- **erfc:** See **Complementary Error Function**.
- **fires:** One iteration of rule execution.
- **full ply / full space:** All dimensions defined in a **CO system**.
- **geometric probability:** Study of probabilities in geometric problems.
- **googol:** One followed by one hundred zeroes.
- **Hamming distance:** Sum of lengths of straight lines between two points in N-space; number of differing bits in binary space.
- **information content:** Number of bits corresponding to event probability, –log₂(p(x)); aka **bit content**.
- **inner product:** Sum of element-wise product of two vectors; aka **scalar product**.
- **ladder:** General model for behaviors in **CO systems** incorporating input, state, and context hierarchically.
- **lobe:** Grouping of rules for hierarchical composition.
- **mechanism:** A **CO program** of rules running on a **CO machine** to obtain particular behavior.
- **midpoint:** Center of a unit N-cube, equally distant from all corners.
- **N-cube:** Cube with N dimensions.
- **N-hedron:** Set of points in bounded N-space mutually about the same distance apart; aka **N-sphere**.
- **N-space:** Space with N dimensions.
- **N-sphere:** Same as **N-hedron**.
- **N-torus:** Torus with N dimensions.
- **name:** Textual label used for referring to a particular space or point.
- **ordinal converter:** Rules mapping an ordered set of world objects to a string **CO**.
- **ordinal objects:** Objects with a particular order (e.g., alphabet).
- **orthogonal basis vectors:** Linearly independent vectors in the same space.
- **pattern CO:** A known point in a space.
- **part:** Re-usable subsystem or set of rules defined as a single object.
- **pattern point:** Known reference or "landmark" point.
- **ply:** A set of dimensions; space or subspace.
- **point:** Coordinates defining a location in a space or subspace; often used interchangeably with **CO**.
- **point capacity:** Number of points randomly selected ensuring no two are closer than distance *d*.
- **probability of proximity:** Probability that two randomly selected points lie no further than distance *d*.
- **pulse rate:** Number of pulses per unit time.
- **quantized converter:** Converter treating world data as having only specific values.
- **random corner:** A corner of an N-cube selected randomly.
- **random point:** A randomly selected point of an N-cube.
- **real unit N-cube:** Unit N-cube where coordinates are from the real numbers.
- **rule:** Directed line segment or coding from source to target soft tokens; performs all data manipulation in **CO computing**.
- **rule execution:** Process by which a rule is evaluated.
- **rule notation:** Formalism for writing rules of a **CO system**.
- **rule operator:** Two-character symbol “>:” or “:<” separating source and target of a rule (arrow points to target).
- **sampled rule:** Rule with points subsampled from another; a lower resolution version.
- **sensor:** Converter mapping world data to **COs**.
- **soft token:** A **CO** representing data.
- **source:** Portion of a rule specifying what must be matched.
- **space:** Set of dimensions hosting all subspaces of a **CO system**; often used interchangeably with subspace.
- **stack machine:** Computational formalism of a state machine with a stack memory (FILO).
- **standard diameter:** Expected distance through midpoint between two random points in same space; twice the **standard radius**.
- **standard distance:** Expected distance between two random points in same space.
- **standard radius:** Expected distance between midpoint and a random point in space.
- **state CO:** Current coordinate values in cells or point in space varying over time.
- **state machine:** Computational formalism whose memory is a current state.
- **state point:** A **state CO**.
- **straddler:** Any **CO system** running on a concurrent computing resource.
- **string:** Short for **string CO**.
- **string CO:** Set of **COs** with a lower-dimensional geometric relationship reflected by mutual distances.
- **subply:** A subset ply with fewer dimensions.
- **subspace:** Space identifiable as a subset of a space with more dimensions.
- **superposition:** Condition where multiple rules execute concurrently in same space; or combined set of **COs** form a single **CO** carrying combined data.
- **swarm:** A type of **straddler** with physically mobile concurrent resource components.
- **swarm mind:** Behavior of a **CO system** running on a swarm.
- **symmetric bounded N-space:** Bounded N-space with origin at its midpoint.
- **Synthorg®:** A **CO system** including converters and **COs**; trademark owned by Lawrence Technologies LLC.
- **target:** Portion of a rule specifying what is to be generated.
- **term:** Element of source or target of a rule specifying a particular point.
- **unit edge:** Normalization of an N-cube with edges of unit length (see **unit radius**).

## 220 Correlithm Object Technology

### Unit N-cube
- **N-cube**: a cube of **N dimensions** with each edge of unit length.

### Unit Radius
- **Unit radius**: a normalization of an **N-cube** where the standard radius has unit length. See **unit edge**.

### World
- **World**: anything considered to be outside—or in the environment—of a **CO system**.

### World Data
- **World data**: data considered to be outside—or in the environment—of a **CO system**.

### Zone of Influence
- **Zone of influence**: the region of space near a **CO**. Also called a **CO field**.

---

## 221 INDEX

### A
- **actor**: 52, 119, 120, 122, 124, 127, 132, 133, 154, 156, 166, 174, 175, 176, 211  
- **analog**: 96, 100, 105, 106, 107, 108, 123, 163, 186  
- **anomaly detection**: 188  
- **Aristotle**: 81  
- **artificial neural network**: 4, 73, 171, 172, 186  
- **attractor**: 77, 148, 150, 151, 152, 161  
- **attractor well**: 77  
- **axon**: 182, 203  

### B
- **bees**: 97, 105, 185  
- **binary**: 22, 58, 112, 135, 137  
- **bit content**: 27, 31, 32, 211  
- **Boolean logic**: 75, 76  
- **brain**: 5, 98, 116, 132, 133, 195  

### C
- **cardinal converter**: 122, 124, 125  
- **chair**: 81, 134, 135  
- **clusters**: 100  
- **CO architecture**: 69, 73  
- **CO computing paradigm**: vii, 37-41, 42, 45, 47, 50, 52, 53, 63, 112, 133, 134, 151, 163, 171, 178, 184, 211, 216  
- **CO field**: 45, 212, 220  
- **CO machine**: 57, 59, 70, 72, 73, 74, 75, 96, 100, 104, 109, 132, 133, 158, 170, 172, 211, 212  
- **coffee**: 106  
- **complement**: 41, 75  
- **complementary error function**: 29, 30, 212  
- **completeness**: 69, 75  
- **complex**: 2, 5, 22, 40, 53, 54, 58, 83, 87, 96, 101, 106, 108, 112, 130, 144, 151, 157, 170, 178, 184, 186, 189, 191  
- **complex numbers**: 40, 112, 191  
- **computing resource**: 5, 97, 99, 105, 118, 190  
- **concurrency**: 99, 100, 103-105, 146, 151, 162, 163, 172, 185, 190, 212  
- **concurrent**: 97, 99, 100, 102, 105, 116, 118, 152, 159  
- **cone**: 46, 93, 94, 112, 206  
- **confidence**: ix, 47, 132, 139  
- **Connectionism**: 73  
- **context free grammars**: 152  
- **Context sensitive grammars**: 152  
- **continuous systems**: 163, 186  
- **converter**: 52, 53, 55, 119-129, 145, 176, 211, 212, 217, 219  
- **Cyrillic**: 90-92  

### D
- **data fusion**: 80, 141, 179, 185, 187  
- **dendrite**: 206-208  
- **dendritic tree**: 7, 204, 209, 210  
- **differential equations**: 106, 169  
- **differentiator**: 169  
- **Digital Signal Processors (DSP)**: 97, 100, 105  
- **DNA**: 193  

### E
- **ebits**: 58  
- **emergent statistical property**: 116  
- **English**: 90-92  
- **environment**: 9, 104, 105, 133, 152, 176, 189, 220  
- **erfc**: 29, 30, 213  
- **error correction**: 137  
- **error detection**: 137, 138  
- **expert system**: 4, 160-162, 186  

### F
- **face**: 79, 82, 83, 91, 139, 174  
- **feedback control systems**: 163, 170  
- **feedback loop**: 53, 152  
- **Field Programmable Gate Array (FPGA)**: 100, 105  
- **finite automata**: 152  
- **flip-flop**: 75, 76, 78, 115, 116, 153, 163, 183  
- **floor**: 65, 165-167, 174  
- **formal cause**: 81  
- **formal grammars**: 48, 145, 152  
- **fovea**: 85, 86  
- **foveal spot**: 85  
- **frequency division multiple access**: 113  
- **function generation**: 142, 143, 150, 151  
- **functional relationships**: 108, 163  
- **fuzzy logic**: 4, 172, 173, 186, 188  

### G
- **generation tree**: 147, 149  
- **geometric probability**: 9, 10, 25, 90, 183, 213  
- **geometrical characteristics**: 81, 82  
- **googol**: 2, 213  
- **grid**: 95, 97  
- **Grid computing**: 100  
- **group mind**: 5, 97, 185, 194  

### H
- **Hamming distance**: 9, 10  
- **hierarchical organization**: 83  
- **hierarchy**: 82-85, 96, 118  
- **Hilbert space**: 112, 191  
- **hologram**: 97, 113-115, 185  
- **holographic**: viii, 5, 97, 183, 185  
- **human brain**: 116  

### I
- **if-then-else system**: 160  
- **immunity**: 115  
- **information content**: 27, 38, 110, 111, 152, 162  
- **inner product**: 19, 192  
- **integer**: 33, 40, 58, 80, 112, 167, 197, 198  
- **integrator**: 169, 170  
- **Internal Control**: 174  
- **intersection**: 41, 139  

### K
- **Klein bottle**: 129  

### L
- **ladder**: 149-152  
- **learning**: 159, 175-177, 190, 203, 209  
- **linear grammars**: 152  
- **lines**: vii, 10, 39, 79, 93, 107, 127, 161, 200  
- **Lobachevsky space**: 130  
- **Logical Completeness**: 71  
- **logical NOT**: 41, 70  

### M
- **major diagonal**: 9, 10, 15, 18  
- **midpoint**: 9, 11, 13-22, 40, 191, 192, 200, 218, 219  
- **mind**: 15, 21, 106, 116, 118, 144, 173, 189, 193  
- **Mind/Body Problem**: 178  
- **Möbius strip**: 129, 130  
- **morph**: 80  

### N
- **N-dimensional tetrahedron**: 21  
- **Neurophysiologists**: 182  
- **N-hedron**: 21, 191, 214, 215  
- **normal distribution**: 26, 29  
- **novelty detector**: 177  
- **N-torus**: 23, 215  

### O
- **operation**: 41, 57-59, 71, 101, 102, 110, 116, 118, 171, 174, 192  
- **ordinality**: 90, 127  
- **orthogonal basis vectors**: 19, 22, 192  
- **overlap**: 40, 113, 159, 184  

### P
- **parse tree**: 145, 146  
- **pattern COs**: 45, 46, 60, 113  
- **pattern generation**: 25, 80-83  
- **pattern recognition**: 4, 25, 83, 142, 150, 151, 181  
- **PatternPoint**: 64  
- **Perceptron**: 73, 74  
- **point capacity**: 27, 32, 216  
- **point of view**: 98, 114, 151  
- **popcorn**: 97  
- **probability of proximity**: 26, 28-33, 216  
- **productions**: 48, 145  
- **pulse rate**: 6, 7, 182, 192, 203-210  
- **Pythagorean theorem**: 2  

### Q
- **quantization**: 167, 168  
- **Quantum Information Science**: 5, 22, 58, 191, 193  
- **qubits**: 58  

### R
- **random corner**: 13, 15-18  
- **rational**: 58  
- **reinforcement learning**: 176, 190  
- **relative location**: 84-86, 91, 92  
- **resolution**: 114, 115, 118, 161, 185  
- **Riemann space**: 130  
- **robot**: 179, 188  
- **robustness**: viii, 97, 115, 163, 184, 185  
- **rule-based systems**:160  

### S
- **sags**: 95  
- **sampled rule**: 109, 110, 217  
- **sampling**: viii, 86, 97, 109-111, 113, 127, 134, 168, 185  
- **sensor**: 52, 119-123, 125, 126, 132, 133, 156, 159, 166, 188, 217  
- **serial**: 99, 105  
- **sets of points**: 79  
- **shooting gallery**: 43  
- **simulation**: 106, 152  
- **speech center**: 116  
- **stack**: 152-156, 158, 159, 217  
- **stack machine**: 152, 154, 217  
- **state CO**: 46, 60, 100, 113, 122-127, 203, 218  
- **State CO**: 218  
- **state machine**: 152, 153-155, 157, 170, 175  
- **statistical landmark**: 3  
- **straddler**: 105, 115, 118, 185, 186, 218  
- **superposed**: 111-113, 127, 152, 159  
- **superposition**: viii, 97, 111-113, 139, 140, 141, 146, 151, 152, 159, 161-163, 172, 185, 219  
- **surfaces**: vii, 79, 107, 127  
- **swarm**: 5, 97, 104, 118, 185  
- **synthetic organism**: 104, 133, 181  
- **synthorg**: 132, 133  

### T
- **Target Cell Independence**: 63  
- **thought processes**: 178  
- **thread**: 99  
- **tools**: 44, 105, 181, 187-190  
- **topology**: 130  
- **tube**: 96, 130, 206  
- **tunnel**: 141, 142  
- **Type Zero grammar**: 152  

### U
- **union**: 41  
- **unitarity constraint**: 191  

### V
- **virtual digital computer**: 115-117  
- **vision center**: 116  
- **volumes**: vii, 79, 80  
- **Von Neumann architecture**: 69  

### W
- **world data**: viii, 52, 55, 119, 120, 127, 145, 211, 212, 217, 220  

### Z
- **zone of influence**: 45, 46, 212, 220