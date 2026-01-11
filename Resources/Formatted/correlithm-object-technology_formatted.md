---
title: correlithm-object-technology
source: correlithm-object-technology.txt
formatted: 2026-01-11T03:34:35.7200776Z
---

## Correlithm Object Technology  
**P. Nick Lawrence, Ph.D.**  
Correlithm Publications  
Dallas, Texas  
A Division of Lawrence Technologies, LLC  

---

### Copyright Notice  
**CORRELITHM OBJECT TECHNOLOGY**  
COPYRIGHT © **2004** by **Lawrence Technologies, LLC**. All rights reserved.  
Printed in the United States of America. No part of this publication may be reproduced or distributed in any form or by any means without prior written permission of the publisher.  
This book is printed on acid-free paper.  
ISBN: **0-9752761-1-5**  
Produced in **Times New Roman**.  
Cover design by **Constance L. Wisber**.  
Printer and binder: Lightning Press, Inc.  
Website: [http://www.LT.com](http://www.LT.com)  

**Dedication:**  
For Monty, who believed, and Connie, who knew.

---

# Table of Contents  

- Preface XI  
- Acknowledgements XIII  
- Chapter I Overview 1  
  - A Short Summary 2  
  - What Good Are CO Systems? 3  
  - How the Brain Works 6  
  - The Problem 6  
  - The Solution 6  
- Chapter II Unit N-Cubes 9  
  - Distances in Unit N-Cubes 9  
  - Summary of Distances 12  
  - Properties of Unit N-Cubes 14  
  - Unfamiliar Geometry 15  
  - Additional Perspectives 20  
- Chapter III Probability and Information 25  
  - Summary 26  
  - Distribution of Distances 27  
  - Probability of Proximity 28  
  - Information Content of Proximity 31  
  - Point Capacity 32  
- Chapter IV Elements of Correlithm Object Computing 37  
  - Spaces 39  
  - Points 41  
  - Soft Tokens 42  
  - Rules 47  
  - Strings 49  
  - Sensors and Actors 52  
  - Systems 53  
  - Miscellaneous 54  
- Chapter V Correlithm Object Machines 57  
  - Definition of a Correlithm Object Machine 59  
  - Building a New State Point 61  
  - Cell Form of Rule 63  
  - Target Cell Independence 63  
  - Dimensions Appearing in Multiple Rules 64  
- Chapter VI Completeness 69  
  - Logical “NOT” 70  
  - Logical Completeness 71  
  - Logical “NAND” 71  
  - No Perceptron Limitations 73  
  - Logical “XOR” 73  
  - Turing Completeness 75  
  - Flip-Flop 75  
- Chapter VII Strings 79  
  - String Concepts 80  
  - Approximate Relative Location 85  
  - Ordinality 90  
  - Higher Dimensions 91  
  - Comments 96  
- Chapter VIII Architecture 97  
  - Serial Implementations 99  
  - Concurrent Implementations 100  
  - Analog Implementations 105  
  - Properties and Capabilities 108  
  - Sampling 109  
  - Superposition 111  
  - Statistical “Holograms” 113  
  - Straddlers 118  
- Chapter IX Converters 119  
  - Cardinal Converters 120  
  - Continuous Converters 122  
  - Ordinal Converters 125  
  - Higher Dimensional Converters 127  
  - Converter Form 128  
- Chapter X Mechanisms of Behavior 131  
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
- Chapter XI Directions 181  
  - Why CO Technology Will Succeed 181  
  - Why CO Technology Was Not Discovered Sooner 182  
  - Summary of Characteristics 184  
  - Early Applications 186  
  - The Future 189  
  - Tools 189  
  - Learning 190  
  - Quantum Information Systems 191  
  - Ensembles 192  
  - Conclusions 194  
- Appendix I Derivation of Unit N-Cube Distances 197  
  - Random Point to Random Point 197  
  - Random Point to Midpoint 200  
  - Random Point to Corner 200  
- Appendix II Correlithm Objects and Neurophysiology 203  
  - Overview 203  
  - Representing Information 203  
  - Storing Information 204  
  - A Closer Look 206  
  - Manipulating Information 209  
  - Conclusion 210  
- Glossary 211  
- Index 221  

---

## Preface  
The goal of this book is to present **Correlithm Object Technology** in a way accessible to a wide audience. College students and advanced high school students have successfully assimilated this material. Anyone with a technical degree should have no trouble.  

Chapters II through X each begin with a general **summary** describing the importance and key results of the chapter, requiring little technical knowledge. Casual readers should read **Chapter I**, the summaries from Chapters II to X, and **Chapter XI**.  

The material is “difficult” only in that the concepts are **unfamiliar**. They involve geometry in more than two or three dimensions—typically about twenty to one hundred dimensions. The interdisciplinary nature draws on **mathematics, computer science, electrical engineering**, and other fields. Most necessary background is provided, rewarding careful and attentive reading.  

The book is structured systematically:  
- **Chapter I and XI** provide summary material.  
- **Chapter II** covers statistics of bounded, high-dimensional spaces.  
- **Chapter III** develops useful formulas and nomenclature.  
- **Chapter IV** introduces the **correlithm object (CO) computing paradigm**.  
- **Chapter V** models the formal computing machine implied by CO computing.  
- **Chapter VI** proves CO systems are **Turing complete** (capable of universal computation).  
- **Chapter VII** extends COs from points to higher-order objects (lines, surfaces, volumes).  
- **Chapter VIII** presents unique properties like robustness, sampling, superposition, and holographic-like characteristics.  
- **Chapter IX** discusses how to interface real-world data with CO systems.  
- **Chapter X** examines unique behaviors in CO systems.  
- **Appendix I** derives key properties of unit N-cubes.  
- **Appendix II** outlines a theory of information representation in living neural systems based on COs.  

Numerous applications have emerged, including **data mining**, **natural resources**, **telecommunications**, **computer security**, and **intelligence**. Development is supported in part by the **United States Air Force** under contracts F30602-02-C-0077 and F30602-03-C-0051.  

Visit [www.LT.com](http://www.LT.com) for more information.

---

## Acknowledgements  
Many thanks to **Warren McCullough**, **Murray Eden**, and **Jerome Lettvin** for stimulating early conversations at **MIT**. Appreciation also to Monty Rial, Dr. Eustace Winn, Steve Clary, Ray Davis, Phil Whisenhunt, Doug Matzke, Ph.D., Chandler Burgess, Ken Loafman, Steve Benno, Katrina Riehl, Zack Loafman, Bryan Aldridge, Mike Lockerd, and Merrill Nunnally for their financial, technical, and editorial support.  

Special thanks to my wife, **Connie**, for her graphic artwork and unwavering support, without which this book and technology might never have seen the light of day.  

Nick Lawrence, April 2004

---

# Chapter I Overview  

**Correlithm object technology** enables building systems that behave more like **living organisms** than traditional machines. It is based on the discovery of remarkable and unexpected **statistical properties** of bounded, high-dimensional geometrical spaces. These systems offer capabilities distinctly different from traditional computing and open new directions for science, engineering, and application development.  

**Correlithm** (cor're-lith'm) is a coined term from about 1975, defined as:   

- **correlithm (n.)**  
  1. (Math) A systematic method for solving problems using **similarities to known examples**.  
  2. (Computing) An algorithm primarily based on the use of **characterizing examples**.  

**correlithmic (adj.)**  

**Correlithm objects (COs)** are the core data tokens in this paradigm, derived from observations of living neural systems. They are believed to be central to all high-level data representation, storage, and manipulation in such systems.  

This book focuses primarily on using COs for engineering information processing systems, not on neuroscience per se. The mathematical nature and utility of COs in **high-dimensional spaces** are the core focus.  

---

## A Short Summary  

- Consider a list of **N real numbers**, each independently selected at random between 0 and 1, interpreted as the coordinates of a random point in a unit **N-cube**.  
- When two points are chosen, their distance is computed using the Pythagorean theorem in N dimensions. For **N ≥ 20**, points randomly selected are almost always about the same distance apart.  
- The **volume** of a unit N-cube is enormous. For instance:  
  - If each coordinate has two possible values and **N=20**, the number of points is over one **million** (2^20).  
  - For **N=50**, there are over one million **billion** points (2^50).  
  - For **N=100**, selecting ten values per coordinate results in 10^100 points, i.e., **one googol**, vastly exceeding the number of particles in the known universe.  
- This vastness means random points are rarely near each other by chance; proximity likely indicates correlation by the same process. This principle underpins CO technology.  
- Points (COs) act as **statistical landmarks** and robust data tokens representing particular processes.  
- Mapping among these tokens forms a general-purpose computational paradigm capable of universal computation and behaviors more like living creatures than machines.  

---

## What Good Are CO Systems?  

- Traditional computers are **precise, fast, but brittle**. Minor faults can cause complete failures. They don’t fail gracefully or handle noisy data well.  
- Living organisms are **imprecise, slower, but highly flexible and robust** to noise and damage. They process complex information quickly and reliably under ambiguity.  
- CO systems exhibit many behaviors of living organisms **emergently**, without explicit design. This strongly suggests CO technology captures the essence of information processing in living creatures.  
- COs add new dimensions to paradigms like expert systems, artificial neural networks, fuzzy logic, and pattern recognition by providing a novel data representation framework.  
- CO systems can be highly **robust and distributed**, analogous to spreading “brain cells” across many computers or robots, producing holographic-like robustness to failures.  
- Potential applications include areas where living creatures outperform traditional computers, e.g., robotics, data mining, legal discovery, medical imaging, natural language processing, vision, handwriting recognition, security, and scientific data analysis.  
- CO technology appears promising in numerous fields where **ensembles** of elements produce correlated states (e.g., neurons in neurophysiology, qubits in quantum information science, individuals in sociological models).  

---

## How the Brain Works  

### The Problem  
- How is information represented, stored, and manipulated in living neural systems?  
- The focus is not on detailed neuron science, but on high-level information processing characteristics of neural ensembles.  

### The Solution  
- A living neuron produces time-varying **pulse rates**, normalized to real numbers between 0 and 1.  
- A neural ensemble’s state at any time is the list of instantaneous pulse rates from all its neurons, viewed as a point in a unit **N-cube**.  
- Changes in the ensemble’s state correspond to movements of this point.  
- Statistically, state points at different times do not lie close together by chance. Proximity usually indicates similarity or the same underlying process with minor variation ("noise").  
- Living systems use these statistical properties to robustly represent information. Each neuron "watches" the ensemble's state point and stores pairs of significant states paired with its output pulse rate, physically stored in the dendritic tree using neuron physiology and correlithm statistics (see Appendix II).  
- Neurons manipulate information by mapping current input state points near stored state points and adopting the paired pulse rate accordingly, adjusted by distance and noise.  
- Ensembles of neurons’ output pulse rates themselves form new state points, creating mappings from one neural ensemble’s state point to another’s.  

The **statistical robustness** and huge number of COs distinguishes CO technology from other computing paradigms.

---

# Chapter II Unit N-Cubes  

Visualizing a cube exceeding **20 dimensions** challenges imagination. Millions of random points within such a cube measure almost the **same distance apart** with high probability, defying intuition. This extreme geometry leads to reliable and uniform distances between random points. The points, called **correlithm objects**, serve as data representatives enabling computational breakthroughs impossible in low-dimensional spaces.  

---

## Distances in Unit N-Cubes  

**Geometric probability** studies probabilities related to geometric configurations. Here, we focus on the probabilities of certain distances in real unit **N-cubes**.  

- A **real unit N-cube** is an N-dimensional cube with edges of length one.  
- A unit N-cube has **2^N corners**.  
- The length of the **major diagonal** (a line between opposite corners) is \(\sqrt{N}\).  
- The distance from the **center (midpoint)** of the cube to any corner is half the diagonal: \(\frac{\sqrt{N}}{2}\).  

The **Hamming distance** between two corners is the cumulative length of the minimum number of edges connecting them...

[Text cuts off here]

## 1

The most recent book on this subject is **D. Klain, G. Rota, Introduction to Geometric Probability, Cambridge University Press, 1997**.

## 10 Correlithm Object Technology

The **Hamming distance** is the number of edges that must be traversed to go from either corner of an **N-cube** to the other. The Hamming distance between two opposite corners is **N**. The number of corners that lie at a Hamming distance of **h** from a particular corner is given by the **binomial coefficients** as the combinations of N things taken h at a time, or **C(N, h)**.

What is the probability that two randomly selected corners of a unit **N-cube** lie at a Hamming distance of **h**? Since there are **2^N** corners and there are **C(N, h)** corners that lie at distance **h** from a given corner, the probability is 

- **C(N, h) / 2^N**.

What is the expected Hamming distance between two randomly selected corners? We know that the mean of the binomial distribution lies at **h = N/2**. Thus, **N/2** is the expected Hamming distance.

**Pentti Kanerva** noted this geometric probability in his book. He also showed that the standard deviation of this expected Hamming distance is about **√N**.

What is the expected **Cartesian distance** between two randomly selected corners? We have already implicitly used the Cartesian distance above, for example to compute the length of the major diagonal. The Cartesian distance is simply the length of a straight line connecting two points, namely the square root of the sum of the squares of the differences between the points’ coordinates in each dimension.

Since we know that the expected Hamming distance between two such corners is **N/2**, and we know each edge has length 1, we can immediately obtain the expected Cartesian distance as 

- **√(N/2)**.

Unless otherwise noted, "**distance**" will mean **Cartesian distance**.

We will need the expected values of three other probabilistic distances in the unit **N-cube**. Suppose we select a point at random from the entire volume of the unit **N-cube**. To do this, we select an...

## 2 More generally, the Hamming distance between two points in a unit N-cube

The **Hamming distance** between two points in a unit **N-cube** can be defined as the sum of the lengths of the straight lines that must be traversed in each dimension to go from one point to the other.

*Reference: Sparse Distributed Memory, Pentti Kanerva, MIT Press 1988, p.18.*

---

## Chapter II Unit N-Cubes

We consider an **independent uniformly distributed random number** between 0.0 and 1.0 inclusive for each dimension. This set of **N random numbers** forms the coordinates of the desired random point, called **p1**.

We can ask:

- What is the expected distance between **p1 and the midpoint**?
- Between **p1 and a randomly selected corner**?
- Between **p1 and another point p2** similarly and independently selected at random?

Exact analytic solutions probably do not exist. Appendix I presents a derivational approach that provides valuable expansions for all three problems. However, the mathematics is not very accessible.

Programs can simulate these processes and gather statistics, which novice programmers can normally complete.

---

### Expected distances and their deviations

- The **expected Cartesian distance** between two points selected at random from the entire unit N-cube is approximately **\( \sqrt{N/6} \)**. We call this distance the **standard distance**.

- The **standard deviation of standard distance** is approximately **\(\sqrt{7/120}\)** or about **0.2415**, independent of **N**.

- The **expected distance** between a point selected at random and the midpoint is approximately **\(\sqrt{N/12}\)**, called the **standard radius**. Its standard deviation is approximately **\(\sqrt{1/60}\)** or about **0.1291**.

- The **expected distance** between a point selected at random and any corner is **\(\sqrt{N/3}\)**, called the **standard diameter**. Its standard deviation is approximately **\(\sqrt{1/15}\)** or about **0.2582**.

---

### Summary of Distances

| **Type**  | **Property**                                 | **Cartesian Distance (Unit Edge)** | **Standard Deviation (Unit Edge)** | **Cartesian Distance (Unit Radius)** | **Standard Deviation (Unit Radius)** |
|-----------|----------------------------------------------|-----------------------------------|------------------------------------|------------------------------------|-------------------------------------|
| Exact     | Edge: length of side                         | 1                                 | 0                                  | ≈ \(1/\sqrt{2N}\)                  | ≈ 0                                |
| Exact     | Major diagonal: maximum corner to corner    | \(\sqrt{N}\)                     | 0                                  | ≈ \(1/\sqrt{2}\)                   | ≈ 0                                |
| Prob      | Corner distance: random corner to random corner | ≈ \(\sqrt{N/2}\)                  | ≈ \(\sqrt{1/9}\)                   | ≈ 6/\(4\sqrt{3N}\)                 | ≈ 4/(3N)                          |
| Prob      | Standard diameter: random corner to random point | ≈ \(\sqrt{N/3}\)                 | ≈ \(\sqrt{1/15}\)                  | ≈ 4/\(\sqrt{5N}\)                 | ≈ 4/(5N)                          |
| Exact     | Half diagonal: midpoint to random corner    | \(\sqrt{N}/2\)                   | 0                                  | ≈ 3/\(\sqrt{N}\)                   | ≈ 0                                |
| Prob      | Standard distance: random point to random point | ≈ \(\sqrt{N/6}\)                 | ≈ \(\sqrt{7/120}\)                 | ≈ 7/\(\sqrt{10N}\)                | ≈ 7/(10N)                         |
| Prob      | Standard radius: midpoint to random point   | ≈ \(\sqrt{N/12}\)                | ≈ \(\sqrt{1/60}\)                  | 1                                  | ≈ 1/\(\sqrt{5N}\)                  |

*Table II-1 Summary of Statistics for an N-Cube.*

---

### Properties of Unit N-Cubes

A real **unit N-cube** (or any bounded N-space) has unfamiliar properties beyond three or four dimensions.

- Approximations used are valid for about **N ≥ 20**.
- For instance, in a unit 35-cube, random points are about the same distance apart most of the time.
- The ratio of the **standard distance to its standard deviation** is about **\(20\sqrt{N}/7\)**.
- As **N** grows, almost any two randomly chosen points in the **unit N-cube** are essentially the **same distance apart**.
- Only **N+1 points** can be arranged so that each point is exactly the same distance from all others (e.g., equilateral triangle in 2D).
- But the **statistical conclusion** is that all randomly selected points are approximately the same distance apart in high dimensions.

---

### Unfamiliar Geometry

Define the following points within a unit **N-cube**:

- **m**: the midpoint
- **p, q**: random points
- **c, d**: random corners
- **o**: corner opposite **c**

Figure II-1 (schematic):

- Various right triangles such as **cdo, cmd, cmp, pmq** have legs and hypotenuses related by the dimensions of the cube.
- All such triangles have notable properties of right angles, e.g.:

  - Triangle **cdo** is right angled with major diagonal length \(\sqrt{N}\).
  - Triangle **cmd** is right angled with hypotenuse \(\sqrt{N/2}\).
  - Triangle **cmp** is right angled with hypotenuse approx \(\sqrt{N/3}\).
  - Triangle **pmq** right angled with hypotenuse approx \(\sqrt{N/6}\).

- Unit Radius normalization simplifies these results.

---

### Structural view of the N-cube

Figure II-2 illustrates:

- From corner **c**, opposite corner **o** forms "poles" on an N-sphere.
- Other corners lie on the "equator" of this sphere.
- Random points lie roughly on another N-sphere.
- All corners appear equally distant from each other and midpoint.
- Random points appear to lie in a ring perpendicular to the major diagonal (line between c and o).
- These views strongly emphasize the **statistical nature** of point distribution in the **N-cube**.

---

### Orthogonality of random points

Using vector analysis:

- Shifting origin to the midpoint **m**:
- Inner product of vectors between points \(p\) and \(q\), **p·q**, is approximately 0.
- Thus, vectors from the midpoint to random points are approximately **orthogonal**.
- This provides a large pool of nearly **orthogonal basis vectors** useful in applications.

---

### Additional Perspectives

#### Random point perspective (Figure II-3)

- A randomly selected point is surrounded by two spheres:
  - Inner sphere: volume sampled by random points.
  - Outer sphere: corners.
- Radii represent expected distances.
- Thicknesses represent one standard deviation.
- From any random point, all others appear roughly the same distance away.

#### Midpoint perspective (Figure II-4)

- Midpoint surrounded similarly by:
  - Inner sphere (random points)
  - Outer sphere (corners, exact distance)
- Placing origin at midpoint facilitates use of random points as orthogonal basis vectors.
- Similar properties apply for other types of bounded N-spaces (e.g., N-spheres, binary N-cubes, complex unitary N-spheres).

---

### Related geometrical forms: N-torus

- An **N-torus** is an N-cube whose dimensions are **looped around** modulo the dimension extent.
- For example, value 0.99 is distance 0.02 from 0.01 in that dimension.
- Provides additional interesting geometry.

---

### Key conclusion

- Intuition from 2D or 3D shapes does not apply well to unit N-cubes at high dimensions.
- Volume grows rapidly with dimensions.
- Remarkable and unexpected statistics emerge.
- These notions are central to **correlithm object technology**.

---

## Chapter III Probability and Information

- For **dimensions > 20** (preferably 50-100), an enormous number of randomly chosen points can be packed into a unit N-cube, all nearly the same distance apart.
- Points that are **abnormally close** carry significant **information**.
- This can be exploited for **error correction**, **pattern recognition**, and **pattern generation**.
- This chapter explores geometric probabilities of unit N-cubes and relates them to information theory, establishing a foundation for correlithm object computing.

---

### Summary of important relationships

| Property                                        | Equation                    | Description                                                                                 |
|------------------------------------------------|----------------------------|---------------------------------------------------------------------------------------------|
| Distribution of Point-to-Point Distances        | \(\text{normal}(d)\) (III-A) | Normal distribution of distances between points in unit N-cube                             |
| Probability of Proximity                         | \( \approx 2.47e^{-7.45(\frac{d-\mu}{\sigma})^2} \) (III-J)  | Probability two points have distance less than \(d\)                                       |
| Bit Content of Proximity                         | \( \approx \frac{\mu - d}{\sigma^2} \times 6 \) bits (III-I) | Approximate information content related to proximity                                       |
| Point Capacity                                   | \( c \approx 3.7 \times 10^{(d-\mu)^2} \) (III-Q)          | Number of points before pair distances less than \(d\) are likely                           |

- Here, \(N\) is the number of dimensions, \(\mu \approx \sqrt{N/6}\), \(\sigma \approx 0.2415\).

---

### Distribution of Distances

- Distances between pairs of random points in a unit N-cube follow a **normal distribution**:

\[
\text{normal}(d) = \frac{1}{\sigma \sqrt{2\pi}} e^{-\frac{1}{2} \left(\frac{d - \mu}{\sigma}\right)^2}
\]

- \(d\): distance between points, \(0 \leq d \leq \sqrt{N}\)
- \(\mu \approx \sqrt{N/6}\), \(\sigma \approx \sqrt{7/120} \approx 0.2415\)
  
(Figure III-1 illustrates Gaussian distributions for various \(N\))

---

### Probability of Proximity

- Probability that two points \(p_1, p_2\) lie no further than distance \(d\):

\[
\text{prob}(\text{dist}(p_1,p_2) < d) = \int_0^d \text{normal}(x) dx
\]

- Expressed using the **complementary error function (erfc)**:

\[
\text{erfc}(x) = \frac{2}{\sqrt{\pi}} \int_x^\infty e^{-t^2} dt
\]

- Approximation valid for certain \(x\) values leads to:

\[
\text{prob}(\text{dist}(p_1, p_2) < d) \approx \frac{\sigma}{\sqrt{2\pi}(\mu - d)} e^{-\frac{(\mu - d)^2}{2 \sigma^2}}
\]

---

### Information Content of Proximity (Bit Content)

- Information content \( \text{BitContent}(d) = -\log_2(\text{prob}(\text{dist} < d)) \).
- Approximate expression:

\[
\text{BitContent}(d) \approx \frac{(\mu - d)^2}{2 \sigma^2 \ln 2}
\]

- Rule of thumb: **4.122 bits per dimension** as \(d \to 0\).

---

### Point Capacity

- If \(c\) is the number of points picked, then total number of point pairs is \(c(c-1)/2\).
- Probability that **all pairs** are further apart than \(d_0\):

\[
p = (1 - a)^k
\]

where:

- \(a = \text{prob}(\text{dist} < d_0) \approx \frac{\sigma}{\sqrt{2\pi}(\mu - d_0)} e^{-\frac{(\mu - d_0)^2}{2 \sigma^2}}\),
- \(k = c(c-1)/2 \approx c^2/2\) for large \(c\).

- Approximating for large \(c\), solving for \(c\):

\[
c \approx \sqrt{\frac{2 \ln p}{\ln(1 - a)}} \approx \sqrt{\frac{2 \ln p}{-a}} \propto 10^{(d_0-\mu)^2}
\]

- Meaning: a **huge** number of points can be selected with near certainty that no two will be closer than \(d_0\).

---

### Practical examples

- For \(N=96, \mu \approx 4\), \(d=1.0\), \(p=0.01\):

\[
c \approx 1.9 \times 10^{33}
\]

- Implies we can select nearly \(10^{33}\) points randomly in this unit 96-cube all at least 1.0 apart with high probability.

- Rule of thumb: never use fewer than **20 dimensions** for correlithm objects, preferably **50 or 100**.

---

## Chapter IV Elements of Correlithm Object Computing

- The **correlithm object (CO) system** is flexible, capable of operating well in ambiguous or imperfect data situations.

- Traditional computers are precise but struggle off their "roads" (analogous to a standard car).

- CO systems work well on and off "road" (like an SUV).

- **CO systems** handle ambiguity and adapt behavior based on similarity judgments, mimicking human **deduction and induction**.

---

### Two perspectives on a point

- A point in space can be considered:

  1. A **single object** or monolithic entity.
  
  2. A **set of coordinates** (list of numbers) relative to a coordinate system.

- Both perspectives are useful depending on the context.

---

### Figure IV-1 Illustration (schematic)

- Shows example coordinates **X, Y, Z** and a **Point**.

---

*End of formatted content.*

## Figure IV-1 The same point viewed in object form and in coordinate form

We will employ both of these **perspectives** at various times. The **“point” perspective** turns out to be more of a **“software” perspective**. It focuses attention on what a **CO computing system** is accomplishing.

In a sense, the **“point” perspective** is at a higher level of abstraction than the **“coordinate” perspective**, which is more of a **“hardware” perspective** that tends to focus attention on how a **CO computing system** in fact accomplishes what it does.

The **CO computing paradigm** represents data using **high-dimensional points** that are typically selected at random in a bounded **N-space** and manipulates data by following known pathways that connect these points. A critical characteristic of the **CO paradigm** is its recognition that the **probability** or **information content** of data is as important as the data itself. The **CO computing paradigm** represents **probability** or **information content** by **proximity to known points**.

**Point = (X, Y, Z) = (0.5, 0.4, 0.6)**

---

## Chapter IV Elements of Correlithm Object Computing

### Spaces

In its most general form, the **CO computing paradigm** is defined within a bounded **N-space**, which is simply a space of **N dimensions** where every possible straight line is of finite length. Here we will limit our attention to the particular bounded **N-space** called the **real unit N-cube**, an **N-dimensional cube** with an edge length of one. (See Figure IV-2.)

---

### Figure IV-2 Visualization of the N-dimensional unit cube

- The **origin** is at the lower left corner.
- The light gray lines radiating from the corner suggest the **N dimensions** that meet at each corner.
- Two adjacent corners are shown with **unit length** between them.
- The corner shown at the top of the figure is the opposite corner to the **origin**.
- The gray perimeter lines and cloudy background suggest the **high-dimensional cubic shape**.
- A randomly selected interior point is also shown.

---

For present purposes, we require that all coordinates of all points are members of the set of **real numbers between 0 and 1 inclusive**, and we place the **origin** at one corner of the **N-cube**.

Other choices for the type and range of numbers used to define the bounded **N-space** and for the location of the **origin** are also important in some forms of **CO computing**. One important choice is to place the **origin** at the midpoint of the **N-cube**. Other important choices are to employ **integers**, **complex numbers**, or even more exotic number types. We will not pursue such alternative choices here.

---

We focus on the **CO computing paradigm** in a **real unit N-cube**.

- There are **N dimensions** available.
- We often use **“dimension”** and **cell** interchangeably.
- A particular position within a dimension is called a **coordinate**.
- Its value must be a **real number** no less than zero and no greater than one.

---

A **space** is any set of **dimensions** and a **subspace** is any subset of a space. Similarly, a **ply** is any set of **cells** and a **subply** is any subset of cells.

- The **full space** is the set of all available **dimensions**.
- The **full ply** is the set of all available **cells**.

Often it is clear from context that a particular set of dimensions or cells is a subset of another set of dimensions or cells, and we omit the **“sub”** prefix.

---

We give names to **spaces** as we define them. These names may be anything but often follow the common programming rules for **identifiers**.

We select a **space** by choosing a specified number of **dimensions** from the **full space**. For simplicity, all of the **spaces** we choose here will be **disjoint**, meaning they have no dimensions in common, unless otherwise stated.

---

Normally, we select a **space** by choosing a specified number of **dimensions** at random from the **full space**. This method can of course result in **overlap** (dimensions in common) between two or more spaces.

Although it may appear otherwise, **overlap** is usually not a problem. In fact, we will see later that **overlap** can be a useful property.

When working with the **CO computing paradigm**, one should normally resist the urge to use non-random methods, because many of this paradigm’s primary properties emerge from **stochastic processes**. One should only use deterministic methods when it is very clear why **stochastic methods** are wrong for a particular case.

---

### Set-theoretic relationships among plies

We will sometimes need to establish **set-theoretic relationships** among various **plies**. We can define a **ply** as the **intersection** or **union** of (the dimensions of) two other plies.

Example:

- Define **ply r** as the **intersection** of plies **p** and **q**.
- Define **ply r** as the **union** of plies **p** and **q**.

The **complement** or **logical NOT** of a ply is all defined dimensions that are not found in the ply.

For example:

- Given plies **p** and **q**, we can define **ply r** as the **“complement of q with respect to p”**, or as the **“remainder of p minus q”**, or simply as **“p and not q.”**

Thus **r** would be all of the dimensions of **p** that are not in **q**.

---

### Points

The most fundamental operation in the **CO computing paradigm** is the definition of a **point**. We often use **“point”** and **CO (Correlithm Object)** interchangeably.

To define a **point**, we specify:

1. The particular **space** that will contain the point.
2. The **coordinates** of the point within that space.

Just as a space can be selected randomly or deterministically, so can the **coordinates** of a point. We usually select **coordinates randomly**, but note that there are important exceptions to this that will come up later.

---

We give names to points as we select them. As with **spaces**, these names may be anything but often follow the common programming rules for **identifiers**.

Since we can define many points within a given space, it is convenient to use a notation that gives both the **name of the space** and the **name of the point** within that space.

---

For example, see Figure IV-3. Suppose we have two spaces (**plies**) named **p** and **q**.

- Identify two points in **p** named **c** and **d**.
- Identify one point in **q** named **c**.
- Refer to these three points as **p.c**, **p.d**, and **q.c** respectively.

**Point q.c** is a different point than **point p.c** because they are in different **spaces**. The identification of a ...

## 4 Example of Subspaces or Subplies

This is an example of a case where it is clear from context that we mean **subspaces** or **subplies**, and the “sub” prefix is omitted.

**Particular point** requires both the name of the space and the name of the point within that space.

### Figure IV-3: Full Ply with Subplies

- **Full Ply**
- **Ply p**
  - Points: p.c, p.d
- **Ply q**
  - Point: q.c

Three points are shown: **p.c** and **p.d** in ply **p**, and **q.c** in ply **q**.

## Soft Tokens

Points in spaces (**COs** in plies) are used to represent all data in the **CO computing paradigm**. Associating a **CO** with data is a very fundamental notion. It is the sole means available for representing data.

Far from being limiting, this one-data-type idea supports a wealth of new computing capabilities. It may help to think of a **CO** as analogous to a **storage location** in a traditional computer, with the associated data as the contents of the location.

This analogy can be misleading, however, because the **CO** does **not store** the data; it **represents** the data.

A **CO** that is representing data is sometimes called a **soft token**.

### Proximity of COs

As seen in **Chapter III**, it is very unlikely that two **COs** randomly selected in the same ply will be in close proximity to each other. If two COs are close, they are almost certainly **not randomly selected**. It is more likely some **non-random process** produced both COs, with noise accounting for small variations in position.

### Analogy of a Shooting Gallery

Suppose we are in a shooting gallery, and all we can see is the target. There are two shooters, one **amateur** and one **expert**. A shot hits near the bullseye (see Figure IV-4). Can we guess which shooter took the shot?

- We cannot be certain, but it seems much more likely the **expert** fired the shot.

Similarly, suppose in ply **p** we see CO **p.c**. There are two processes: one picks COs at random in **p**, and one deterministically picks **p.c** but with some noise.

- The pick **p.c’** appears very near to **p.c** (see Figure IV-5).
- Which process made the pick?

### Figure IV-5: CO p.c and p.c’ Close Together

- **Ply p**
- Points: p.c, p.c’

We use tools from **Chapter III** to analyze proximity quantitatively:

- Let number of dimensions in p be **100**.
- Distance between **p.c** and **p.c’** is **0.5**.
- Expected distance, µ, between random points in p is about \( \sqrt{100} \approx 4.08 \).

Probability of random proximity ≤ 0.5 is given by:

\[
\text{prob}(\text{dist}(p.c, p.c') \leq 0.5) \approx 10^{-10}
\]

The probability that **p.c’** came from the random process is extremely close to zero.

### Bayesian-Like Probability Approach

There may be a third, unknown source for **p.c’**, but without further information, this can be discounted.

The CO paradigm uses a **Bayesian-like approach**: given exactly two possible sources, calculate probabilities for each being the source.

### Zone of Influence

Random COs will rarely appear near a particular CO **p.c**.

- A CO **p.c’** in the vicinity of **p.c** is likely a “noisy version” of **p.c**, produced by the same process.
- The **distance** between points determines the probability they represent the same data.

This defines a **zone of influence** around a CO (see Figure IV-6):

- Any CO found near **p.c** likely represents the **same data**.
- This zone forms a **CO field**, explaining the term **soft token**.
- A CO represents particular data not only at its exact coordinates but partially at nearby coordinates.
- The **information relevance falls quadratically** with distance (see Equation III-I).

### Known Points as Pattern COs

- Known points in space are called **pattern COs**.
- These serve as **landmark-like positions** and centers of known soft tokens.
- A **state CO** is time-varying and travels in the ply, representing instantaneous cell values.

### Figure IV-6: Zone of Influence Around a CO

- N-space as a 2D mottled plain.
- Small sphere is the CO.
- Light-colored rod is standard distance.
- Dark rods spaced at one standard deviation.
- Cone shows quadratic growth in bits of information as approaching the CO.

### Properties of Soft Tokens

- Data representation is no longer sharp-edged or brittle.
- **Pattern COs** co-opt surrounding regions to represent data.
- Defines an **information field** inversely proportional to the square of distance, like a **gravitational field**.
- Data representations become **approximate**, **robust**, **unambiguous**, and in **shades of gray**.
- CO data is inherently **soft** and **resilient** instead of hard-edged.

### Impact on Computation Model

Using **soft tokens** changes computation from:

- Brittle, exact, unforgiving → flexible, accommodating, forgiving.
- A CO **represents a data object**.
- Its field defines its **territory**.
- Any CO in its territory likely stands for the same data object.
- Softness adds **generality**: exact data can be represented, but an information field remains to communicate confidence.

## Rules

A **rule** is a directed line segment from a set of **soft tokens** called the **source** to a set of soft tokens called the **target**.

- Rules perform **all data manipulation** in the CO computing paradigm.

### Example of Rule Notation

Suppose two COs: **s.c** and **t.d** (see Figure IV-7).

- **s.c** represents CO **c** in ply **s**.
- **t.d** represents CO **d** in ply **t**.
- Both **s** and **t** have enough dimensions for CO stability.
- Points **c** and **d** are selected at random within their respective plies.

### Rule IV-1

\[
s.c >: t.d
\]

## 5 The Soft Tokens of the CO Computing Paradigm and the “Fuzzy Logic” of Zadeh

**Soft tokens** of the **CO computing paradigm** and the **“Fuzzy Logic”** of **Zadeh** have similarities.  
Reference: **Zadeh, L.A., "Fuzzy sets," Information and Control, Vol. 8, pp. 338-353, 1965**, and subsequent literature.  

**Soft tokens** can be used to implement **Fuzzy Logic**. However, soft tokens form the basis for the **CO computing paradigm**, which is a complete computational model with significant new capabilities.

---

## 48 Correlithm Object Technology

### Rule Association and Notation

- **Space s** and **Space t** are considered.
- A rule associates a point **s.c** in a source space to point **t.d** in a target space.

**Figure IV-7**:  
Here is a rule that associates point **s.c** in a source space to point **t.d** in a target space.

The operator **“>:”** is the **rule operator**, separating terms **s.c** (source) and **t.d** (target).  
- Terms on the **angle bracket side** are collectively called the **source** of the rule.  
- Terms on the **colon side** are collectively called the rule’s **target**.

The statement in **Rule IV-2** is equivalent to the one in **Rule IV-1**.

**Rule IV-2**  
`t.d :< s.c`

Some prefer rules left-to-right (**Rule IV-1**) and some right-to-left (**Rule IV-2**), originating from formal grammar productions. The **left-to-right** form **Rule IV-1** is used here.

---

### Rule Execution

**Rule IV-1** instructs to watch the region around **s.c**. If a point **s.c'** appears near **s.c**, generate a new point **t.d'** near **t.d**.

**Figure IV-8**:  
The rule that maps point **s.c** to point **t.d** causes **s.c'** near **s.c** to map to **t.d'** near **t.d**.

**Rule execution** refers to how this mapping is done, discussed later.

---

### Mapping

---

### Strings

- A **set of COs** in a ply can be defined using subscripts, e.g., **p.ci, i=1..k** or **p.di,j, i=1..k, j=1..m**.
- A set of **COs with subscripts** is called a **string CO** or simply a **string**.
- **Subscripts** establish ordering or geometric relationships:
  - One subscript: **one-dimensional ordering** (string fits well).
  - Two or more: higher dimension; still called strings despite geometric misfit.

Considering **p.ci, i=1..k** where distance between **p.ci** and **p.ci+1** is less than standard distance and does not affect distances to other COs like **p.q**. This means ordering by subscript reflects adjacency by distance.

**Figure IV-9**:  
Distance from **p.ci** to other members of the string CO, showing distance in N-space increasing to standard distance as sequence distance grows.

---

### Usage of String COs

- Used to embed **lower dimensional** sampled geometries into **higher dimensional** subspaces of the CO computing paradigm.
- **1-dimensional string CO** often captures the **time axis** in series of samples.
- **2-dimensional string CO** captures proximity in images.

---

### Rule IV-3

```
p.ci >: q.d, i=1..k
```

This is a set of **k rules** mapping every CO **ci** in **p** to the single CO **d** in **q**.

**Figure IV-10**:  
Associating CO elements of a string CO in space **p** with an equivalent number of CO elements in space **q**.

This lets single point **q.d** stand for the entire string CO. A string can also be named by a single CO as in **Rule IV-3**.

- Adjacent COs in a string CO are usually closer than standard distance, but it can still be a string CO even if farther apart.

---

## Sensors and Actors

- A system in the **CO computing paradigm** represents all internal data using **COs**.
- Data manipulation is performed using **rules**.
- To convert between **world data** and **COs**, use **converters**:
  - **Sensors:** special rules with **world data sources** and **CO targets**.
  - **Actors:** special rules with **CO sources** and **world data targets**.
- Converter types:
  - **Cardinal, ordinal, continuous converters**
  - **Quantized converters** and rules that **error-correct** world data
  - **Interpolating converters** that handle values between known points.
  
Converters are essential for data input/output to/from a CO system. They are an extensive subject to be revisited later.

---

## Chapter IV Elements of Correlithm Object Computing

---

## Systems

- A **CO system** is a **set of rules**, including converter rules and internal rules mapping COs to COs.
- Programming involves defining rules and their elements.
- Elements often have default values, requiring sufficient size and stochastic behavior.
- Advanced topics include building CO systems that deviate from defaults.
- **Rules are chained** to build programs.

Example **Rule IV-4**:

```
p.c >: q.d  
q.d >: r.e
```

- If the first rule fires, a point near **d** in **q** arises, causing the second rule to fire, creating a point near **e** in **r**.
- Rules can have multiple terms; for example:

**Rule IV-5**

```
p1.a p2.b p3.c >: q1.d q2.e q3.f
```

**Rule IV-6**

```
p1.a p2.bi >: p2.bi+1, i=1..k
```

- Feedback loops are common in rule sets.

---

## Miscellaneous

- Complex CO systems use **re-usable subsystems** called **parts**.
- A **part** is a set of rules defined as a single object, which can be instantiated multiple times and “wired into” other parts.
- Parts can be **hierarchically composed**.

### Part Syntax

```
<part_name> { (one or more rules or parts) }
<instantiation_name> = <part_name>
```

- The `<instantiation_name>` allows access to terms inside the part externally.

### Examples:

```
part1 { s1.p1 >: s2.p2 }

part2 { s1.p1 >: s3.p3 }

part3 {
  pt1 = part1
  pt2 = part2
  s1.p1 >: pt1.s1.p1
  pt2.s3.p3 >: s2.p2
}
```

- **part1** and **part2** define parts with one rule each.
- **part3** instantiates **pt1** and **pt2** as **part1** and **part2**.
- Points like **pt1.s1.p1** reference unique points inside parts.
- Points like **s2.p2** are unique in any instantiation.

---

- A **lobe** is usually a part **without converter rules**.
- A **Synthorg®** is a complete CO system, especially one that only accepts or produces **world data** with internally hidden COs.

---

## Chapter V Correlithm Object Machines

---

### Overview

- The **computing machine model** described is analogous to the **machine language level** in traditional computers.
- It provides a **formal description of a CO computing system**, useful both theoretically and for engineering real systems.
- Implementations have been built and tested successfully.

---

### CO Machine Model

- The model is at the level roughly analogous to machine language.

**Figure V-1: Basic CO machine operation**

- Rule:  
  `s1.p1 >: s2.p2`
- Maps **state point s1.p(t)** to **s2.p(t+1)**.

---

### State Representation

- The **state** is carried by a single time-varying **N-dimensional point P(t)**.
- P(t) is specified as a set of N coordinates in a **real unit N-cube** (bounded space **S**).
- Subsets of these coordinates form state points **p(t)** defined in **subspaces s**, e.g., **s.p(t)**.

---

### Rules and Operation

- A **rule** maps the **current state point s1.p(t)** in the source subspace **s1** to the **next state point s2.p(t+1)** in the target subspace **s2**.
- A rule holds two **time-invariant points**, called **pattern points**:  
  - **s1.p1** in source subspace  
  - **s2.p2** in target subspace
- The rule “watches” the source state point relative to **s1.p1** and controls the target state's movement relative to **s2.p2**.
- It moves the target point so that distance **d2** to the target pattern point reflects current distance **d1** between source state point and source pattern point.

---

### Cells

- A **cell** stores one coordinate of **P(t)** at time **t**.
- It acts like a memory location in a traditional computer that stores one real number in range **0.0..1.0**.

---

### Rule Execution in the Machine

- Each rule is a **self-contained, independent object** acting on cells without depending on other rules.
- The machine is **clocked**; each rule executes once per clock cycle.
- During a cycle, a rule examines its source subspace cells, compares the state point to its source pattern point, and generates the next state accordingly.

---

## 6 Real Numbers in CO Computing Machines

**Real numbers** are used in this particular **CO (Correlithm Object) computing machine**. Almost any type of number may be used if applied consistently, including:

- **Binary**
- **Integer**
- **Rational**
- **Complex**
- Exotic forms such as **qubits** and **ebits** from **Quantum Information Science (QIS)**

Some types have unique advantages.

---

## Chapter V Correlithm Object Machines

A **rule** sets the **next time step values of cells** in its target from the **current time step values of cells** in its source. This apparently requires the existence of two cell arrays:

- A **current cell array**
- A **next cell array**

This is to prevent later-executing rules within one cycle from using results produced by earlier-executing rules within the same cycle.

The correct overall behavior of a **CO machine** is often remarkably insensitive to the exact details of its operation.

- Two cell arrays are not usually necessary
- **Incremental update methods** exist
- All rules do not have to fire within a given cycle
- Rules can both read and write the same cells
- Rules can fire with little regard to the time of firing of other rules, provided certain conditions are met

These are advanced topics not covered here. Instead, we assume:

- There are two cell arrays
- Rules read from the **current array** and write to the **next array**
- All cells execute exactly once in every machine cycle

Shortly, we will consider what happens when two rules try to set values of the same cells in the next array. This results in a valuable strength of a **CO machine**.

---

## Definition of a Correlithm Object Machine

A **correlithm object machine** is formally defined as a tuple **(c, r)**, where:

- **c** is a set of **n cells**, \( \{ c_i \}, i = 1..n \)
- **r** is a set of **k rules**, \( \{ r_j \}, j = 1..k \)  
(See Figure V-2)

Each cell has:

- A unique address, **i**
- A time-varying state, \( c_i(t) \), where \( 0.0 \leq c_i(t) \leq 1.0 \)

A cell corresponds to one unique dimension of a bounded **N-space**. The **unit N-cube** is used here as the bounded N-space, but other bounded N-spaces could be used.

---

### Figure V-2 The Correlithm Object Machine

- All rules are executed **in parallel** at every time step.
- A rule reads a **state point** from a **source subset** of cells (e.g., \( s_1 \)) at time \( t \) and writes a state point to a **target subset** of cells (e.g., \( s_2 \)) at time \( t+1 \).
- A rule writes a **randomly selected state point** in the target whose proximity to the **target pattern point** \( s_2.p_2 \) reflects the proximity of the **source state point** to the **source pattern point** \( s_1.p_1 \).

---

## Cell Subsets and State CO

A **ply, p**, is set of **M cell (or dimension) addresses**, \( \{ a_m \}, m = 1..M \).

- The set of values in the particular cells referenced by \( a \) at time \( t \), \( \{ c_i(t) \}_{i \in a} \), is called the **state CO** for ply \( p \) at time \( t \).
- **Pattern COs** are time-invariant points.

---

## Rules and Their Syntax

Rules store **pattern COs** internally and relate those pattern COs to **state COs**.

The simplest rule syntax is:

**s1.p1 >: s2.p2**

This means:

- "If you see that the **state point** at the current time \( t \) in ply **s1**, \( s_1.p(t) \), is ‘near’ the **ply s1 pattern point**, \( s_1.p_1 \), then build a new **state point**, \( s_2.p(t+1) \), for ply **s2** that is 'similarly near' the **pattern point** \( s_2.p_2 \) in ply s2."

To do this:

- Find the distance, \( d_1 \), that \( s_1 \) lies from \( s_1.p_1 \)
- Choose a new state point, \( s_2.p(t+1) \), based on pattern point \( s_2.p_2 \) and that distance

---

## Building a New State Point

There are various ways to use \( s_2.p_2 \) and the distance \( d_1 \) to choose the new state point:

- Choose \( s_2.p(t+1) \) by randomly selecting a point in \( s_2 \) that lies at distance \( d_1 \) from \( s_2.p_2 \).
- Express \( d_1 \) in units of **standard deviation** and use that in \( s_2 \). This maintains distance statistics in \( s_2 \) even if \( s_2 \) has a different number of dimensions than \( s_1 \).
- Maintain **f**, the fraction of standard distance in \( s_1 \) represented by \( d_1 \), by picking randomly a point in \( s_2 \) with the same fraction of standard distance from \( s_2.p_2 \).

Many other methods are possible. To choose a method, we review some basic principles of **CO systems**.

---

## Interpolation Form of Rule

The distance \( d_1 \) between \( s_1.p(t) \) and \( s_1.p_1 \) in ply \( s_1 \) of \( D_1 \) dimensions is the square root of the sum of the squares of the individual cell differences:

**Equation V-A**

\[
d_1 = \sqrt{\sum_{i=1}^{D_1} \left( s_1.p_i(t) - s_1.p_{1i} \right)^2 }
\]

Similarly, the distance \( d_2 \) between the desired point \( s_2.p(t+1) \) and \( s_2.p_2 \) in ply \( s_2 \) of \( D_2 \) dimensions is:

**Equation V-B**

\[
d_2 = \sqrt{\sum_{i=1}^{D_2} \left( s_2.p_i(t+1) - s_2.p_{2i} \right)^2 }
\]

If the distance between \( s_1.p(t) \) and \( s_1.p_1 \) is \( d_1 \), suppose we pick a point for \( s_2.p(t+1) \) at random from points in \( s_2 \) about \( d_1 \) from \( s_2.p_2 \).

We know that **standard distance** in \( s_2 \) is \( \sqrt{\frac{S_2}{6}} \), where \( S_2 \) is the number of dimensions in \( s_2 \).

Let

\[
f = \frac{d_1}{\sqrt{S_2/6}}
\]

where \( f \) is the fraction of standard distance represented by \( d_1 \) in \( s_2 \).

Suppose we generate a random point, \( s_2.q \), in \( s_2 \), then interpolate from \( s_2.p_2 \) to \( s_2.q \) by fraction \( f \):

\[
s_2.p_i(t+1) = s_2.p_{2i} + f \cdot (s_2.q_i - s_2.p_{2i}), \quad i = 1..S_2
\]

Then \( s_2.p(t+1) \) will be at about the right distance \( d_1 \) from \( s_2.p_2 \).

---

This **interpolation form of rule** is simple but does not maintain certain statistics, such as the probability that a point \( p_1 \) chosen at random will lie at a distance no greater than \( d \) from the pattern point \( p_2 \).

From **Table III-1 in Chapter III**, we have:

\[
\text{prob}(\text{dist}(p_1, p_2) < d) \approx \left( \frac{d}{\mu} \right)^2 = \left( \frac{d}{\sqrt{N/6}} \right)^2
\]

Recall that \( \mu = \sqrt{N/6} \) is the **standard distance**, where \( N \) is the number of dimensions in the space.

The probability will be maintained only if \( s_1 \) and \( s_2 \) have the **same number of dimensions**.

## 7 There are many ways to obtain a good point

This is not normally the best way. It is presented here primarily because of its **simplicity**, which is useful for illustration.

---

# Chapter V Correlithm Object Machines

### Cell Form of Rule

We can quite easily compute a distance **d2** that maintains the same probability in **s2** that **d1** produces in **s1**. We simply set the probabilities in the two spaces equal to each other and solve for **d2**:

\[
\text{prob(dist\_in\_s2}(p1, p2) < d2) = \text{prob(dist\_in\_s1}(p1, p2) < d1)
\]

\[
d2 = 1 d1 + \mu_2 - \mu_1
\]

A rule such as **“s1.p1 >: s2.p2”** compactly expresses a set of rules of the form:

- \( s1.p1 >: s_i.c, i=1..S_2 \)

where **si** represents each of the dimensions in **s2** in turn. This form of rule causes a single cell to be viewed as a function over time. The cell “watches” the movement of the state point, **s1.p(t)**, in a given space such as **s1** and produces a new state value for itself that is close to the given target value, **si.c**, as the state point is close to the source point, **s1.p1**.

This “cell form” of rule highlights that the values taken on by a cell depend only on the values of the **source sides** of the rules that affect the cell. A cell’s next state value clearly must **not depend** on the values of other **target cells** that may have similar source-side rules.

### Target Cell Independence

Both methods require that each **target cell** knows the number of dimensions in both **s1** and **s2**. Knowing the number in **s1** is fine. Two target cells may be defined on the same source cells and remain completely **independent**.

But requiring knowledge of the number of dimensions in **s2** means that every target cell in **s2** depends on every other target cell in **s2**. If even one of the target cells fails, distances and statistics in **s2** would change, affecting every remaining cell.

It is highly desirable for **target cells** to remain independent because:

- **CO systems** are inherently **parallel**, offering exploitable parallelism.
- Target cell independence is perhaps the most important exploitable parallelism.
- The **CO computing paradigm** is a **statistical computing paradigm**.
- We want as much behavior as possible to emerge from **statistical processes**.
- Target cells should be selected **statistically, not algorithmically**.
  
This concept is **workable and highly desirable** in the CO computing paradigm. The question of how to compute target cell values will be revisited shortly, after deeper exploration of the CO paradigm.

### Dimensions Appearing in Multiple Rules

- Until now, rules have assumed a single source space and a single target space.
- A particular cell appears in exactly one place: either source or target.
- Usually, a **dimension** can appear in multiple **Terms** in both the Source and Target of many rules.

#### Syntax of a Rule

Consider this syntax definition:

```
<Rule> ::= <Source> >: <Target>
<Source> ::= <TermList>
<Target> ::= <TermList>
<TermList> ::= <Term> | <TermList> <Term>
<Term> ::= <Space> . <PatternPoint>
```

- A **Space** is a set of dimensions.
- A **PatternPoint** is one time-invariant dimension value for each dimension in the Space.
- A particular dimension will appear at most once in a Space’s dimension set.
- Spaces are usually chosen **independently** and often **statistically**, so two Spaces may have dimensions in common.

How are rules computed when the same dimension can appear in multiple Terms in Source and Target of any number of Rules? This is equivalent to asking: **how to compute the value of a single target cell**.

#### Key Concept

- A rule **does not set a value** into a target cell.
- It **weights** the target pattern value from the Rule for that cell.
- After all rules provide weights to their target cells, each target cell **independently computes** its new state value from its own accumulated weights and pattern value, then reinitializes its weights.

A rule passes the same **weight** to each of its target cells. The weight is often the **reciprocal of the probability of distance** used earlier (from Table III-1 in Chapter III).

Each cell computes:

- New value = **First moment** of the target values from all rules for that cell, weighted by corresponding weights.

The formula for a target cell’s new state value:

\[
\text{new cell value} = \frac{\sum_i \text{weight}_i * \text{pattern value}_i}{\sum_i \text{weight}_i}
\]

If there is only **one rule** contributing (k=1), the new cell value equals the pattern value regardless of weight. In practice, a "noise floor" may be injected at a preset weight to control exact matches.

This method allows each target cell to be **independent**. All a target cell needs is:

- Target pattern values.
- Weights from its various rules.

This method is **simple and effective**.

---

### Multiple Terms in Source and Target

Consider a rule:

\[
1.c.S1.p1 \quad s2.p2 \ldots >: s3.p3 \quad s4.p4 \ldots
\]

- Source contains multiple terms.
- To evaluate distance with multiple terms, use:

\[
\text{dist} = \sqrt{\sum_i (s1.p(i,t) - s1.p(i))^2 + \sum_i (s2.p(i,t) - s2.p(i))^2}
\]

- The square root contains sum of independent squared differences.
- A cell appearing multiple times in different subspaces is acceptable.
- Cells appear once in each subspace; different subspaces compare to different pattern points.

A cell appearing in source terms of different rules is not a problem; each applies formula independently without interaction.

Target also contains multiple terms; accumulation of contributions prevents ambiguity. Cells in the target of different rules accumulate incremental contributions and incorporate all by cycle end.

---

### Choice of Formulas and Paradigm

- Some may find choice of formulas arbitrary.
- **CO computational paradigm** relies on averages, approximations, and robustness to noise.
- Forgiving characteristics extend to formulas within bounds.
- We want **target cells** to compute next value independently.
- Formulas have firm basis in **statistics** and **information theory**.
- These formulas have been tested with **outstanding results**.
- Future work may find better formulas.
- Currently an **engineering approach** is employed.

---

# Chapter VI Completeness

**“Completeness”** is an important theoretical concept.

- Computation processes can be compared to towns and roads.
- If only traveling by road, are all towns reachable?
- If computing with particular computer, are all programs runnable?

Showing a computer has **completeness** means the computer can run **any program**.

### Traditional vs. CO Machines

- Traditional computers are based on the **Von Neumann architecture**:
  - Single processing unit.
  - Random access memory.
  - Serial execution (one thing at a time).
  
- CO architecture machines are:
  - Highly **parallel** and **redundant**.
  - Multiple things happen at once on several levels.
  
- Both Von Neumann and CO machines are **general-purpose computers**.
- Von Neumann completeness is well-known.
- Here, the completeness property for **CO machines** will be established, along with related properties.

---

### Logical “NOT”

One of the simplest logical operations is the **logical “NOT”**.

- Maps **True** to **False** and **False** to **True**.
- Established with two spaces: **s1** and **s2**, each with points for True and False.
- Write rules to connect them:

```
s1.True >: s2.False
s1.False >: s2.True
```

![Figure VI-1 Visualization of a logical NOT function](link-to-image-if-any)

- At time \( t \), \( s1.state(t) \) exists in **s1**.
- If \( s1.state(t) = s1.True \), distance to \( s1.True \) is zero; distance to \( s1.False \) is standard.
- The first rule's target, \( s2.False \), is weighted strongly.
- Second rule's target, \( s2.True \), is weighted slightly.
- \( s2 \) cells take on \( s2.False \) value, so \( s2.state(t+1) \) = False.
- Similarly, if \( s1.state(t) = s1.False \), then \( s2.state(t+1) \) = True.
- The **entire s1 space** divides into two regions mapping to \( s2.True \) or \( s2.False \).

---

### Logical Completeness

**CO machines are logically complete**, also called **Boolean complete**.

- This means they can implement **every logical expression** or **Boolean function**.
- To show this, it suffices to implement a **logically complete set** of logical functions.

---

### Logical “NAND”

Common logically complete sets:

- AND, OR, NOT.
- We have shown NOT; AND and OR require many rules.

Another **logically complete set** is the single function **“NOT-AND” (NAND)**.

- Early computers used NAND because it was simplest to implement electronically.
- NAND implementation with CO rules (see Figure VI-2):

```
s1.F s2.F >: s3.T
s1.F s2.T >: s3.T
s1.T s2.F >: s3.T
s1.T s2.T >: s3.F
```

![Figure VI-2 Logical NAND function visualization](link-to-image-if-any)

- Four rules, each with two source terms and one target term.
- Source term \( s1.F \): a point "F" in **s1**.
- Source term \( s2.F \): point "F" in **s2**.
- Target term \( s3.T \): point "T" in **s3**.
- The rule watches for appearance of source tokens and generates the target token accordingly.
- This set implements a **NAND gate**.

Thus, **CO machines are logically (or Boolean) complete**, able to implement any logical function.

---

### No Perceptron Limitations

The **Perceptron** is an early biologically inspired computing model.

- In 1969, it was shown to **fail** to compute certain important functions.
- CO architecture also has biological origins, but **does not suffer these limitations**.
- Limitations arise because Perceptrons are **linear devices**.
- Connectionism and Artificial Neural Networks (ANNs) show relaxing linearity removes these limitations.
- To demonstrate CO machines lack the Perceptron limitations, it suffices to show they can implement a function **not computable by Perceptrons**.
- The simplest such function is the logical **Exclusive-OR (XOR)** function.

---

### Logical “XOR”

An **XOR** function cannot be implemented by a **linear system**.

(Start of explanation with coordinates (0,0) is cut off)

## 8 Perceptrons: an Introduction to Computational Geometry  
**Marvin Minsky** and **Seymour Papert**, MIT Press **1969**

---

## XOR Problem in Linear Partitioning

The points \((0,0), (0,1), (1,0),\) and \((1,1)\) represent the four possible inputs to an **XOR** function. To specify XOR:  
- Assign **0** to \((0,0)\) and \((1,1)\)  
- Assign **1** to \((0,1)\) and \((1,0)\)  

It is **impossible** to place a single straight line in the input coordinate plane such that both 1s are on one side and both 0s on the other. This means the **XOR function is not linearly partitionable**, a result that can be proven rigorously.

### Figure VI-3: XOR Function Representation

- The **black points** represent 0 values  
- The **white points** represent 1 values  
- Left image: no **straight line** can separate zeros from ones  
- Right image: a **curved line** easily separates zeros and ones  

Relaxing the linear constraint enables partitioning with curved lines. Systems that implement curved lines can separate XOR outputs.

---

## CO Machine and XOR Implementation

A **CO machine** can overcome the limitations of a **Perceptron** by coding the logical **XOR** function using the following rules:

- `s1.Zero s2.Zero >: s3.Zero`  
- `s1.Zero s2.One >: s3.One`  
- `s1.One s2.Zero >: s3.One`  
- `s1.One s2.One >: s3.Zero`  

---

## Turing Completeness

A **Von Neumann machine** is **Turing complete**, capable of computing anything computable. This chapter argues a **CO machine** is also **Turing complete**, meaning both architectures can perform the same computations. They differ in strengths and weaknesses but can complement each other.

Proving **Turing completeness** rigorously requires showing the machine can implement a **Universal Turing Machine**. The presented argument is simpler and correct but not rigorously formal.

---

## Flip-Flop

Digital computers are built from:  
- **Passive Boolean logic**  
- **One-bit memory devices (flip-flops)**  

Since a **CO machine** can implement any Boolean function, showing it can implement a **flip-flop** proves it can build all components for a modern computer, demonstrating Turing completeness.

### CO Flip-Flop Rules

- `s2.p1 >: s2.p1` # Rule 1: remember s2.p1  
- `s2.p0 >: s2.p0` # Rule 2: remember s2.p0  
- `s1.p1 >: s2.p1` # Rule 3: force s2 state to s2.p1  
- `s1.p0 >: s2.p0` # Rule 4: force s2 state to s2.p0  

### Figure VI-4: Flip-Flop Conceptual Drawing

- **s2 state point** represents the flip-flop state  
- If **s1 state point** is far from \(p0\) or \(p1\), rules 1 and 2 latch the state  
- If s1 moves inside "white circles," rules 3 or 4 override latch and update s2  
- \(s1\) has more dimensions than \(s2\), allowing stronger influence from \(s1\)

---

## Flip-Flop Memory Functionality

- Rules 1 and 2 implement a **two-state latch**, enabling the state point to remain at either \(s2.p0\) or \(s2.p1\) indefinitely without external input  
- Rules 3 and 4 from \(s1\) control state transitions, overpowering latch rules when \(s1\) dimensions dominate  
- With dimensions, for example, \(s2=100\) and \(s1=200\), \(s1\) can generate stronger weights, stabilizing the flip-flop behavior

This achieves a **one-bit CO flip-flop**. Duplicating rules and spaces creates multiple flip-flops.

---

## Conclusion on Turing Completeness

Since CO machines can build flip-flops and any Boolean function, they can implement any current-day computer and thus are **Turing complete**.

---

# Chapter VII Strings

---

## Introduction to Strings in CO

We previously focused on **individual points** in a unit **N-cube**. Geometry teaches us that:  
- Lines, shapes like rectangles or spheres, and complex shapes are composed of **sets of points**.

**String correlithm objects (string COs)** represent sets of points defined by geometric shapes within the CO system.

---

## Geometric Relationships in Patterns

- Objects with geometric relations, like “**two eyes, a nose, and a mouth**,” form a **face** only if positioned properly  
- String COs represent spatial or temporal/sequence relationships in patterns—e.g., the difference between “Ready, aim, fire” and “Ready, fire, aim”  

---

## Extending Point COs to String COs

- String COs embed **lower-order geometrical objects** (lines, surfaces, volumes) into a higher-dimensional CO space  
- Distances in the original object correspond approximately to distances in the CO space, preserving shape roughly  
- String COs are **statistical objects**, similar to point COs

---

## Mapping String and Point COs

- Mapping a string CO to a single point CO produces a **soft token** for pattern generation or recognition  
- Mapping a string CO to another string CO morphs one geometric object into another under a mathematical transformation  

---

## Uses of String COs

- Represent geometric objects with **capture zones** akin to point COs  
- Capture approximate shapes so similarly shaped objects match  
- Represent sets of features with geometry, color, shape, texture, and their relations—providing powerful **data fusion**

---

## String Concepts

At the core, strings provide mappings between:  
- A **geometric object (G)**: any set of points including line segments, surfaces, volumes  
- A **single point (P)**: a CO point, usually in a high-dimensional bounded unit N-cube  

### Requirements for Mappings

- Mappings may be from \(G \to P\), \(P \to G\), or bi-directional  
- Small deformations of \(G\) (translations, rotations, magnifications, distortions, noise) map to small distances near \(P\)  
- This preserves the **degree of change in \(G\)** as distance in **\(P\)**  

### Figure VII-1: String CO Mapping

- A geometric object (**G1, G2, G3, G4**) maps to corresponding points (**P1, P2, P3, P4**) in CO space  

---

## Beyond Membership: Capturing Geometrical Characteristics

- Strings do not just capture membership in \(G\), which can be done by rules mapping points of \(G\) directly  
- Instead, strings capture **“G-ness”** or the intrinsic geometric form of \(G\)—like recognizing “chair-ness”  
- The goal: \(G\) and anything that “looks like \(G\)” associates to point \(P\) or a nearby point  

---

## Example: Generating a Face Pattern

- A face is composed of elements: **two eyes, a nose, and a mouth**  
- Small changes in relative positions maintain “face-ness”  
- Larger variations disrupt perception of a face  

The detailed geometrical structure of a face (**G**) can be captured with a point \(P\) and related strings; finer patterns (**lesser Gs**) correspond to strings of points (**lesser Ps**), forming a hierarchy of mappings representing detailed geometrical forms.

## Chapter VII Strings

### Figure VII-2 Explained

Even though the **image elements** remain the same, moving them much from their “expected” relative locations causes most people to fail to see a **face**. Most people describe a cartoon face as “**two eyes** here and here, a **nose** here, and a **mouth** here.”

Suppose we have the ability to recognize **eyes**, **noses**, and **mouths** anywhere in an image. We still need to capture their approximate **relative locations**—the idea of “**here**.” This is done with a **string CO**.

- The relative locations of elements are assembled into a set of **point COs**.
- Together with their relative **geometric coordinates**, they form an appropriate string CO, **G**.
- The string CO, **G**, is then mapped to a single point CO, **P**, representing the ensemble.
- Thus, **P** is a single token at the top of an organizational hierarchy representing a face.

### Hierarchical Organization of Faces and Elements

- String COs represent **geometrical relationships** among point COs.
- They can capture geometrical relationships among any kind of elements, including real-world objects, point COs, and other string COs.
- This hierarchy can extend to both higher and lower levels (See **Figure VII-3**):
  - Define an **eye** as a geometric relationship of elements (e.g., eyelashes, pupil).
  - Moving up, define a **human form** including “a face here, two arms here.”
- The top of the hierarchy is a single point representing an entire complex arrangement.
- The bottom consists of myriads of tiny elements at the limits of perception.
- The unifying feature is the approximate relative locations of all these elements.

### Robustness of String COs

- String COs are **robust representations** of geometrical objects.
- Elements may vary somewhat in position; some may be out of position or missing.
- The ensemble still generates a state point, **P'**, near the pattern point, **P**, representing the face.
- The string CO transforms the problem of assessing geometrical similarity into measuring the **distance** between two points:
  - The **pattern point**, **P**, representing an example arrangement.
  - The **state point**, **P'**, representing the current arrangement.

### Pattern Recognition and Pattern Generation Directions

- The hierarchy can be traversed:
  - **Upwards** — a “pattern recognition” direction.
  - **Downwards** — a “pattern generation” direction.
- CO systems can “see and do things like things previously seen and done.”
- Pattern recognition is **familiar** (patterns fit known classes).
- Pattern generation is less familiar and involves producing output patterns adapted to current conditions.
- String CO systems produce **adaptive pattern generation** reliably.
- In contrast, conventional approaches (e.g., robots) struggle to adapt, such as walking on unfamiliar terrain.

### Figure VII-3

An image is composed of a **hierarchy of elements** with particular geometric relationships. At each level, **string COs** capture relationships, representing both **geometry** and **hierarchy**.

### Importance of Approximate Mapping of Relative Location

- Approximate mapping of relative location is a **key capability**.
- Point COs apply **reliable labels** to state points near their pattern points.
- Adding the ability to reliably map **approximate relative location** completes the framework to build hierarchies.
- The mapping of **G** to **P** is engineering:
  - Number of hierarchy levels.
  - Kind and number of elements per level.
  - Allowed variation in location and element match.

---

## Approximate Relative Location

### Defining Relative Location

- Focus on the **“relative”** part of approximate relative location.
- An element can be relative to:
  - Another **element**.
  - A particular **reference point** or **origin**.
- Both forms are equivalent and convertible by simple geometry.
- Relating all elements to an origin means only **absolute positions** in relation to the origin matter.

### Use of Origins in Living Systems

- Human eye centers its image on something interesting (the **foveal spot** or **fovea**).
- Humans recognize shapes poorly when off the fovea by even a few degrees.
- (See **Figure VII-4**): Recognition of letter “A” degrades with radial distance from fovea.
- Elements of a letter hierarchy relate by relative location to the origin at the **center of the fovea**.
- The fovea is a widespread tool in human perception.

### Simplification Assumption

- For now, deal with relative location with respect to a **selected origin** like the fovea’s center.
- We assume the correct placement of the origin is done.
- A coordinate system centered on the origin is needed to define locations.

### Problem Statement: Mapping Approximate Relative Location

Given:

- An **origin**.
- A coordinate system related to that origin.
- Some point, **g**, in the coordinate system.

Associate CO **p** with **g** such that:

- Points near **g** are associated with points near **p**.
- Points closer to **g** produce points closer to **p**.

### Example with Spaces s1 and s2

- Space **s1** has pattern point **g** on a unit line with origin at left end.
- Associated pattern point **p** in high-dimensional space **s2**.
- As state point **g’** moves along s1 through **g**, corresponding **p’** in s2 moves with a functional distance relationship.
- When **g’** is far from **g**, **p’** is at standard distance from **p**.
- When **g’** approaches **g**, distance between **p’** and **p** goes to zero. (See **Figure VII-5**).

### Sampling Approach

- Set up **K rules** that sample the functional relationship:

  ```
  s1.h[i] >: s2.q[i],     i=1..K
  s1.h[i=n] = s1.g
  s2.g[i=n] = s2.p
  ```

- One rule maps **g** to **p**.
- Exact mapping not strictly necessary; sampling near this rule suffices.

### Figure VII-5

- Pattern point **g** (in s1) maps to **p** (in s2).
- Moving **g’** in s1 past **g** causes **p’** in s2 to follow a path through **p**.
- Distance between **p’** and **p** depends on distance between **g’** and **g**.
- Only near **g** does **p’** approach **p**.

### Wide Notch Enhancement with String CO

- Points in s1 and s2 are random COs.
- If state point **g’** is set to **g**, rules produce **p’** close to standard distances, except at **p** (distance zero).
- Want to widen the "notch" so that points near the pattern receive stronger mappings.

### Constructing the String CO in s1

- Move points **s1.h[i]** closer than standard distance.
- Pick **s1.h[1]** randomly.
- Pick **s1.h[2]** closer to **s1.h[1]** than standard distance.
- Similarly, move points **s2.p[i]** correspondingly closer than standard distance.
- Arrange distances between adjacent points **s1.h[i]** and **s1.h[i+1]** to be fraction **f** of standard distance where:

  - **0 < f ≤ 1**

- With **g’ = s1.g**, **p’** has zero distance to **s2.p** and smaller distances to neighbors.
- Neighboring rules gain relative strength due to this closeness.
- Distance increases asymptotically as rules get further away from **i = n** in s2.

### Effect of Fraction f

- Smaller **f** results in a wider “notch.” (See **Figure VII-6**).
- The notch follows the state point **g’** as it moves.
- If **g’** is not on this path, the notch behaves differently.

---

## 9 A Few Comments About This Process

The problem of building **string COs** is subtler than it may appear. Many things that one may initially think of to try have unexpected and undesirable side effects. For example, one might select two random **COs**, **a** and **b** in space **s1**, then perform a simple linear interpolation of their coordinates to produce intermediate COs. This indeed produces roughly the functional form we seek. 

But it turns out that the intermediate COs produced in this way are closer than **standard distance** to any third CO, **c**, selected at random. The interpolation actually “takes a shortcut” through the space **s1** that produces this behavior, which is almost certainly not what we want.

---

### Figures VII-6 and VII-7 Explanation

- **Figure VII-6** shows distance as a fraction, **f**, of standard distance in **s2** as rules get further away from strongest. Decreasing **f** makes the “notch” wider.

- **Figure VII-7** illustrates how as the state point moves away from the string CO, the depth of the notch shrinks. When the state point in **s1** is exactly on the string CO, it matches one of its rules producing a state point in **s2** at zero distance from that rule's pattern point. 

- When the state point in **s1** is at **standard distance** from the string CO, all rules produce points in **s2** that are standard distance from the state point.

---

## Ordinality

One use for such a one-dimensional **string CO** is **ordinality**. For example, suppose we build a one-dimensional string CO of twenty-six points and adjust their spacing so that the first and the twenty-sixth are closer together than standard distance. Thus, the distance between any two of them will be less than the standard distance, increasing uniformly in both directions from any particular string CO point.

Label the first point with **"A"**, the second with **"B"**, and so forth up to the twenty-sixth labeled **"Z."** If we set the state point in the space of this string CO to the point corresponding to **"C"**, the distance to each point reflects our intuition about the ordinal distance of the corresponding letter. For example, **"B"** is closer to **"C"** than **"A,"** and **"D"** is closer to **"C"** than **"E."**

In the same manner, define a string CO for the **Cyrillic alphabet** in the same space. All points in this **Cyrillic string CO** will be at **standard distance** from all points of the **English string CO**.

If the state point is set to the Cyrillic character **"Җ"** (the “zh” sound), the distances to other Cyrillic points will be ordinal. However, distances from this state point to every point in the English string CO will all be about **standard distance**.

---

### Mapping Alphabets to New Spaces

Set up rules that map all **English string CO points** to a single random CO labeled **"English Letters"** in a new space and all **Cyrillic string CO points** to another single random CO labeled **"Cyrillic Letters"** in the new space.

- If the state point gets near either string CO, the corresponding state point in the new space will approach the corresponding random CO. This tells us whether we are seeing English or Cyrillic characters.

- Add two new rules that map both random COs to a single random CO in a third space. This will be approached whenever either an English or a Cyrillic string CO point is approached in the first space.

This setup is illustrated in **Figure VII-9**.

---

### Figure VII-8

- Shows two string COs defined in the same space, one for the English alphabet and one for Cyrillic.

- Every point on each string CO is about **standard distance** from every point on the other string CO.

- Within each string CO, alphabetically adjacent points are very close together in the space.

- Points distant alphabetically are distant in space as well.

---

### Figure VII-9

- Every point in the string CO representing the English alphabet maps to a single CO labeled **"English."**

- Similarly, the Cyrillic alphabet points map to **"Cyrillic."**

- Both target points then map to a single CO labeled **"Alphabet,"** representing that the letters belong to alphabets.

- The letter **"A"** is first in both alphabets, so rules cause both COs to be at equal strength.

- This setup allows recognition that a letter is alphabetic but requires additional information to determine which alphabet it belongs to.

---

## Higher Dimensions

Approximate relative location is not useful just in **one dimension** but also in **higher dimensions**. For example, a face composed of eyes, nose, and mouth requires:

- Approximate relative location in **two dimensions** for drawings or photographs

- Approximate relative location in **three dimensions** for solid figures

Higher dimensionalities are often needed, but discussion here is confined to **two dimensions**, asserting that higher dimensional string COs are straightforward extensions of these concepts.

---

### Coding Approximate Relative Location in Two Dimensions

The problem is extended from one to two dimensions:

- Given an origin, a **coordinate system** related to that origin, and some point, **g**, in the coordinate system, associate CO **p** with **g** such that points near **g** are associated continuously with points near **p**, with points closer to **g** producing points closer to **p**.

- This problem statement works equally well for one, two, or more dimensions.

Consider a square with the origin at the **lower left corner** and each edge of **unit length**, called space **s1**. Place a pattern point **g** somewhere within it. Associate a pattern point **p** in a suitable high-dimensional space **s2**.

---

### Figure VII-10 

Depicts the desired behavior for **2D approximate relative position mapping**:

- If the state point for **s1** is placed at **g**, then distances to points in **s2** should form a “cone” around **g** that asymptotically approaches **standard distance**.

- Approximate contour lines of the statistical surface are shown in the square for visualization.

- The **z-axis** origin in this illustration is well above the plane of the unit square.

## 94 Correlithm Object Technology

There are two coordinates for **g**, the **x coordinate** and the **y coordinate**. One possible strategy for defining rules is to set up a one-dimensional string **CO** for each coordinate, then concatenate them to form one point in **s2**.

`s1.(x,y) >: s2.gx[x] s2.gy[y], x=1..X, y=1..Y`

One of the things we want for the string **COs** in both spaces is for the **“cone” of distance** around a given point to be **symmetrical**. This means that if a given point **g** at (x,y) in **s1** maps to **p** in **s2**, then all points at the same radial distance, **r**, from **g** in **s1** will map to points in **s2** that are all about the same distance from **p**.

### Figure VII-11

These distance **“troughs”** arise when trying to build a 2D string **CO** by concatenating two 1D string **COs** that correspond respectively to the **x and y coordinates** of a point in the unit square.

This simple strategy fails. (See **Figure VII-11**.) Since directions parallel to the **x** and **y axes** in the unit square produce **no changes** in the values of the corresponding 1D string **COs**, distance in **s2** is not increased by that string **CO**. Only the other string **CO** contributes to distance in **s2** in this circumstance, and the result is decidedly **not symmetric**.

---

### Figure VII-12

Here is another approach that fails. A point in **s1** is coded into **s2** by building a **coarse grid** of random **COs**, then **interpolating** among the four that bound the point.

The **“sags”** are caused by the interpolation process, which **“takes a shortcut”** through the high dimensional space of **s2**.

Another simple strategy also fails. (See **Figure VII-12**.) We might try laying out a coarse grid of random **COs** across the unit square, then doing a **linear combination** of the random **COs** at the four corners that bound the point of interest, **g** at (x,y). This indeed works reasonably well for points within those four corners, and the grid of random points itself does indeed produce **standard distances**, but there are **“sags”** in the surface.

These are caused by the same problem that plagued interpolation in the 1D case, namely the fact that such interpolations produce points that **“take a shortcut”** through **s2**. The result is that interpolated points are **not at about standard distance**.

Rest assured that there is at least one good solution to this problem. It would take more space than is reasonable here to investigate it in detail, but the essence of the solution is to **distribute the kinds of problems and anomalies** we have just seen **randomly across the space** to be coded.

The result is **computationally efficient**, even though the algorithm is not simple to describe. But once again, **randomness “comes to the rescue.”**

---

## Comments

As promised, this has been a **“gentle” introduction** to a very complex subject. We have left out many things. For example, the simple **line segment** and **planar patch string COs** presented here can be **“looped.”** 

- The ends of the line segment can be joined to form a continuous space, a **ring**.
- Similarly, one or both of the edges of the planar patch can be joined to its opposite. 
  - If this is done in one dimension only, a **tube** results.
  - If it is done in both dimensions, the result is a **torus**.

Any continuous object in any number of dimensions can be dealt with, so string **CO** coding is very general.

The ability to represent and relate geometrical objects in ways that map **deformations** of a given object to **distances** from the point **CO** that represents that object provides a powerful tool for **representing and generating patterns**.

It gives us the ability to:

- Construct **hierarchies of patterns** by giving specific examples.
- Have the security of knowing that anything **“like”** the examples we give will also be **robustly recognized**.

Such a hierarchy can be traversed:

- Toward its root to obtain **robust pattern classifications**.
- Toward its leaves to produce **robust behavioral patterns** that accommodate variations in encountered situations.

Finally, it should now be obvious that **string COs**, and therefore **CO machines** in general, have a natural, innate ability to represent **continuous, analog processes**.

A **CO machine** is very different from a traditional computer, with different strengths and weaknesses, as we will see shortly.

---

## Chapter VIII Architecture

**CO systems** run best on special **multiple-machine computing resources**. When we make popcorn, we do not pop the kernels one at a time because it would take too long. The tasks inside a **CO system** are like kernels of corn. We can **“pop” them all at once**.

A standard digital computer runs only one task at a time, and it cannot finish the tasks of a **CO system** very quickly. But a **CO system** runs fast on any computing resource that runs **many tasks at once**, such as:

- A **“grid”** of internet computer sites
- A set of **digital signal processors (DSPs)**

And there are even better designs on the drawing board.

The combination of a **CO system** and one of these **multiple-machine computing resources** has many unique properties.

- The **CO system** itself just goes on working correctly even if large numbers of the multiple processors fail.
- There is no need to reconfigure the computing resource or even to detect the failures.
- We can run multiple **CO systems** at the same time on the same set of machines, and they **won’t interfere** with each other.
- This is not **“timesharing”** or **“distributed processing”** as it is commonly known, but something more like a **hologram**.
- We can even establish the kind of **“group mind”** found in the determination and purpose of a **swarm of bees**.

**Correlithm Object (CO) systems** feature computing resources that are radically different from those of mainstream digital computers. The resources are inherently **concurrent** and can benefit greatly from **concurrent implementations**.

**CO systems** have unique properties:

- **Robustness**
- **Sampling**
- **Superposition**
- Even **holographic-like characteristics**, as well as others.

---

### The fundamental architectural unit of a CO system

The fundamental architectural unit of a **CO system** is the **cell**, which carries a **time-varying state** that is an independent function defined on the states of some set of cells.

- Potentially, no two cells have identically the same functionality.
- System behavior emerges from **statistical properties** involving individual cell functionality, the organization of the cells, and the flow of their intercommunications.
- In a strong sense, each cell has a unique **“point of view.”**
- A cell observes the states of other cells, compares those states to **reference states** it has previously stored, and generates its output as a **one dimensional, time-varying “opinion”** about the current situation.
- From time to time, a cell may add to its pool of stored reference states under various circumstances.

To approach an understanding of the nature of **CO systems**, one should visualize a **living brain**.

- No single cell is ultimately important.
- Although macroscopic functionality can be assigned to various regions of cells in a brain, a detailed inspection of those cells looks like nothing so much as **noise and chaos**.
- It is intuitively clear that each brain cell is contributing something to the whole, but it is anything but clear what a particular cell actually does from an information processing perspective.

**CO systems** are much the same.

This should come as no surprise because **CO systems** were derived from a theory of information processing in **living neural systems**, and such systems remain a guiding light in the evolution of **CO technology**.

The autonomy of individual cells and the **emergent behavior** of cell ensembles present unique architectural capabilities and opportunities that exist essentially independent of what processes the **CO system** is actually performing.

Many of these properties are worthy of study and exploitation in their own right, not merely as a means to enhance system performance.

They offer:

- New resources and perspectives that can be brought to bear in a wide range of applications.
- Frequently provide **robust and satisfying solutions** to long-standing, difficult problems.
- New directions of investigation that promise even more capability.

---

### Serial Implementations

Given the widespread availability of digital computers, it was inevitable that **CO systems** would appear first as programs for these machines.

We can view current computers as **serial systems**, although it is not an entirely accurate description.

- Certain kinds of concurrency are in fact being exploited in various places in these machines.
- But their predominant behavior is the execution of a **single programmatic thread**.

A current **digital computer** is perhaps the **worst possible system** to host a **CO process**.

**CO behaviors** emerge from the **statistics of ensembles**. Producing these behaviors necessarily involves dealing with:

- Large numbers of **cells**
- **Rules**
- Communications channels

A **serial or single-thread computer** must process this workload one element at a time.

Worse, the elements are all essentially **independent** at several logical levels and fully capable of correct behavior with little synchronization among them, a situation that simply begs for **high levels of concurrent processing**.

A **CO system** will potentially run many orders of magnitude **faster on appropriate concurrent hardware** than on a serial machine.

See **Figure VIII-1**.

## Concurrent Computing in CO Systems

**Figure VIII-1** shows that **CO systems** run faster on **concurrent computing resources**, such as multi-CPU machines, because **CO systems** are inherently **concurrent**. While current computers are ubiquitous and will execute a **CO system** correctly, many interesting properties are weak or unavailable on them.

Properly **concurrent host computers** are needed to fully exploit these capabilities. Fortunately, some hardware opportunities suitable for **CO systems** already exist, including:

- **DSP (Digital Signal Processing)** integrated circuits
- **FPGA (Field Programmable Gate Array)** integrated circuits
- Multi-CPU machines
- Clusters and distributed computing
- **Grid computing**

Future possibilities include **purpose-built CO integrated circuits** and even a purely **analog CO machine**.

---

## Concurrent Implementations

Within a **CO system**, there are at least **three levels of concurrency** available for exploitation:

- **Concurrent Rule Execution:** All rules can be executed concurrently.
- **Concurrent Source Cells:** All rule source sides can be executed concurrently.
- **Concurrent Target Cells:** All target cells can be executed concurrently.

See **Figure VIII-2** for a summary of these concurrency types.

### Concurrent Rules

- Rules can be executed independently without affecting each other.
- This ensures a **rule’s operation is independent** of others.
- The overall role of a rule depends on all rules, but its execution does not.

### Continuous and Asynchronous Operation

An ideal **CO system** operates concurrently, asynchronously, and continuously without a traditional clock or machine cycle. Cells emit values as continuous functions of **time**, allowing source state points to be continuous as well.

---

### Rule Independence and Execution

**Figure VIII-3** illustrates that:

- Rules read values from **source cells** independently.
- Internal processing depends only on values read from source cells.
- Rules can write different values to the same target cells.
- Formulas for computing new target cell values are **incremental**, ensuring correct handling of multiple nominated values.

This means:

- Rules can be executed concurrently without intrinsic execution order.
- Concurrent rule execution does not introduce unsolvable problems.

---

### Source Side Evaluation Concurrency

Evaluating the **source side** of a rule involves comparing the **state point** with the **pattern point**, generating a measure such as distance, probability, or bits.

- Comparisons are done dimension-by-dimension, each independent.
- These comparisons can be executed concurrently as shown in **Figure VIII-4**.
- Aggregation produces a single evaluation value after dimension comparisons.

There are two main approaches for source side evaluation concerning target cells:

- Evaluate the source once and distribute the result to all target cells (more efficient).
- Have each target cell perform its own source side evaluation to maintain autonomy, despite redundant work (**Figure VIII-5**).

---

### Target Cells Execution

- Target cells can be executed concurrently.
- Each target cell only needs its **pattern points** and the **source evaluation** to compute its next target value.
- Target cells do not need to coordinate with each other.
- This design requirement guarantees concurrency in target cell execution.

---

### Multiple CO Systems and Straddlers

- **Multiple instantiated CO systems** usually provide additional concurrency.
- Each **CO system** behaves like a living organism interacting with an environment.
- Many systems can operate concurrently and independently, interacting through the environment (**Figure VIII-6**).

A **straddler** is a **CO system** running on concurrent computing resources such as:

- Multi-CPU computers
- Grid computers
- LANs or the internet

Exploiting concurrency in a **straddler** reduces wall clock time and provides additional benefits.

---

### Hardware Acceleration

Simple hardware acceleration effectively exploits concurrency:

- Tools exist that convert **C/C++ source code** into efficient executable code for **DSPs**.
- Similar tools are emerging for **FPGAs**.
- These approaches provide significant performance improvements over serial computers.

---

## Analog Implementations

- **CO systems** can directly implement **continuous functions** as analog systems.
- Digital computers avoid using components' **active regions**, focusing instead on extreme states: full saturation (conducting fully) and cutoff (conducting nothing).
- Analog systems exploit the **active region**, allowing continuous computation.
- Historically, analog computers primarily solved **differential equations**.

### Current Status of Analog Computing

- Analog computation knowledge is limited among computer scientists today.
- Some control systems still use analog hardware.
- Digital systems increasingly replace analog systems by converting analog information to digital, running algorithms digitally, then converting results back to analog.

---

## Figure VIII-7 Analog Computers and Speed

**Analog computers** can be much faster than **digital computers**. A cup of coffee “solves” complex equations of **fluid dynamics** typically much faster than digital supercomputer simulations.

No general-purpose analog computer has ever gained prominence. Yet such a system might be very desirable. **Analog processes** are often inherently fast, sometimes much faster than their digital counterparts.

Consider a cup of hot coffee. Suppose we stir it and then let a drop of cream fall into it. The cream will disperse rapidly through the hot coffee. The discrete approximation needed to simulate this dispersal on a digital computer is daunting, and the computational time to produce a reasonable approximation will be long, even on the fastest digital hardware.

Clearly, there is a lot of computational power in a simple cup of coffee. But how do we harness this power? How do we build a **general-purpose analog computer**?

## CO Technology and Analog Computation

**CO technology** provides a means. CO machines are inherently analog, modeled after living **neural systems** which are definitely analog in nature. The functionality of a rule involves only simple mathematical operations that can be readily implemented in purely analog hardware.

- The basic computation is a **distance calculation** between a **state point** and a **pattern point**.
- The pattern point can be represented by preset analog quantities such as **voltage** or **current**.
- The state point is similarly represented but varies over time.
- To compute distance:
  - Form differences in each dimension using a **differential amplifier**.
  - Square each difference, sum the squares, and take a square root using the same kind of hardware.
- The remaining steps are equally simple mathematical operations.
- Due to strong noise tolerance of CO systems, the typical **two or three-decimal-digit accuracy** of such hardware is not a problem.

There is no significant impediment to purely analog implementation of rules. CO systems are **Turing complete**, meaning a CO system can implement any computable process. Thus, a general-purpose computer can apparently be built in purely analog hardware using CO technology.

## Direct Implementation of Analog Functions by CO Systems

CO systems can implement analog functions directly. Current digital computers implement analog functions by algorithms manipulating digital numbers. CO systems work by directly mapping points between spaces, essential for continuous functionality.

In the previous chapter on strings, we showed how to map lines, curves, surfaces, and similar objects.

For example, to implement a function such as **y = x²** by mapping, simply build two lines in separate spaces and map points (x values) placed with linear separations on the first line to points (y values) placed with quadratic separations on the second line. (See Figure VIII-8.)

In this way, arbitrarily complex functional relationships can be implemented directly by what is essentially **"table look-up and interpolation."**

## Figure VIII-8 Mapping a Quadratic Function

A **quadratic function** can be built by establishing two **string COs** and writing rules for the mapping. Here a string CO in space **s1** is mapped to another string CO in space **s2**.

## Increasing Computer Speed: A Third Principle?

Currently, only two general techniques are available to computer science to make digital computers run faster:

1. Build the same thing out of **faster parts**.
2. Do more than one thing **at a time**.

Every architectural enhancement of digital computer technology applies one or both of these principles. **CO technology** may add a third principle: the direct use of the inherent speed of **analog processes**.

## Properties and Capabilities of CO Systems

Here are some of the many unique things one can do with a CO system. The focus here is on properties and capabilities of the **CO machine architecture**, not on the CO processing itself (discussed in Chapter X).

### Sampling

A **sampled rule** will operate correctly. This means if a rule operates correctly, then another rule whose spaces are subspaces of the first—a sampled rule—will also operate correctly, provided the subspaces are not too small.

#### Figure VIII-9 Sampling Concept

Two points in their respective **three-dimensional spaces** and their shadows in corresponding **two-dimensional spaces** are shown. The left point maps in 3D space to the right point, and the shadow of the left point is correspondingly mapped in 2D space to the shadow of the right point. The shadow mapping represents a **sampling** of the original point mapping.

A CO derives its primary properties from **statistics defined on its many dimensions**. Properties found in high-dimensional COs begin to emerge in systems with about **20 dimensions**, rapidly becoming statistically robust as dimensionality grows.

Consider two rules:

- s1.p1 >: s3.p3
- s2.p2 >: s3.p3

Both relate a source point to the same target point.

- Let s1 have **2N dimensions**.
- Let s2 have **N dimensions**, a randomly drawn subset of s1.
- Point s2.p2 is a corresponding subset of s1.p1, so s2 is a sample of s1.

Both rules perform the same mapping; the first rule is stronger since s1.p1 matches carry about 2N σ bits, while s2.p2 matches carry about N σ bits.

If **N is big enough**—at least 20 and preferably 50 or 100—either rule works satisfactorily for most purposes.

### Sampled Target Spaces

The sampling principle applies if the space is sampled in the source, target, or both. The result is a **sampled rule**.

### Importance of Sampled Rules

The principle of sampled rules means CO systems can operate correctly even with partial **hardware failures**. For example, if some cells in the underlying hardware fail, leaving only a random subspace s3, the rule continues to perform its function with reduced information content.

This was illustrated in the case where cells of s1 are allocated across many physical computers, some of which become unavailable. The rule remains operational, demonstrating a **fail-soft** advantage inherent to CO technology.

## Figure VIII-10 Superposition Concept

**Superposition** allows correlithm objects (COs) to coexist in the same space. This is illustrated by three geometrical objects coexisting in the same three-dimensional space.

- Each object can still be discerned clearly.
- COs continue to function correctly while in superposition.
- Objects have reduced information content due to others acting as **noise**.
- Enough information remains for each object to retain its identity.

### Superposition in CO Systems

COs can be **superposed**. A single state point can be built from multiple COs in one space such that the COs coexist and can be recovered and used simultaneously.

This means multiple independent rules can be defined in the same spaces and still run correctly.

Example:

- In space **s1** of dimension N, define two random COs: s1.p1 and s1.p2.
- Their distance apart is approximately **standard distance** or N^½.
- Build a state point s1.p by averaging s1.p1 and s1.p2.
- The distance from s1.p to either s1.p1 or s1.p2 is approximately (1/√2) * standard distance or about 0.707 times standard distance.
- In terms of bits, a near-perfect match of a pattern point is about N σ bits, where σ ≈ 7/120 (standard deviation).
- The match of s1.p with either pattern point is about (1/2) * N σ bits.
- Generally, superposing K random COs by averaging reduces the bit match value to about 1/K of what it would be without superposition.
- If N is large enough, the chance of a random point matching well remains small enough to be negligible.

Use this approximation with care.

## 11 Superposition Property in CO Spaces

The **superposition property** is available in any **CO space**, not just the real unit N-cubes considered in this book. It is particularly interesting in **Hilbert spaces** (coordinate systems comprised of complex numbers), where it has significant implications for **quantum systems**. 

Cells of the CO computing paradigm can be of any numeric type, including:
- Binary
- Integer
- Real
- Complex
- More exotic forms

All such systems exhibit the **superposition property**. Despite some variation, all properties of the CO computing paradigm exist regardless of the **numeric type** of the cells or coordinates.

---

### Rule Mapping and Dimension Sharing

Consider the following two rules:

```
s1.p1 >: s2.p3  
s1.p2 >: s2.p4
```

- Both rules map points from the same source space **s1** to the same target space **s2**.  
- Each rule will carry at most **half the bits** they would without the other.  
- Both rules can share the two spaces and operate reliably if the number of **dimensions (N)** in each space is sufficiently large.

If **N** is "large enough," spaces can be allocated for rules and mapped into hardware with little concern for dimension sharing. Spaces are usually allocated at random from a large pool, and overlap is rare. The **superposition property** often allows overlap to be ignored safely.

---

### Deliberate Overlay and Frequency Division Multiple Access

In some cases, it is desirable to overlay many rules on the same space. An example is **frequency division multiple access** in telecommunications, where each frequency corresponds to one dimension of the space.

The principle of **superposed COs and rules** enables large numbers of channels and tokens to be employed.

---

## Statistical “Holograms”

The **sampling** and **superposition** principles of CO systems derive from their **statistical nature** and independence of cells.

Each target cell in a rule functions like a "point of view" by comparing the **state CO** in source cells to known source pattern COs. This is analogous to a **photographic plate** supporting a **hologram**.

### Holograph and Hologram

- A **hologram** is the 3D image formed by **holography**.  
- A **holograph** is the photographic plate carrying the interference pattern defining the hologram.

A holograph is made by "**freezing light**" (see Figure VIII-11):
- A real object emits light that travels to the holograph.  
- Each grain of the emulsion captures a unique “point of view” via the degree of darkening (opacity from 0 to 1).  
- Under proper conditions, each grain can reconstruct and retransmit the captured light, recreating the original light wave front.

Increasing the number of grains improves the resolution of the 3D image.

---

### CO System Similarity to Holograph

- Each **target cell** in a rule is a unique "point of view" on the match between **source state points** and **source pattern points**.  
- When the source state matches the source pattern, the rule enforces a match between **target state points** and **target pattern points**.  
- Increasing the number of dimensions improves the **resolution**, **robustness**, and **noise immunity**.

---

### Virtual Digital Computer Example

Suppose we build a set of rules to implement a current **digital computer** by assembling **logic gates** and **flip-flops** (Chapter VI).

- The virtual computer hovers over a plane of physical CO system cells like a **hologram** (see Figure VIII-12).  
- Programs can run, accessing virtual memory and disks.  
- Standard software (operating systems, accounting programs, etc.) can be installed.

---

#### Location and Robustness of Virtual Components

- Components such as the **CPU**, memory locations, or disk drives are physically dispersed across the CO hardware hosting cells.  
- Small units (flip-flops or gates) comprise many cells, which are randomly distributed.  
- Larger components are assembled from smaller parts, involving more cells.  
- The entire virtual digital computer is dispersed throughout all cells, making it "nowhere and everywhere" in the concurrent resource.

Loss of a single cell affects the virtual digital computer **almost not at all** because:
- Cells participate in many rules, each backed by many other cells.  
- Even the loss of a substantial percentage of cells probably would not cause failure.  
- Failure occurs only if the dimensions of one or more rule spaces fall below reliability thresholds—a probability that can be quantified and minimized.

---

### Implications for Brain Mapping

This robustness explains why it is difficult to map **brain functionality** precisely. 

- Large, generic regions such as speech or vision centers can be identified because they involve many cells.  
- If CO-style rules govern brain information processing, brain functionality is an **emergent statistical property** of cell ensembles.  
- The "**mind**" is a **virtual object** dispersed across ensembles, similar to the virtual digital computer described above.  
- Cells in these ensembles need not be numerous or physically close.  
- It is highly unlikely any two cells perform exactly the same function.

---

### Figure VIII-13: Robust CO System as a Swarm Mind

A CO system running on a concurrent resource can be extremely robust. For example, a single CO "**mind**" running in cells spread across a **swarm** of vehicles can sustain substantial damage without significant loss of functionality.

---

## Straddlers

**Straddlers** are CO systems running on **concurrent resources**.

- They are valuable in situations where computing resources must not fail.  
- A bonus is that virtual implementations of existing hardware and software can be achieved.  
- The primary goal of CO systems is their unique operational benefits, discussed in Chapter X.

---

### Swarms as Straddlers

A **swarm** is a useful type of straddler, named after insect swarms (see Figure VIII-13):

- Imagine tiny land, sea, or air vehicles each hosting CO cells.  
- Rules control each vehicle's basic operation.  
- Vehicles broadcast and monitor cell values via low-bandwidth communication.  
- Rules can operate across multiple vehicles, creating a **swarm mind** that orchestrates the entire group.  
- High-level rules can be superposed by encompassing all cells in all vehicles, making the system highly resilient.  
- No single vulnerable point exists; hierarchy above the individual vehicle is virtualized across the group.  
- Loss of vehicles reduces resolution but not overall functionality. The remaining group continues high-level behavior.

---

## Chapter IX Converters

Moving data **into and out of correlithm object space** requires **converters**.

- **Soft tokens** or points in high-dimensional spaces represent all data within a CO system.  
- **World data** (numbers, words, pictures, etc.) exist outside CO systems.  
- A **converter** translates between world data and soft tokens.  
- A converter translating from world data to soft tokens is a **sensor**.  
- A converter translating from soft tokens to world data is an **actor**.

---

### Converter Categories

Although infinite converter types can exist, most derive behavior from a few categories. This enables the creation of **generic converters** quickly adaptable to any world data, simplifying the translation problem.

---

### Mapping Points to Points

So far, we've seen rules mapping points to points (**COs to COs**) in high-dimensional spaces, representing all data.

To build practical CO systems, we need ways to move data **into** and **out** of CO space using **sensors** and **actors** (see Figure IX-1).

Despite apparent complexity, a few simple types of converters handle a large number of requirements.

---

### Next Steps

Here, three broad categories of converters—**cardinal**...

## 12 Some Terminology

Some might prefer the term **actuator** to **actor**, but **actuator** normally implies that **mechanical motion** results. We have chosen the term **actor** because it carries the more general connotation of **doer**.

---

## 120…Correlithm Object Technology

Converters include **cardinal converters**, **continuous converters**, and **ordinal converters**. These are useful in various **dimensionalities** and in two forms: **open** and **ring**.

- In most cases, **converters** can be used as either **sensors** or **actors**, depending on the desired direction of conversion.

### Figure IX-1

All **data representation** within a **CO system** is in the form of **COs**. **Sensors** convert world data into **COs**, and **actors** convert **COs** into world data.

---

## Cardinal Converters

Without unduly getting into mathematical issues, a **cardinal set** is defined as a set of objects with **no relationship other than set membership**. In particular, there is **no order** associated with a cardinal set.

- An important example of a (nearly) **cardinal set** for our purposes is a set of **random COs**, all defined in the same space.
- Some of these COs will be closer than others, but essentially they are **all the same distance apart**, making them very useful in **CO theory**.

A **cardinal converter** is a set of rules that maps a set of **cardinal objects one-to-one** onto a set of **same-space COs**. (See Figure IX-2.)

### Figure IX-2

A **cardinal converter** maps a set of unordered objects from the world outside the **CO system** to a set of **random COs** within it.

Example for logical values, **True** and **False**:

- Sensor functionality:
  - **True** >: **s.T**
  - **False** >: **s.F**

These rules state that if the data "**True**" arrives from the world outside, then a random CO, **s.T**, will be generated in the target cells of space **s**. The same applies to "**False**" with **s.F**.

- The presence of either “True” or “False” as input is considered an **exact match** of the rule.
- The number of **bits** this match is worth in a real CO system is a detail that does not matter much here.

Similarly, the actor definition is:

- **s.T** >: **True**
- **s.F** >: **False**

If the **state CO** in space **s** matches **s.T**, then “True” will be sent to the world outside, and if it matches **s.F**, then “False” will be sent.

- Matching a CO is usually **approximate**.
- Both **True** and **False** are always sent, but their **strengths** of matches vary.
- It is up to the outside world to decide the **threshold** of match strength.

Notice the **cardinal converter** maps external data elements onto the same space, **s**. This is **standard practice** for a cardinal converter.

---

## Continuous Converters

A **continuous converter** maps a **continuous variable** to another **continuous variable**.

> The name can be misleading; as of this writing, there are **no truly continuous converters available.**  
> Actual converters sample continuous functions and use various schemes to **interpolate**.

### Figure IX-3

This continuous sensor maps the **unit line segment** into a CO system by mapping three world points: **0.0**, **0.5**, and **1.0** to the string CO. Intermediate points, such as **0.4**, are mapped automatically to **interpolated CO positions**.

- Suppose the data is a **floating-point variable** in a bounded range, e.g., zero to one.
- The unit line segment between zero and one is mapped **linearly** onto a one-dimensional **string CO**.
- As a sensor, input **0.4** produces a **state point** in the target cells, four-tenths along the string CO from zero to one.

The state point produced will not be “near” but **exactly on** the string CO.

- The input and the string CO are **one-dimensional**.
- There is no approximate position; the state point cannot be anywhere else.

As an actor, the continuous converter behaves differently:

- Exact matching with a point on the string CO is **extraordinarily unlikely** because state points in CO systems are generally **noisy**.
- The actor must provide the **strength** of the output.
- It finds the **closest point** on the string CO and maps the corresponding data line point.
- The output **strength** reflects the proximity of the closest CO point.

### Figure IX-4

This continuous actor maps a string CO to a world unit line. The state CO will usually lie **off the exact line**. The closest point on the string CO is chosen and mapped to the world, along with a **strength** representing closeness.

- Usually, a continuous **linear converter** is sufficient for many CO system applications.
- Non-linear mappings are possible or can be handled within the CO system itself.

> **Note:** No truly continuous converters exist currently as all CO systems are built using digital hosts. When analog CO host hardware becomes available, this might change.

---

## Ordinal Converters

An **ordinal converter** combines features of **cardinal** and **continuous converters**.

- An **ordinal set** is a **cardinal set** with an **order among members**.
- The **alphabet** is an example of an ordinal set.

An ordinal converter builds one rule for each ordinal set member, like a cardinal converter, but:

- The ordinal set members are mapped in **evenly spaced order** along a **string CO** as in a continuous converter. (See Figure IX-5.)

### Figure IX-5

This sensor maps the **alphabet**, an ordinal set, onto a **string CO**:

- Data from the world is accepted as an ordinal set member.
- The corresponding rule matches strongly, and the target state CO gets the corresponding string CO point.

Nearby string CO rules also partially match, with the strength **decreasing with ordinal distance**.

### Figure IX-6

- Shows points in the plane illustrating the **sequential relationship** of elements of a string CO.
- Height represents **distance in CO space** from the reference CO.
- Similar distance relationships apply to all string CO elements.

This means:

- Nearby points influence the target state CO but **less strongly**.
- Ordinality is represented as **superposed state COs** with strengths that diminish with ordinal distance.

As an actor:

- The ordinal converter accepts a source state CO.
- It produces strengths of matches with every point on the string CO paired with a data ordinal value.
- These strengths are provided to the world.
- World programs decide the use of this information.

---

## Higher Dimensional Converters

Higher dimensionality applies only to **continuous** and **ordinal converters**:

- **Cardinal converter** data are **discrete, unordered**; placing them in 1D or higher-dimensional spaces would impose an order.
- **Dimensionality** makes sense for **continuous converters** and **ordinal converters**.

A 1D continuous converter maps a line segment to a 1D string CO. A 2D continuous converter maps a planar segment to a 2D string CO, and so on.

- Rules are placed at selected points throughout to **sample and define** the mapping.

### Figure IX-7

- A 2D world region is mapped into a CO system.
- Four black points in the 2D world map to four members of the 2D string CO.
- Distances between points in the 2D world correspond to distances between CO points.
- A gray point in the world maps proportionally in the string CO.

Ordinal converters are defined similarly for any number of dimensions.

- They are not limited to simple geometric figures.
- The world data geometric basis can be curved lines, deformed surfaces, or irregular solids.

---

## Converter Form

Revisiting the simple continuous converter (Figure IX-3) that linearly maps a line segment between 0 and 1 onto a 1D string CO:

- What if we want to make a **ring** out of this line segment? 
- That is, points **0** and **1** would be right next to each other.
- A **1D string CO** does not have this property; endpoints are typically at standard distance from each other depending on the notch width.

The solution is to use a **ring CO** (briefly mentioned in Chapter VII).

- A **ring CO** is created by joining the endpoints of a string CO to form a ring.

### Figure IX-8

- (Not fully detailed here)  
- Shows how joining the endpoints of a string CO creates a ring CO, useful for circular/conjoining data mappings.

---

## Summary of Converter Types

- **Cardinal Converters:** For unordered discrete sets; map each element to a unique random CO in the same space.
- **Continuous Converters:** Use sampled functions to interpolate continuous values; map continuous variables between world and CO space.
- **Ordinal Converters:** Map ordered discrete sets onto ordered locations along a string CO; represent order through match strengths.
- **Higher Dimensional Converters:** Extend continuous and ordinal converters into multiple dimensions.
- **Ring Converters:** Modify string COs by connecting endpoints to form rings for circular data mappings.

## Continuous Converter and World Ring CO

- On the left of **Figure IX-8** is a **continuous converter** that maps the **real number line from zero to one** onto a string **CO**.
- The string CO elements labeled **“a”** and **“b”** are probably at standard distance from each other.
- On the right, “a” and “b” can be the **same string CO element** by using a **ring CO**.

## CO System and Geometrical Mappings

- **Converters** can map any **geometrical relationship** into or out of a **CO system** (Figure IX-9).
- Shapes include familiar ones like the **sphere** and **torus**, as well as exotic shapes like the **Möbius strip** and the **Klein bottle**.
- Geometrical relationships can be maintained and manipulated entirely within a CO system.
- Any geometrical object can be mapped into **CO space** if the space is of suitably higher dimensionality.
- This includes **finite and infinite objects**, fractals, and objects from alternative geometries (e.g., **Riemann space**, **Lobachevsky space**).

### Form and Topology

- **Form** refers to whether endpoints are joined.
- Joining the endpoints of a line segment creates a **ring**; otherwise, it is an **open line segment**.
- Joining one pair of opposite edges of a rectangle forms a **tube**.
- Joining the other pair of opposite edges, or closing the tube, forms a **torus**.

> **Note 14**: The joined edge can be twisted before joining (e.g., rolling the y-axis to form a tube or twisting to form a Möbius strip). If the topology can be defined, it can be mapped onto a suitable CO space.

---

## Chapter X: Mechanisms of Behavior

- A **dog** and a **laptop computer** both process information but excel at very different tasks.
- **Animal intelligence** has inspired many projects to embed animal behaviors into automated processing, mostly with disappointing results.
- **Correlithm Object (CO) systems** share many behaviors with animals. These are not programmed but **“fall out” naturally** when using **CO soft tokens**.
- CO systems “see and do things like things previously seen and done,” similar to animals.
- Applications for CO systems exist wherever humans, animals, or insects outperform computers.
- Obvious application: **robotics**—but applications go far beyond, analogous to the diverse uses of **lasers** today.
- Imagine smart everyday devices behaving like intelligent animals and **“supersmart” CO systems** pushing knowledge forward.

---

## CO System Architecture and Behavior

- Chapter VIII discussed properties and capabilities of the **CO machine architecture**.
- Here focus is on **CO processing**, programming with **rules** to build systems on a CO machine.
- A **synthorg®** is a synthetic organism with:
  - **Sensors** (e.g., antennas, eyes)
  - **Actors** (e.g., wings, legs)
  - An internal **CO system functioning as its brain** (Figure X-1).

### Behavior and Mechanisms

- Behavior applies hierarchically: whole synthorg behavior down to individual rules.
- A **mechanism** is a CO program, or a set of rules running on a CO machine, designed to produce specific behavior.
- Example rule:

  ```
  s1.p1 >: s2.p2
  ```

  - Relates point **s1.p1** in **space s1** to point **s2.p2** in **space s2**.
  - Mechanism compares source state point to **s1.p1** and influences target state point position relative to **s2.p2**.
  - The closer the source gets to **s1.p1**, the closer the target gets to **s2.p2**.

- This chapter samples techniques illustrating what synthorgs can do compared to traditional computers.

---

## Turing Completeness

- CO systems are **Turing complete** (Chapter VI), meaning they are **general-purpose computing paradigms**, capable of computing anything computable.
- CO systems are **not more or less powerful** than traditional digital computers but have different strengths.
- Digital computers excel at **precision**:
  - Example: adding 1 million 10-digit numbers.
  - They work best with **crisp, clean, exact data** and programs with perfectly followed steps.
- CO systems excel at **similarity**:
  - Example: making a robot sit in an unfamiliar chair.
  - They excel at adaptation, recognizing **“chair-ness”**, and handling approximate or noisy data (Figure X-2).

---

## Soft Tokens

- Digital computers use **hard tokens**: data is either represented or not, with **sharp edges** and no intermediate degrees.
- CO systems use **soft tokens**:
  - Tokens represent data in a **continuous range** from “fully represented” to “not represented.”
  - They have **soft edges** and influence zones.
  - The closer a state point is to a CO, the stronger it represents that data (Figure X-3).
- Within a space, soft tokens have **equal stature** and form independent sets with no intrinsic ordering.

### Capacity of Soft Token Spaces

- The number of soft tokens depends on the need to avoid false matches.
- To ensure a mismatched token event occurs less than six standard deviations from the mean:
  - A **50-dimensional space** provides ~**10 million** usable soft tokens.
  - A **100-dimensional space** provides ~**10¹⁷** usable soft tokens.
- Reducing requirements (e.g., 1 million soft tokens in 100-dimensional space) results in even greater safety margins against false matches.

---

## 15 String COs

**String COs** are composed of **COs** that do not have equal stature. These form **soft string tokens**.

---

## Chapter X Mechanisms of Behavior

### Token Properties

**Figure X-4**: The tokens used to represent data in **CO systems** have three important properties:
- They all have a **“soft” nature**.
- They are all **equal in stature**.
- A **huge number** of them are available.

These are inherent properties of the underlying mathematics, not “engineered” into CO systems.

It is important to understand that these are **intrinsic properties** of CO systems. The “soft” nature, “equal stature” nature, and the huge number of available soft tokens were not designed into CO systems. They simply exist as characteristics of our CO paradigm and rank as the **most important mechanisms of behavior** in CO systems. Without these properties, we would likely not study CO systems.

---

### Robustness

Rules built with **soft tokens** are inherently **error correcting** and **robust in the presence of noise**.

- Traditional error detection and correction codes select **uniformly sparse points** in high-dimensional spaces (e.g., N-bit binary spaces).
- These algorithmically selected points are sparse, so small errors typically produce a point closer to one selected point than others.
- The erroneous point can be flagged as an error or corrected to the nearest valid point, which is standard practice.

However, **randomly selected points** in high-dimensional spaces tend to be sparse and uniformly spaced as well, offering:
- Similar or better error correction behavior.
- A state point near a random point implies they represent the same thing, with differences caused by noise.
- This likelihood can be used to perform error detection and correction similarly to traditional systems.

**Figure X-5** illustrates that in a high-dimensional space, any three random points will be approximately equidistant. If two points are closer than expected, they likely stem from the same underlying process plus some noise.

---

### Noise Retention

Consider this rule:

```
s.p >: t.q
```

- A state point in space **s** near **pattern point s.p** will nominate a state point in space **t** similarly near **pattern point t.q**.
- A noisy version of s.p produces a noisy version of t.q.

This process:
- Preserves both the **identification** and the **confidence level** of the data.
- Allows downstream actions to consider how sure the system is about its identification.
- Retaining noise allows confidence information to remain, while “correcting” to exactly t.q would lose this confidence information.

---

### Superposition

The **superposition mechanism** of CO systems means that a state point in a space can **simultaneously represent two or more things**. A state point can be near multiple pattern points with various distance ratios.

- Given a space **s** with **k pattern points s.p1 through s.pk**, there are regions defined by the intersection of “soft spheres” surrounding these points.
- A state point in such an intersection exhibits specific distance ratios to those pattern points.

**Figure X-6**: 
- Points s.p1, s.p2, and s.p3 are all approximately equally distant.
- The state point s.state is equally distant from s.p3 but about half the distance to both s.p1 and s.p2.
- Thus, s.state can represent both s.p1 and s.p2 simultaneously.

This means:
- A state point can represent any subset or all pattern points with any weight ratios.
- This is called **superposition** because the single state point simultaneously represents all soft tokens and their weight ratios.

Superposition is powerful, enabling strong data fusion:

- For example, a geometric pattern such as the letter “A” can be mapped where each point i is associated with a CO s.Ai.
- The simultaneous instantiation of all s.Ai results in a superposed state point s.A that represents the entire geometric pattern.
- Variations or missing points only slightly shift this state point, maintaining statistical closeness to s.A.
- This allows reliable probabilistic assignment between arbitrary points and known patterns.

---

### Strings

- All mechanisms applicable to **point COs** also apply to **string COs**.
- A **string CO** is a path surrounded by an **influence tunnel**, analogous to a point CO’s influence zone.
- Strings represent **soft sequences**, just as point COs represent **soft tokens**.

**Figure X-7**: A soft string CO consists of COs placed close together; their overlapping influence zones merge to form an influence tunnel representing soft sequences.

---

### Patterns and Functions

- All mechanisms are important for both **pattern recognition** and **function generation**.
- Pattern recognition enables recognizing patterns similar to known patterns.
- Function generation is the reversed capability: generating behavior similar to known behavior but adapted to current situations.
- CO systems can adapt previously learned skills to current situations, enabling functional generation with small or large adjustments.

Example:

```
s1.pattern s2.context1 >: s3.action1
s1.pattern s2.context2 >: s3.action2
```

- If the state point in **s1** is near s1.pattern and in **s2** near s2.context1, the nominated state point in s3 will be near s3.action1.
- Different context states lead to different action nominations.
- This generalizes to complex behaviors.
- A robot programmed with simple walking rules for even ground can adapt to uneven or sloped terrain, illustrating CO systems' ability to adapt behavior.
- This adaptability distinguishes CO systems from traditional computer systems.

---

### Rule Pathways

- Sets of rules define **pathways** through a CO system.

Example:

```
a.p >: b.q
b.q >: c.r
```

- The target of the first rule is the source of the second, forming a pathway:

```
a.p >: b.q >: c.r
```

Adding another pathway:

```
a.p >: b.q >: c.r   # previous pathway
d.s >: b.q >: e.t   # new pathway
```

- Both pathways share pattern point b.q.
- A state point near a.p or d.s nominates points near b.q in space b.
- The two nominated points in b combine by **superposition**, carrying forward proximity confidences.
- This combined state point triggers nominations near c.r and e.t.
- Both pathways converge and interact at b.q.
- For example, if the state point in a is close to a.p and d is further from d.s, nominations in c will be stronger than in e.
- We can design the system to **favor one pathway** over another.
- Converging pathways are complex and require careful programming to ensure intended behavior.

## Simplifying Rule Descriptions

The preceding narrative is a bit clumsy due to the complexities of describing the operations implied by the notation. By now, it should be understood how **rules** deal with **pattern points** and **state points**. We will therefore adopt simpler descriptions and bear in mind the more exact forms.  
For example, we describe the rules above as "**a.p or d.s drives b.q and b.q drives c.r and e.t.**"  

We will also adopt another simplification:  
```
p1 >: p2 >: p3  # where: p1 = a.p, p2 = b.q, p3 = c.r
p4 >: p2 >: p5  # where: p4 = d.s, p5 = e.t
```

Since all five spaces are different, and there is only one point named in each space, these substitutions are unambiguous. For example, **p1** means “a random point in a random space.” Similarly, **p2** means “another random point in another random space.” It's not important which points are in which spaces, only that they are not the same points or spaces. When more information is needed, we can use previous coding methods. This convention simplifies writing some pathways.

---

## Grammar Systems

The **rules of CO systems** can be used to build **production systems** of formal **grammars**. The basic strategy:  

- Use **converters** to get terminals (world data) into and out of the system  
- Use **soft tokens** to code all the **non-terminals**  
- Code the actual **productions** almost directly as **CO rules**

Important unique behaviors occur with this method, which we explore below, focusing on **trees**.

---

## Parse Trees

We use the simplified notation introduced above. Consider the parse tree shown in **Figure X-8**.

- If **p1** or **p2** match, then **p5** builds  
- If **p3** or **p4** match, then **p6** builds  
- If **p5** or **p6** match, then **p7** builds  

**Terminology:**  

- Elements **p1** through **p4** are called **leaves**  
- **p7** is the **root** of the tree  
- **p5** and **p6** are **intermediate nodes**  

In typical use: one of the leaf elements instantiates, e.g., **p1**. The presence of **p1** causes **p5** to instantiate, which causes **p7** to instantiate. We say "**p1 is a p5, which is a p7.**" Thus, **p1** is a member of the set that can instantiate **p7**.

```
p1  p2  p3  p4
p5  p6
p7

p1 >: p5
p2 >: p5
p3 >: p6
p4 >: p6
p5 >: p7
p6 >: p7

Figure X-8: Tree structure (left) coded by CO rules (right).
```

### Differences from Traditional Parse Trees

- In a **traditional system**, exact matches are required at every tree level.  
- In a **CO system**, only **like matches** are required. The **state points** at each new level are a **superposition** of the nodes below.  
- Traditional system instantiates **p7** only if a pathway matches exactly from a leaf to **p7**.  
- A CO system instantiates **p7** to the degree of the **strongest matching pathway** from any leaf.  

Thus, **CO systems implement a soft parse tree**, while traditional systems implement **hard parse trees**.

### Superposition and Concurrency in CO Systems

- The strongest matching pathway dominates the **p7** state point.  
- All pathways influence the **p7** state point concurrently.  

Rewrite the recognition tree as:  

```
p1 >: p5 >: p7
p2 >: p5 >: p7
p3 >: p6 >: p7
p4 >: p6 >: p7
```

- All four pathways are active concurrently in a CO system unless deliberately coded otherwise.  
- CO systems inherently perform **context-sensitive searching concurrently**.  
- If a path to the root exists, it dominates the root's state point.  

This concurrent context-sensitive search is a key advantage of CO systems.

---

## Generation Trees

Previously, we described going from **leaves (p1-p4)** to the **root (p7)**; this is the **recognition direction**. It's also possible to go from the **root** to **leaves**, called **derivation** or **generation** direction.

Consider the generation tree in **Figure X-9**.

```
q4  q5  q6  q7
q2  q3
q1

q1 >: q2 >: q4
q1 >: q2 >: q5
q1 >: q3 >: q6
q1 >: q3 >: q7

Figure X-9: Generation tree (left) coded by CO rules (right).
```

### Issue of Multiple Equally Strong Pathways

- If **q1** matches, everything else will be built: **q1** builds **q2** and **q3**, **q2** builds **q4** and **q5**, **q3** builds **q6** and **q7**.  
- Normally, only one of these four pathways should dominate.  
- In CO systems, all four seem equally strong, but this is only approximate.

#### Statistical Strength Differences

- Building state points is a **statistical process**; one pathway will be slightly stronger.  
- Suppose **q2** is stronger than **q3**; hence **q4** and **q5** favored over **q6** and **q7**.  
- Similarly, between **q4** and **q5**, one will be slightly stronger.  
- All four leaves (**q4..q7**) are built at different weights.  

### Establishing a Winner Using Attractors

Example code:

```
q4 s.q4 >: s.q4
q5 s.q5 >: s.q5
q6 s.q6 >: s.q6
q7 s.q7 >: s.q7
```

- **s** is a space with points named after tree leaves.  
- The strongest leaf triggers the corresponding rule, "latching" in space **s**.  
- Space **s** has four regions (s.q4 to s.q7), each centered on its pattern point.  
- The state point matches most strongly with its region, making the rule rapidly force the state point to that pattern.  
- The rule "latches", not changing without stronger influence.

More code can allow the state point to track the winning leaf, but the main point is we can establish a clear winner.

### Using Context Rules to Bias Pathways

Add two new rules:

```
q1 >: q2 >: q4
q1 >: q2 >: q5
q1 >: q3 >: q6
q1 >: q3 >: q7
t.q8 >: q4  # first new rule
t.q9 >: q5  # second new rule
```

- **t** is a space with points **t.q8** and **t.q9** that are **cardinal** (strong in one means weak in the other).  
- **q4** is driven by **q2** and **t.q8**; **q5** by **q2** and **t.q9**.  
- **t.q8** and **t.q9** act as **context rules** for biasing pathways from **q2**.  
- By managing space sizes or strengths, pathways can compete on any basis.  
- Similar control can be applied to other pathways, giving complete control over generation tree behavior.

### Note on Rule Equivalence

The two rules:

```
q2 >: q4
t.q8 >: q4
```

are equivalent to:

```
q2 t.q8 >: q4
```

if **bits** are used as the comparison metric. Otherwise, they differ. Assume bits are in use unless otherwise stated.

---

## Ladders

Rule pathways can **branch** and **converge** arbitrarily. Higher-level structures repeat frequently; one such is the **ladder** (see **Figure X-10**).

```
pr0.s in.s fg0.s >: pr0.s  # r1
pr1.s pr0.s fg1.s >: pr1.s  # r2
pr2.s pr1.s fg2.s >: pr2.s  # r3
pr2.s x.s >: x.s  # r4
x.s fg2.s >: fg2.s  # r5
pr1.s fg2.s fg1.s >: fg1.s  # r6
pr0.s fg1.s fg0.s >: fg0.s  # r7
fg0.s >: out.s  # r8

Figure X-10: Ladder mechanism with hierarchical pattern recognition, function generation, short-term memory, and context.
```

- The **"s"** suffix is preceded by a named space; points like **fg2.s** in different rules refer to the same point.  

### Ladder Structure

- Each space receives input from three spaces: itself (**feedback**), a **through-feed**, and a **cross-feed** space.  
- Example: space **pr1** takes input from **pr1** (feedback), **pr0** (through-feed), and **fg1** (cross-feed).  
- Typically, ladder has two paths:  
  - Left path: pattern recognition (e.g., **in, pr0, pr1, pr2, x**) with **x** as root  
  - Right path: function generation (e.g., **x, fg2, fg1, fg0, out**) with **x** as root  
- Cross-feed paths connect as needed, creating a complex system.

### Functions of Ladder Components

- **Pattern recognition through-feed paths**: hierarchical summarization into general **soft** classes.  
- **Function generation through-feed paths**: hierarchical derivation into specific **soft** actions.  
- **Feedback paths**: attractors functioning as short-term temporal memory to maintain current system state.  
- **Cross-feed paths**: provide context for both recognition and generation.

### Advanced Properties

- Each dimension can have unique rules and viewpoints.  
- Organizing dimensions into spaces simplifies work but is not required by CO systems.  
- The concurrency and superposition properties encourage highly parallel, overlapping computations.

### State Point in Space x

- The **state point** in **x** is the most general **soft token** in a CO system.  
- It summarizes the entire system state, including all concurrent and superposed pattern and action classes.  
- Each dimension carries slightly more than four bits of information, so **x** holds substantial information and concurrent rules.

### Simulation Capability

- Input/output can be suppressed by maintaining attractor **latch states** in spaces like **pr0** and **fg0**, controlled by higher-level spaces.  
- Cross-feed loops present superposed patterns/actions corresponding to known inputs/outputs.  
- Higher levels evaluate these simulated interactions as if from the environment.  
- Cross-feed paths may establish these conditions at multiple levels concurrently due to superposition.  
- Upon observing a suitable action, the latch states are released to **play back** the solution.  
- This hierarchical simulation enables examining and selecting actions using the full representational power of CO systems.

---

## Automata

Finite automata and formal grammars have strong equivalences. For example:

- A **Turing machine** is equivalent to a **Type Zero** grammar.

Choosing to implement functionality in one form or another can be beneficial.

### Comparison Summary

- **Turing machines** and **Type Zero grammars** are generally impractical for many uses.  
- **Context sensitive grammars** correspond to **linear bounded automata**; working with grammars (e.g., as trees) is often better.  
- For **context free** and **linear grammars**, equivalent **stack machines** and **state machines** are easier to work with.  

We will look at simple cases of these two important classes.

---

## State Machines

A **state machine** exists in **one discrete state** at a time.

- Accepts input causing it to stay or change state.  
- Generates output each time it evaluates its state.

**Figure X-11** example: a state machine that counts inputs **modulo two**.

```
States: s1 (0), s2 (1)
Transitions:
s1 --0/0--> s1
s1 --1/0--> s2
s2 --1/0--> s2
s2 --0/1--> s1

Equivalent CO Rules:

s2.0 s1.0 >: s2.0 out(0)
s2.0 s1.1 >: s2.1 out(0)
s2.1 s1.1 >: s2.1 out(0)
s2.1 s1.0 >: s2.0 out(1)
```

- Left diagram: Traditional depiction with states and transitions labeled **X/Y** means: if in that state and input **X**, output **Y** and transition accordingly.  
- Start in zero state. Inputs lead to outputs and state changes as per rules above.

### CO System Implementation

- Uses two spaces of different sizes: e.g., **s1 >> s2**  
- **s2** is the latch space, **s1** controls transitions.  
- **out(X)** indicates an actor sending output **X**.

Points like **s1.0** and **s1.1** represent states (e.g., zero and one). They are **CO pattern points** corresponding to soft tokens.

### Robustness to Noise

- If inputs are soft tokens representing colors, e.g., **red** and **green**, CO operates similarly.  
- An input like **pink** (a "little bit of red") produces a state point close to **s1.0** ("red").  
- Rules operate **weakly but correctly**, providing robustness unavailable in traditional state machines.

---

## Stack Machines

A **stack machine** = state machine + **stack**.

- The stack holds tokens with **last in, first out** behavior.  
- Tokens are pushed onto the stack and popped off in reverse order.  

Example: push A, then B, then C.  
Pop operations return C, then B, then A.

### CO Rule Example: Two-Level Stack with Two Tokens

**Figure X-12** shows a two-element stack storing tokens **A** or **B**.

```
Stack Levels: empty, L1, L2
Tokens: A, B

Rules:

// control s1 by s2
s2 >> s1

// stack empty
s1.empty s2.push in(A) >: s1.L1 L1.A
s1.empty s2.push in(B) >: s1.L1 L1.B

// stack at level 1
s1.L1 s2.push in(A) >: s1.L2 L2.A
s1.L1 s2.push in(B) >: s1.L2 L2.B
s1.L1 s2.pop L2.A >: s1.empty out(A)
s1.L1 s2.pop L2.B >: s1.empty out(B)

// stack at level 2
s1.L2 s2.pop L2.A >: s1.L1 out(A)
s1.L2 s2.pop L2.B >: s1.L1 out(B)
```

- **s1** holds current top stack state: **empty**, **L1**, or **L2**.  
- For each stack level, a separate state machine holds the token at that level.  

Example Operation:  

- Start at **s1.empty**. Receive **s2.push** and soft token **A** → move to **s1.L1**, set **L1.A**.  
- Pop → move back to **s1.empty**, emit **A**.  
- Push **B**, move to **s1.L2**, set **L2.B**.  

Two consecutive pops produce outputs **B**, then **A**, ending at **s1.empty**.

---

## Generic Stack Machines

The previous example fixed stack size and tokens. Consider expanding without preset limits.

### Notation with Subscripts

```
s2 >> s1
s1.L[i] s2.push >: s1.L[i+1] (s1.L[i+1] s2.pop >: s1.L[i])
```

- **s1.L[i]** denotes the i-th stack level; subscript is notation, not literal fixed points.  
- If **s1.L[i+1]** doesn't exist, the system creates it dynamically during execution.  
- There are virtually unlimited points available for new stack levels.  

### Temporary Rule Mechanism

The parentheses define a **temporary rule**, a short-term memory mechanism:  

- When **s1.L[i] s2.push** matches, instantiate the temporary rule:  
  ```
  s1.L[i+1] s2.pop >: s1.L[i]
  ```  
- This creates two rules:  
  1. `s1.L[i] s2.push >: s1.L[i+1]`  
  2. `s1.L[i+1] s2.pop >: s1.L[i]`  

- Together, these implement **push-pop** connections between stack levels.  
- If in state **s1.L[i]** and receive **s2.push**, move to **s1.L[i+1]**.

---

*End of formatted section.*

## 16 The Details of Temporary Rules

The details of **temporary rules** are more complex than we can deal with in this work. The **simplification** above captures the essence of the idea.

## 158 Correlithm Object Technology

### Stack State Machine Enhancement

- We have added a level to the **stack’s state machine**.
- If we are in state **s1.L[i+1]** and we receive the command **s2.pop**, then we move to state **s1.L[i]**.

### Commands and Stack Levels

- Every time the **s2.push** command appears, it establishes:
  - A new state or stack level represented by a new, never-before-generated **pattern point** in **s1**.
  - A new rule to use the **s2.pop** command to navigate back to the previous state or stack level.
- Once the **s2.pop** command is issued at a given level, as in the second line of code above:
  - The new pattern point **s1.L[i+1]** is abandoned.
  - The new rule is not deleted, but it will never again be activated because there are so many **random pattern points** available in **s1** that the same point or even one nearby will never again be generated.

### Generating a Stack with Arbitrary Levels and Tokens

Now that we can generate a **stack with an arbitrary number of levels**, let us consider how to add an **arbitrary number of tokens** at each level.

```plaintext
s1.L[i] s2.push >:    # the source side of the rule
s1.L[i+1] S[i+1].L[i+1](state)    # the target side of the rule
(s1.L[i+1] s2.pop >: s1.L[i] S[i+1].L[i+1]);    # the temporary rule
```

- Here we have added the **space S code** to the previous code.
- In the second line, we have added:  
  **s[i+1].L[i+1](state)**

### Significance of (state)

- This instructs the **CO system** to capture the **current state point** in space **S[i+1]** and give it the name **S[i+1].L[i+1]**.
- We could have left **(state)** off, and the **CO system** would simply have generated a **random pattern point**.
- We added **(state)** because it is frequently necessary in a **CO system** to capture the current **state point** and embed it in a rule. This is one way to do that.

## 17 Physical Implementation of a CO Machine

In a physical implementation of a **CO machine**, there are often means to select subsets of rules, deactivate, or even delete rules. Otherwise, the constant evaluation of all rules places an increasing burden on the machine the longer a stack is used. In the abstract, a **CO system** does not need to delete such rules.

Notice that a random space, **S[i+1]**, is selected to illustrate it can be done, though it is not needed for a simple stack. This guarantees the state point will be a random point in the space, avoiding non-random placement which may or may not be desirable.

A **“temporary” rule** watches for **s2.pop** to appear in the context of state **s1.L[i+1]** and when it does, the previous state **s1.L[i]** appears. The rule nominates the state point captured in **S[i+1]**.

Thus, we can build a stack of arbitrary depth holding an arbitrary set of tokens. This also shows a way to capture the current state point in some part of a **CO system**, which is important for systems that learn because state points produced by sensors or high-level parts of the system cannot be predicted.

Rule sets are **concurrent** and **superposed**; a small set of rules may drive many co-existing behaviors at different strengths and instantiation levels.

We can build an arbitrary **random-access memory** using similar methods: picking a random space implements one “storage location,” and instantiating a state point “loads” that location.

A **CO system** has large storage capacity due to:
- The large number of usable points in a space
- Many available spaces given total dimensions (N) and average dimensions used per space (M)
  
Superposition allows overlapping spaces extensively, increasing available spaces combinatorially.

---

## Expert Systems

An **expert system** can be more robust and generalized if implemented as a **CO system**.

- Expert systems, sometimes called **rule-based**, **decision trees**, or **if-then-else** systems, ask questions and make decisions sequentially.
- Example expert system about lunch:

  ```
  If hungry then
    If prefer soup then eat soup
    Else eat sandwich
  ```

- Expert systems capture human expert knowledge but do not generalize well, likened to a locomotive bound to rails.

### CO System Implementation Example

Rules for the example expert system as a CO system:

```
s1 >> s2
s1.not_hungry >: s2.do_not_eat
s1.hungry >: s2.soup s2.sandwich
s2.do_not_eat >: s2.do_not_eat
s2.soup >: s2.soup
s2.sandwich >: s2.sandwich
```

Starting with **s2** latched to **s2.do_not_eat** and **s1** random:
- Near **s1.hungry**, **s2** state is a balanced superposition of **s2.soup** and **s2.sandwich**, unable to decide initially.
- Small noise injected by the CO system breaks this balance, making the state point choose either soup or sandwich.
- The attractor rules cause the state to rapidly move to the nearest choice.

This noise-driven decision varies day to day, which is similar to techniques used in traditional expert systems but not exceptionally distinctive.

The **concurrency** and **superposition** properties allow exhaustive search of decision trees with gray-scale decisions, resulting in a quantitative best-fit, or “fail-soft” decision-making.

With partial states like “moderate hunger,” the system produces varying decisions and strengths of choice, still influenced by noise.

---

## Continuous Systems

**Correlithm object technology** is inherently continuous, enabling straightforward implementation of continuous functions, differential and integral equations, and feedback control systems.

The approach allows mixing continuous and discrete component implementations (e.g., NAND gates, flip-flops).

### Functional Relationships

Arbitrary continuous or piecewise continuous functions can be built by exploiting interpolation inherent in rules.

Example linear interpolation:

```
s1.p1 >: s2.q1
s1.p2 >: s2.q2
```

- If **s1.state** moves fraction **f** along the line between **s1.p1** and **s1.p2**, the selected **s2.state** moves along the line between **s2.q1** and **s2.q2** proportionally to **f**.
- This linear mapping is described by a formula of the form **m*f + b**.

For non-linear functions, multiple points in a string CO and a **floor mechanism** (weight cutoff based on distance) allow piecewise linear approximations.

Example extension including sensors and actors:

```
in(0) >: s.p1 >: t.q1 >: out(0)
in(1) >: s.p2 >: t.q2 >: out(1)
in(2) >: s.p3 >: t.q3 >: out(4)
in(3) >: s.p4 >: t.q4 >: out(9)
```

- The mapping produces piecewise linear interpolation related to points **0, 1, 2, 3** mapped to **0, 1, 4, 9**.

Alternatively, **quantization** can create discrete “snap” mappings with sharply bounded regions, resembling traditional digital states but less useful for interpolation.

### Differential Equations

- Differentiators can be built using delays in rules mapping previous states to current states with time delay **∆t**.
- Integrators similarly use rules that map the current and previous integral to the next value.
- Rules execute effectively continuously on suitable hardware, not necessarily in discrete cycles.

### Feedback Control Systems

- Arbitrary feedback control systems can be constructed by assembling CO system components like differentiators, integrators, adders, and multipliers.

---

## Artificial Neural Networks

- Neural networks map space **A** to space **B** via an unknown function **F()**.
- A training phase estimates **F'()** from a training set **T** of input-output pairs.
- Execution phase uses **F'()** for mapping new points.

### Similarities and Differences with CO Systems

- Both map points between spaces.
- Artificial neural networks focus heavily on obtaining **F'()** with internal hidden layers.
- **CO systems** use **correlithm objects** and random point statistics fundamentally.
- Artificial neural networks do not tokenize data as CO systems do.

### Implementation in CO Systems

Artificial neural networks can be represented by CO rules:

```
s1.b >: s2.a
```

- Points **s1.b** and **s2.a** correspond to training pairs.
- CO systems leverage simple neural network-like calculations internally but with faster training.

---

## Fuzzy Logic

- Developed by **Dr. Lotfi Zadeh** in the 1960s, fuzzy logic deals with **partial truth** values between 0 and 1.
- Uses **membership functions** describing concepts like "small," "medium," and "large."
- Appeals include natural language descriptions, simplicity, and adjustable control through overlaying variables and rules.

### Parallels with CO Technology

- Both use fuzzy or soft data representations and rules that map data.
- Membership functions in fuzzy logic resemble influence zones of point and string COs.

### Differences

- The unique **correlithm object** and associated statistics do not exist in fuzzy logic.
- Fuzzy logic is not tied to a specific system architecture as CO technology is.

Potential exists for fruitful collaboration between fuzzy logic and CO systems.

---

## Internal Control

- CO systems need different modes of operation (e.g., “floor” mode for selective rule weighting).
- Variation mechanisms can be implemented using rule-based recognition of situations.
- These rules can control an **actor** that modifies CO system parameters externally.

This simple feedback using an actor to modify system controls provides elegant internal mode switching.

---

## Learning

- Learning is essential: CO systems must create their own rules and states.
- Advanced learning theories, especially **reinforcement learning**, will augment CO technology.
- **Learning limits**:
  - Systems cannot learn to sense or act outside their physical converters or actors.
  - Learning adapts using available data and actions only.

### Example: Unfamiliarity Mechanism

Novelty detection concept:

```
# “novelty” detector
latch < input    # this “<” is not a mapping operator
latch.otherwise >: latch.otherwise
latch.otherwise >: novelty.novel
input.known1    >: novelty.known
input.known2    >: novelty.known
…
input.knownK    >: novelty.known
```

- The system flags inputs as **novel** if not near known points.
- This mechanism allows adding new rules for unfamiliar situations.

Learning can use any technique combined with CO systems, emphasizing small rule sets with significant benefit.

---

## Mind

- CO technology aims to build systems behaving like living systems, prioritizing flexibility over traditional computer precision.
- CO systems emerged from a breakthrough understanding of neural ensembles, reflecting properties similar to living minds.
- Rule pathways resemble thought processes, and CO results show behaviors aligned with living systems.
- Future intelligent systems will evolve hierarchically and exponentially.
- Science fiction of intelligent robots with “positronic pathways” parallels the current promise of correlithm objects.

---

## Chapter XI Directions

### Vision of CO Technology

- **Correlithm object technology (CO)** opens the door to a new **“Other Computer Industry”** where systems behave like living organisms.
- Past AI attempts largely failed due to mechanistic approaches.
- CO technology succeeds by modeling living neural systems and exploiting high-dimensional statistics.

### Why CO Technology Succeeds

- Breakthrough mathematical understanding of **ensemble states** in high-dimensional spaces.
- Emergence of behaviors from statistical properties of bounded spaces.
- CO objects represent data in surprising, non-intuitive ways, unlike simpler local logic models.

### Challenges Delaying Discovery

- High-dimensional geometry and statistics are counter-intuitive.
- Neurophysiology focuses on molecular details but misses global ensemble information representation.
- Computer science often inserts properties superficially rather than modeling emergent behaviors.
- The underlying mathematics, geometric probability, is mostly considered a niche field.

---

## Summary of Characteristics

### The Correlithm Object

- Core to CO technology and absent elsewhere.
- Enables unique behaviors seen in living neurons.

### Soft Tokens

- Represent data with variable **strength**.
- Provide “soft” or fuzzy data representation.

### String COs

- Represent **geometrical relationships**.
- Handle distorted or noisy inputs like spoken words or letters.

### Robustness

- Fault-tolerant operations at cell level.
- Error-correcting nature allows graceful degradation under noise.

### Sampling

- Subsets of CO objects still represent data adequately.
- Robustness diminishes with smaller samples.

### Superposition

- Multiple CO objects share dimensions simultaneously without loss of identity.
- Simplifies rule overlay and enables powerful data fusion.

### Straddlers

- CO systems can be distributed over thousands of nodes or robots.
- Exhibit group mind behavior, resistant to partial failures.

### Statistical “Holograms”

- CO systems resemble holograms with statistical distributed information.
- Built from cells rather than light interference.

### Virtual Systems

- CO systems can emulate any computing system virtually.
- Virtual machines running on **straddlers** are highly robust.

### Continuous Systems

- CO systems are inherently continuous, not discrete.
- Support all continuous functionality.

### General Purpose Analog Computers

- CO systems can be fully analog.
- Potential advantages: lower power, smaller size, easier fabrication, higher speed.

---

## Early Applications

- Tasks where animals or humans outperform traditional computers suggest CO applications.
- Expected near-term strengths include:

  - **Expert systems** with graceful degradation.
  - **Cueing systems** that prioritize data relevance for experts.
  - **Data mining** overcoming noise and semantic issues.
  - **Data fusion** combining multisensory inputs.
  - **Speech** recognition beyond current capabilities.
  - **Natural language** processing with improved meaning representation.
  - **Vision** systems inspired by living organisms.
  - **Handwriting** recognition with CO-based tools.
  - **Simple robots** with improved coordinated behaviors.
  - **Security**, especially anomaly detection in massive data.

---

## The Future

### Tools

- High-level abstraction and automation are critical.
- Existing tools for CO systems are early stage and need development.
- Formal high-level languages and graphical interfaces needed.
- Expectations for rapid progress driven by interest.

### Learning

- Learning is central to unlocking CO potential.
- Integration with reinforcement learning and CO-specific learning mechanisms is promising.
- Size and complexity may only be limited by computational resources.

### Quantum Information Systems

- **Correlithm objects** exist in quantum realms with qubits and ebits.
- Quantum COs survive measurement, sharing similar statistics in **Hilbert spaces**.
- Quantum spaces involve **bounded N-dimensional complex spaces** with unitarity constraints.
- Points lie on an **N-sphere** of unit radius, with unique geometric statistics.
- As dimensions grow, certain distance statistics converge to constants.

---

*End of formatted educational content.*

## 18 U.S. Air Force Contracts  

- **Contract Nos.**: F30602-02-C-0077 and F30602-03-C-0051.

## 19 Table II-1 Comparison  

- **Table II-1 in Chapter II** shows a comparison of **unit edge** versus **unit radius** statistics for the real unit **N-cube**.  
- Similar numbers are obtained for **QIS systems**.

## 192 Correlithm Object Technology  

All statistics in this book are considered from the perspective of a **unit radius** rather than a unit edge. All of the **CO statistics** still apply.

An interesting observation arises from this changed perspective:  
- All points in a bounded, high-dimensional space tend to be about the **same distance apart**.  
- When viewed as vectors, these points tend to be **orthogonal**.

We can pick two points at random, express them as vectors, and multiply (inner product operation) to get a result essentially **zero every time**. This means:  
- All points are nearly at **right angles** to each other through the midpoint of the space.  
- Sets of random points can be used as nearly **orthogonal basis vectors**.  
- Huge numbers of such basis vectors are available in any given **N-space**.  

If exact orthogonality is required, only **N orthogonal basis vectors** can be defined. However, if “essentially exact” orthogonality is sufficient (which it is in many cases), the available number is huge. This opens many issues and opportunities across fields.

We expect to publish at least one book devoted exclusively to **quantum correlithm objects**.

## Ensembles  

The **ensemble** is central to correlithm object technology. An ensemble is any group modeled using correlithm object concepts.

This book focuses on ensembles of **neurons**:  
- Each neuron has an **axonal or output pulse rate**, a real number varying over time between zero and one.  
- Applying correlithm object technology requires only an ensemble where each member independently produces its own version of the same **bounded variable**.  
- The set of values across the **N members** defines a point in a bounded **N-space**, causing correlithm object statistics to emerge.

These ensemble concepts apply clearly to **neurophysiological investigations and modeling**.  

We believe we have discovered the primary mechanism for the **representation, manipulation, and storage of information in neural systems**, but much work remains.

Major opportunities include:  
- **Neurodiagnostics**  
- **Neurostimulation**  
- **Prosthetics**

Questions posed:  
- Can these principles be established in the lab?  
- Will mysterious neural behaviors become less mysterious?  
- Will significant applications be developed?  
  
Our belief: **Yes**.

## Applications in Psychology  

At a macroscopic level, CO technology provides a new tool to model the **human mind**.

Questions include:  
- Can psychological issues be evaluated using CO mechanisms?  
- Will it be possible to test treatment alternatives and methodologies?  
- Can this help us understand the nature of **mind, emotion, and consciousness**?  

We believe so.

## Other Suitable Ensembles  

Suitable ensembles exist everywhere. We have linked **quantum information science** and CO technology:  

- For example, photons form an ensemble using **phase angle** as the unifying variable.  
- Recognizing this ensemble behavior led us into quantum information science.

Other particles also possess suitable bounded variables, such as electrons.  

Questions include:  
- What can CO technology reveal about systems based on photons or electrons?  
- What applications might emerge?  
- Does ensemble behavior assist as electronic devices approach scales dominated by quantum effects?  
- Are there benefits for **light**, **lasers**, or the emerging field of **nanotechnology**?  

We think yes.

## DNA and CO Technology  

Does DNA fit with CO technology? If living neural systems use high-dimensional statistical objects, might similar mechanisms appear elsewhere?

- Each DNA component can be one of four values, making a DNA string composed of **four-valued** or **two-bit elements**.  
- Treating a DNA pattern as a point in an **N-cube** where each dimension has four values means CO statistics emerge.  
- The entire DNA string forms a huge **N-cube**, with each dimension having 4 possible values.  

A particular type of DNA can be represented as a single point in the N-cube, representing the organism.

Since a living cell uses DNA as its primary **program**, CO technology might provide new possibilities in **DNA** and **living cell research**. We think so.

## Other Fields with Suitable Ensembles  

**Sociology** is another example:  

- Consider an ensemble of people.  
- Suppose an opinion spectrum is the ensemble variable.  
- Each person holds an opinion (possibly time-varying) at some strength within that spectrum.  
- This ensemble defines a correlithm object in the corresponding space.  

Because opinions influence others, mappings of COs constantly occur in human populations.

Question:  
- Can we model a kind of **group mind** across human ensembles and make predictions?  

We think yes.

## Conclusions  

This book introduces major concepts of **correlithm object technology**. The field is vast – exploring any direction reveals interesting results and possibilities.

We feel correlithm object technology is at a stage analogous to **computer science in the late 1940s**. However, technology development has accelerated since then.

We do not believe it will take a half-century for correlithm object technology to mature.

**Correlithm object technology** may be “the other computer industry.”

Living neural systems and traditional computers have vast differences in information processing. Neither excels at tasks the other handles well.

The theory presented here models **macroscopic information representation, storage, and manipulation** in living neurons, based on very simple first principles. Emergent properties align closely with those observed in living organisms.

## 20 Parallels with Psychohistory and Correlithm Object Technology

We cannot resist pointing out the parallels between this concept and the **“psychohistory”** described by **Isaac Asimov** in his *Foundation Trilogy*. The concept is simple and elegant. Many people find it very satisfying.

It produces insights and predictions that can be tested and verified, and also lead directly to engineering implementations that do new and useful things. We think that **correlithm object technology** will usher in a major new direction for information processing systems, namely systems that behave more like **living creatures** than machines. We call this direction **“the other computer industry.”**

Whether or not we are right that correlithm object technology **explains** how the brain works, it has already produced many new and powerful concepts. It provides a strong theoretical foundation for progress in many diverse directions. We have already begun to explore these new worlds — and there is no end in sight!

---

## Appendix I: Derivation of Unit N-Cube Distances

### Random Point to Random Point

#### The Problem

Find analytic expressions for the **mean** and **standard deviation** of a variable, **Z**, the Cartesian distance between two points selected at random within a **unit N-cube**.

#### The Solution

While closed-form expressions for the mean and standard deviation of **Z** as a function of **N** remain elusive, good expansions can be obtained.

Note that:

\[
Z^2 = \sum_{i=1}^N V_i^2
\]

where the \(V_i\) are copies of random variables **V**:

\[
V^2 = (X_1 - X_2)^2
\]

with \(X_1\) and \(X_2\) being uniform, independent random deviates on \([0,1]\). We have:

\[
E[V^m] = \iint_0^1 (x - y)^m dx\, dy = \frac{1}{(m+1)(m+2)}
\]

for all positive integers \(m\). In particular,

\[
E[V^2] = \frac{1}{6}
\]

Thus, if we knew the mean, the variance \(\sigma^2\) would be easy:

\[
\sigma^2 = E[Z^2] - (E[Z])^2 = \frac{N}{6} - (E[Z])^2
\]

For an approximation to \(E[Z]\), write the ansatz:

\[
Z = \sqrt{\frac{N}{6}} (1 - Y)
\]

where:

\[
Y = \frac{1}{\sqrt{6N}}
\]

Expansions and higher-order terms are available, and as \(N\) tends to infinity, the mean and standard deviation simplify to:

\[
E[Z] \approx \sqrt{\frac{N}{6}} \quad \text{and} \quad \sigma \approx \frac{7}{120}
\]

(Full detailed expansions and Mathematica® derivations are given in the original text.)

---

### Random Point to Midpoint

#### The Problem

Find analytic expressions for the mean and standard deviation of **Z**, the Cartesian distance between a point selected at random and the **midpoint** within a unit **N-cube**.

#### The Solution

The derivation follows the same lines as above, leading to expansions for the mean and standard deviation. As \(N \to \infty\):

\[
E[Z] \approx \frac{1}{12} \quad \text{and} \quad \sigma \approx \frac{1}{60}
\]

---

### Random Point to Corner

#### The Problem

Find analytic expressions for the mean and standard deviation of **Z**, the Cartesian distance between a random point and any **corner** within a unit **N-cube**.

#### The Solution

Similar derivations produce the expansions. As \(N \to \infty\):

\[
E[Z] \approx \frac{1}{3} \quad \text{and} \quad \sigma \approx \frac{1}{15}
\]

---

## Appendix II: Correlithm Objects and Neurophysiology

### Overview

**Correlithm Object (CO) technology** provides a framework for understanding the functionality of living **neural systems**. The dynamic state of a neuron is the **time-varying rate of pulses** found on its axon.

The state of a group of **N neurons** can be viewed as a point in a **unit N-cube**. Such points, or COs, have unique properties and are the primary tokens used to represent data in living neural systems.

Important tokens are stored in the synapses as CO pairings that capture the state CO of one group of neurons with respect to another at a particular time. These pairings cause the instantiation of a CO in one group of cells to instantiate the corresponding CO in another.

The model is **Turing complete**, capable of learning from experience, and incredibly **robust**. It exhibits many properties uncommon in traditional computers but widespread in living information processing systems.

---

### Representing Information

The state of a neuron is its **pulse rate**, a time-varying, continuous, bounded variable ranging from **zero** to about **200 pulses per second**. Normalized to the interval \([0,1]\), the **information in pulses is real and continuous**, not digital.

The state of a group of **N neurons** at time \(t\) can be represented as a point in a **unit N-cube** called a **Correlithm Object (CO)**.

---

### Storing Information

Information is stored as COs in a cell’s **input dendritic tree**. Each input cell tries to impose a particular pulse rate on the output cell, but not necessarily its own rate.

There is a learned functional relationship visualized as isolated **“spots”** on a photographic film (see Figure AII-1).

- A spot relates the **input pulse rate** of each input cell to a “nominated” output pulse rate.
- Spot size reflects **synaptic strength**.
- The input cell strongly influences the output cell only on or near these spots.
- When the input rate intersects a spot, the output rate is nominated.
- The nomination strength is proportional to spot size.

Aggregating all input nominations produces a **weighted average** determining the output cell’s pulse rate.

The CO in the input cells is compared to stored COs and their pairings to produce the output cell’s pulse rate (see Figure AII-2).

---

### A Closer Look

A **conical neuronal membrane**, such as a tapering dendrite, acts as a **spatial pulse rate analyzer** (Figure AII-3).

- Maximum sustainable pulse rate in a tubular membrane is proportional to tube diameter.
- In a conical membrane:
  - **Conduction region:** pulses pass fully.
  - **Transition region:** partial conduction.
  - **Cutoff region:** no conduction.

Pulses of different rates stop at different points along the cone, mapping pulse rate to physical position.

Only synapses in the **transition region** effectively transmit signals since:

- Cutoff region synapses have too little signal.
- Conduction region synapses have too much signal, overwhelming the target membrane.

Thus, only a small percentage of synapses transmit at any instant (Figure AII-5).

An active synapse stimulates its target membrane to pulse at the rate corresponding to the target membrane’s diameter, independent of the source pulse rate (Figure AII-6).

---

### Learning and Manipulating Information

Learning involves changing the conductivity of synapses. To retain a CO, a cell increases conductivity of only the currently active synapses, producing the “spots” pairing the incoming CO with the cell’s pulse rate.

Neurons manipulate information by **mapping correlithm objects to correlithm objects**.

- A neuron watches the CO output of a group of neurons.
- It compares this input CO with stored COs.
- Generates a new output pulse rate as a function of this comparison.
- If input CO is close to stored CO, output is near the stored output pulse rate.
- If several stored COs are close, output is a weighted average.
- If no close CO, output drifts randomly.

Outputs of a set of cells form a CO, mapping input COs to output COs.

CO mapping is **Turing complete** (demonstrated in Chapter VI). This process shows characteristics similar to living neural systems.

---

### Conclusion

This model represents data as COs in pulse rates across neurons, stores data by associating COs in dendritic trees, and manipulates them using these associations.

We encourage further study, understanding, and testing of this model in the field.

---

## Glossary

- **actor:** A converter that maps world data to COs.
- **Behavior:** The way a CO system functions or operates.
- **bit content:** See **information content**.
- **bounded N-space:** A space of **N dimensions** where every straight line is finite in length.
- **cardinal converter:** Rules mapping one-to-one between a cardinal set of world objects and same-space COs.
- **cardinal objects:** Objects with no order other than set membership.
- **cell:** Storage for the coordinate value of one dimension in a CO machine.
- **CO:** Correlithm object.
- **CO computing paradigm:** Computation using high-dimensional points from bounded N-spaces, manipulating data by known pathways.
- **CO field:** The region of space near a CO; also a **zone of influence**.
- **CO machine:** The computing machine implied by the CO paradigm.
- **CO system:** A computational structure that uses COs.
- **complementary error function:** Integral of the tails of the normal distribution.
- **concurrency:** Property allowing multiple processes at the same time.
- **continuous converter:** Converts between a continuous world variable and a string CO.
- **converter:** Means to get world data into/out of a CO system.
- **correlithm:** (1) A systematic problem-solving method based on known examples; (2) An algorithm based on characterizing examples.
- **correlithm object:** A point in a high-dimensional bounded space.
- **ensemble:** A group of N objects with properties representing coordinates in bounded N-space.
- **erfc:** Complementary error function.
- **fires:** One iteration of the execution of a rule.
- **full ply/full space:** Set of all dimensions defined in a CO system.
- **geometric probability:** Study of probabilities in geometric problems.
- **googol:** One followed by one hundred zeroes.
- **Hamming distance:** Number of differing bits between two points in binary space or sum of straight-line distances per dimension in N-space.
- **information content:** Number of bits corresponding to an event \(-\log_2 p(x)\).
- **inner product:** Sum of element-wise products of two vectors.
- **ladder:** A general model for behaviors in CO systems using hierarchical structures.
- **lobe:** A grouping of rules for hierarchical composition.
- **mechanism:** A set of rules running on a CO machine; a way to write rules for particular behavior.
- **midpoint:** The center of a unit N-cube equally distant from all corners.
- **N-cube:** Cube with N dimensions.
- **N-hedron:** Set of points in bounded N-space mutually about the same distance apart; also called an N-sphere.
- **N-space:** Space with N dimensions.
- **N-sphere:** See **N-hedron**.
- **N-torus:** A torus with N dimensions.
- **name:** Textual label for referring to spaces or points.
- **ordinal converter:** Maps ordered set of world objects to string CO.
- **ordinal objects:** Objects with a particular order (e.g., alphabet).
- **orthogonal basis vectors:** Linearly independent vectors in the same space.
- **pattern CO:** A known point in a space.
- **part:** A reusable subsystem or set of rules defined as a single object.
- **pattern point:** A known reference point or landmark.
- **ply:** A set of dimensions; a space or subspace.
- **point:** A set of coordinates defining a location in a space; often synonymous with CO.
- **point capacity:** Number of points that can be selected at random ensuring no two are within a specific distance.
- **probability of proximity:** Probability two random points lie within distance \(d\).
- **pulse rate:** Number of pulses per unit time.
- **quantized converter:** Treats world data as having specific discrete values.
- **random corner:** A random corner of an N-cube.
- **random point:** A random point in an N-cube.
- **real unit N-cube:** Unit N-cube with coordinates from real numbers.
- **rule:** Directed line segment from a source set of soft tokens to target tokens for data manipulation.
- **rule execution:** The process of rule evaluation.
- **rule notation:** Formalism for writing CO system rules.
- **rule operator:** Symbol (">:" or ":<") separating source and target sides of a rule.
- **sampled rule:** Rule with points subset of another rule’s points; lower resolution.
- **sensor:** Converter mapping world data to COs.
- **soft token:** A CO representing data.
- **source:** Portion of rule specifying what must be matched.
- **space:** Set of dimensions hosting all subspaces in a CO system.
- **stack machine:** Computational formalism with state machine plus stack memory.
- **standard diameter:** Expected distance through midpoint between two random points.
- **standard distance:** Expected distance between two randomly selected points.
- **standard radius:** Expected distance between midpoint and a random point.
- **state CO:** Current coordinate values in cells or corresponding point; varies with time.
- **state machine:** Computational formalism with only current state memory.
- **state point:** A state CO.
- **straddler:** CO system running on concurrent computing resource.
- **string:** Short for string CO.
- **string CO:** Set of COs with lower-dimensional geometric relationships.
- **subply:** Ply that is a subset of a larger ply.
- **subspace:** Space subset of a space with more dimensions.
- **superposition:** Property where multiple rules execute concurrently and correctly in the same space, or combined CO representing multiple COs.
- **swarm:** Type of straddler with physically mobile components, like insects.
- **swarm mind:** Behavior of CO system running on a swarm.
- **symmetric bounded N-space:** Bounded N-space with origin at midpoint.
- **Synthorg®:** CO system including converters; trademark of Lawrence Technologies LLC.
- **target:** Portion of a rule specifying what is generated.
- **term:** Element in rule source or target specifying point in space.
- **unit edge:** Normalization where each edge of N-cube has unit length.
- **unit N-cube:** N-cube with unit length edges.
- **unit radius:** Normalization where standard radius has unit length.
- **world:** Anything outside the CO system.
- **world data:** Data in the environment of a CO system.
- **zone of influence:** Region near a CO; also called CO field.

---

## Index Highlights

- **Actor:** 52, 119-127, 132-133, 154-156, 166, 174-176, 211
- **Analog:** 96, 100, 105-108, 123, 163, 186
- **Artificial Neural Network:** 4, 73, 171, 172, 186
- **Attractor:** 77, 148-152, 161
- **Axon:** 182, 203
- **Bit Content:** 27, 31, 32, 211
- **Brain:** 5, 98, 116, 132-133, 195
- **CO Architecture:** 69, 73
- **CO Computing Paradigm:** vii, 37-42, 45, 47, 50-53, 63, 112, 133-134, 151, 163, 171, 178, 184, 211, 216
- **Concurrency:** 99-105, 146, 151, 162-163, 172, 185, 190, 212
- **Dendritic Tree:** 7, 204, 209, 210
- **Geometric Probability:** 9, 10, 25, 90, 183, 213
- **Hamming Distance:** 9, 10
- **Information Content:** 27, 38, 110-111, 152, 162
- **Learning:** 159, 175-177, 190, 203, 209
- **Midpoint:** 9, 11-22, 40, 191-192, 200, 218-219
- **N-Cube:** 9-13, 15-18, 21
- **Neurophysiologists:** 182
- **Pulse Rate:** 6-7, 182, 192, 203-210
- **Rule:** 41, 57-59, 71, 101-102, 110, 116, 118, 171, 174, 192
- **State CO:** 46, 60, 100, 113, 122-127, 203, 218
- **Superposition:** viii, 97, 111-113, 127, 139-141, 146, 151-152, 159, 161-163, 172, 185, 219
- **Swarm:** 5, 97, 104, 118, 185
- **Synapse:** 206-210

(For full index, refer to the original document.)