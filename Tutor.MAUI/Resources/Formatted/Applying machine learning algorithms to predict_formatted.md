---
title: Applying machine learning algorithms to predict
source: Applying machine learning algorithms to predict.txt
formatted: 2026-02-08T23:04:22.6422593Z
---

## Applying Machine Learning Algorithms to Predict the Stock Price Trend in the Stock Market – The Case of Vietnam

**Tran Phuoc¹², Pham Thi Kim Anh¹², Phan Huy Tam³⁴ & Chien V. Nguyen⁵✉**

---

The aims of this study are to **predict the stock price trend** in the stock market in an **emerging economy**. Using the **Long Short Term Memory (LSTM)** algorithm, and the corresponding **technical analysis indicators** for each stock code including:

- **Simple Moving Average (SMA)**
- **Moving Average Convergence Divergence (MACD)**
- **Relative Strength Index (RSI)**

and the secondary data from **VN-Index** and **VN-30** stocks, the research results showed that the forecasting model has a **high accuracy of 93%** for most of the stock data used. This demonstrates the appropriateness of the **LSTM model** and test set data used to evaluate the model’s performance.

The forecasting model's high accuracy of **93%** illustrates its effectiveness in analyzing and forecasting stock price movements on the **machine learning platform**.

---

¹ Ho Chi Minh City University of Food Industry, Vietnam  
² Faculty of Finance and Accounting, Hochiminh City University of Industry and Trade  
³ University of Economics and Law, Ho Chi Minh City  
⁴ Vietnam National University, Ho Chi Minh City  
⁵ Institute of Graduate Studies, Thu Dau Mot University, Binh Duong Province, Vietnam  
✉ email: chiennv@tdmu.edu.vn

---

## Introduction

Predicting the **future direction of stock prices** has attracted much interest from researchers and investors. Due to the **varied and wide factors and sources of information**, predicting future stock market price behavior is very difficult. It is evident that stock prices **cannot be accurately predicted**.

Several studies address this challenge using various approaches (Appel, 2005; Brown et al., 1998; El-Nagar et al., 2022; Fromlet, 2001). For example:

- Sen and Chaudhuri (2016) and Sen (2017) use **time series decomposition** to forecast stock prices with potential accuracy.
- Short-term stock price forecasting with **machine learning and deep learning algorithms** also shows very high results (Sen and Chaudhuri, 2016; Sen & Datta Chaudhuri, 2018).
- Mehtab and Sen (2019) confirm the strong and reliable **stock price prediction ability** of machine learning models, both regression and classification.

By analyzing users’ emotions on social networks, using the **self-organizing fuzzy neural network (SOFNN)** algorithm, researchers demonstrated high accuracy in predicting the **NIFTY index**. Additionally, the **compound neural network (CNN)** algorithm was used for time series forecasting with high accuracy (Mehtab and Sen, 2020).

---

### Machine Learning and LSTM in Stock Price Prediction

A popular approach applies **machine learning algorithms** to learn from historical price data to predict future prices. The **scale** demonstrates predictive power on historical stock price data, outperforming other methods due to its data suitability.

- **Recurrent neural networks (RNNs)** have **short-term memory**.
- The **LSTM algorithm (Long Short-Term Memory)** confirms stability and efficiency in **short-term stock price forecasting**.
- LSTM distinguishes and synthesizes **short-term and long-term factors** by weighting parameters and skipping irrelevant memory.
- It can handle **long input sequences** better than traditional RNNs (Nelson et al., 2017; Pahwa et al., 2017).
- LSTM introduces the **memory cell** that replaces traditional neurons in hidden layers.
- Networks efficiently link **memories and inputs just-in-time**, suitable for capturing **time-flexible data structures** with high predictability (Chen et al., 2015).

---

### Technical Analysis and Stock Price Forecasting

Short-term predictive analysis often combines **machine learning and technical analysis**. Several stock price technical analysis indicators and patterns are used:

- **Bollinger Band**
- **Moving Average Convergence Divergence (MACD)**
- **Relative Strength Index (RSI)**
- **Moving Average (MA)**
- **Stochastic Momentum (MS)**
- **Meta Sine Wave (MSW)**
- Patterns such as **head and shoulders**, **triangle**, **flag**, **Fibonacci fan**, and **Andrew’s pitchfork**

These indicators help investors detect **stock volatility patterns** leading to better investment returns (Nelson et al., 2017; Borovkova and Tsiamas, 2019).

Alternate time series techniques like the **difference in difference (DID)** method (Trinh et al., 2021) or **non-linear autoregressive distributed lag** (Le et al., 2022) exist. However, the **LSTM model** is more appropriate for **short-term stock price forecasting** in the machine learning domain and has been rarely used in the Vietnamese market.

---

### Vietnamese Stock Market Context

- Established in 2000, rapid development with market capitalization of **82.15% of GDP**.
- Number of accounts: **6.98 million** domestic investors and **38,897** foreign investors (Hoang, 2023).
- Predicting stock prices is vital for effective **investment portfolios**.

No prior studies have tested the effectiveness of **LSTM combined with technical analysis indicators** in the Vietnamese market.

---

### Study Objective and Structure

- Evaluate applicability of **cyclic neural networks, especially LSTM**, combined with **technical analysis indicators** for price volatility prediction.
- Assess performance on **VNindex** data and **VN30** stock group using accuracy and other metrics.

The paper is structured as follows:

- Section **Theoretical Basis** – Discusses the theoretical background.
- Section **Data Collection and Research Methodology** – Details data and methods.
- Section **Research Results and Discussions** – Presents results.
- Section **Conclusions** – Summarizes findings.

---

## Theoretical Basis

### Foundation Theory

The **predictability of stock returns** has been debated extensively. Two main hypotheses are:

- **Efficient Market Hypothesis (Fama, 1970)**: Current asset prices reflect all prior information immediately.
- **Random Walk Hypothesis**: Stock price changes are independent of history; tomorrow’s price depends only on tomorrow’s information, not today’s price (Burton, 2018).

These suggest **no accurate method exists** for predicting stock prices.

Conversely, some researchers argue that **stock prices can be partly predicted**. Various methods have been explored across economics, statistics, physics, and computer science (Lo and MacKinlay, 1999).

---

### Technical Analysis Indicators

**Technical analysis** models and predicts the stock market based on **historical market data**, primarily price and volume. It follows these assumptions:

1. Prices are determined exclusively by **supply and demand** relationships.
2. Prices change with the **trend**.
3. Changes in supply and demand cause trend **reversals**.
4. Changes in supply and demand can be **identified on the chart**.
5. Chart **patterns tend to repeat**.

Technical analysis does **not** consider external factors like politics, social events, or macroeconomics (Kirkpatrick & Dahlquist, 2010).

Research shows that **short-term trading strategies** based on technical analysis indicators can outperform traditional methods such as **MACD** and **RSI** (Biondo et al., 2013).

It is widely recognized as a method for generating **buy or sell signals** based on price information, ranging from simple moving averages to complex time series pattern recognition.

- Brock et al. (1992) showed that **simple trading rules** using short-term and long-term moving averages have significant predictive power on the **Dow Jones Industrial Average** over a century.
- Fifield et al. (2005) found markets like **Greece, Hungary, Portugal, and Turkey** to be information inefficient, unlike more advanced European markets.

Though criticisms exist regarding **data bias** (Brock et al., 1992), technical analysis remains an important forecasting tool.

---

### Long Short Term Memory (LSTM) Algorithm

The **Recurrent Neural Network (RNN)** was proposed by Elman (1990) to process **sequence data** such as text, voice, and video, where each sample depends on the previous one.

Examples include:

- Text: a word related to preceding words.
- Meteorological data: daily temperature relations to prior days.

RNN processes sequences, which fits the **time series data properties in stock analysis**.

---

### Figure 1: Structure of RNN

The output of the RNN's **hidden layer** is stored in **memory**, enabling the network to remember previous inputs for sequence analysis.

## Recurrent Neural Networks (RNNs) and Memory

Memory can be thought of as another **input** in recurrent neural networks (**RNNs**). The main challenge in training RNNs is the passing of the hidden layer parameter **ω**.

Since the **error propagation** in the RNN is not handled properly, the value of **ω** multiplies during both forward and reverse propagation, leading to two main problems:

- **Gradient Vanishing**: When the gradient is small and decreases exponentially, it has almost no effect on the output.
- **Gradient Exploding**: Conversely, when the gradient is large and increases exponentially, it leads to gradient explosion.

These problems exist in any **deep neural network** but are especially evident in RNNs due to their recursive structure.

Unlike traditional relay networks, RNNs can transmit data not only forward but also to previous layers or the same class, enabling them to have **short-term memory** in addition to the long-term memory acquired through training.

---

## Long Short Term Memory (LSTM)

The **Long Short Term Memory (LSTM)** algorithm, introduced by **Hochreiter and Schmidhuber (1997)**, aims to provide better performance by solving the **Gradient Vanishing problem** experienced by RNNs with long sequences of data.

### Structure and Functionality

- Each neuron in LSTM is a **“memory cell”** connecting previous information to the current task.
- LSTM is a special type of RNN that can capture error and move it back through the layers over time.
- It maintains the error at a certain maximum constant, allowing long training periods and parameter adjustments (**Liu et al. 2018**).

### Gateway Structures

An LSTM unit contains three "gateway" structures:
- **Input Gate**
- **Forget Gate**
- **Output Gate**

Information that matches the algorithm rules is forwarded via the input and output gates. Information that does not match is discarded via the forget gate.

This **gate-based architecture** allows selective forwarding of information based on the **activation function** of the LSTM network.

### Applications and Performance

- LSTM networks are widely used and have shown positive results compared to other methods (**Graves, 2012**).
- Particularly effective in **Natural Language Processing** and **handwriting recognition** (**Graves et al. 2008**).
- Despite many variations of LSTM algorithms, none have significantly improved upon the original to date (**Greff et al. 2016**).

---

## Experimental Study: Stock Market Data Analysis

### Characteristics of Stock Market Data

Stock market data is:
- Very large
- Non-linear in nature

Modeling such data requires deep learning algorithms that can identify and exploit hidden information through **self-learning**.

Deep learning models efficiently handle this type of data (**Agrawal et al. 2019**).

### Neural Network Models in Financial Time Series

- Some studies use only a **single time series** input (**Jia, 2016**).
- Others incorporate indicators showing market information and **macroeconomic variables** (**White, 1988**).
- Variations include combining financial time series with **natural language data** (**Ding et al., 2015**), and deep learning architectures modeling **multivariable financial time series** (**Roman and Jameel, 1996; Heaton et al., 2016**).

### Specific Studies and Techniques

- **Chan et al. (2000)** predicted the Shanghai stock market using neural networks with technical analysis variables, improving back-propagation efficiency through conjugate gradient learning.
- Research using RNN models for stock analysis and forecasting is extensive:
  - **Roman and Jameel (1996)** predicted stock indexes for five markets.
  - **Saad, Prokhorov, and Wunsch (1998)** used delay time, recurrence, and probability neural networks for daily stock prediction.
  - **Hegazy et al. (2014)** applied machine learning algorithms like PSO and LS-SVM to forecast the S&P 500 stock market.

### Incorporation of LSTM in Financial Predictions

- LSTM efficiently handles time-dependent data and is widely used in **stock price prediction** (**Heaton et al., 2016**).
- Combining LSTM with **Natural Language Processing (NLP)** using news data improves prediction of price trends.
- Various studies use:
  - Historical price data for predicting stock price movements (**Chen et al., 2015**).
  - Price data along with stock indices for daily price movement prediction (**Di Persio and Honchar, 2016**).
  - Comparison of LSTM with other combined algorithm methods (**Pahwa et al., 2017**).

### Sentiment Analysis Integration

- **Zhuge et al. (2017)** combined LSTM with **Naive Bayes** to extract market emotional factors, improving predictive performance.
- Sentiment analysis integrated with LSTM predicts stock opening prices effectively.

### Further Research

- **Jia (2016)** demonstrated LSTM’s effectiveness in stock return forecasting.
- Combined **real-time wavelet transform** and LSTM to predict the East Asian stock index, correcting logic defects in earlier studies with improved accuracy.
- **Gülmez (2023)** noted LSTM is suitable for financial time series, especially stock prices based on supply and demand.
- Research on the **Dow Jones stock index** forecasted market trends from 2019 to 2023.
- **Usmani Shamsi (2023)** studied the Pakistan stock market and the influence of general, industry, and stock-related news on price forecasts.

These confirm the increasing use of the **LSTM model in stock price forecasting**.

---

## Data Collection and Research Methodology

### Data Collection

The research applied the **LSTM algorithm** with **technical analysis indicators** to forecast price trends on the **Vietnamese stock market**.

- Data included **price history** of **VN-Index** and **VN-30** group stocks.
- Data sourced from website **vietstock.vn**.
- The study selected **31 enterprises**, mostly from the VN-30 group, representing large market-cap and high liquidity stocks.
- Historical price data ranged from listing date to **April 1, 2021**, covering the pandemic period for impact assessment.
- Stocks have different starting dates, so data lengths vary.
- Classification followed the **GICS classification system**.

### Collected Data Indexes

- Closing price
- Opening price
- Highest price
- Lowest price
- Trading volume per trading session

### Data Processing Steps

1. Check and handle defective data such as empty data or deviations.
2. Calculate **technical analysis indicators**:
   - Simple Moving Average (**SMA**)
   - Convergence Divergence Moving Average (**MACD**)
   - Relative Strength Index (**RSI**)
3. Aggregate historical price data with technical indicators; eliminate observations with lacking data.
4. Use aggregated data as input for the **LSTM model** to forecast stock prices.

### Tools Used

- **Microsoft Office Excel** and **Python** for calculating indicators and data processing.
- LSTM model built using **Sklearn**, **Keras**, and **Tensorflow** libraries.

---

## Research Methodology: LSTM Model Construction

- Data split into **training set** and **testing set**:
  - Training set: from stock listing start to **December 31, 2020**
  - Test set: from **January 1, 2021** to **April 1, 2021**
- Training data used to train LSTM; test data used to evaluate model performance.
- Independence between training and testing data ensures objectivity.

### Model Advantages

- Processes **daily data**, allowing measurement of daily stock price fluctuations.
- Controls accuracy through prediction accuracy.

### Model Parameters

- **Step coefficient** set as 60:
  - Uses previous 60 days’ data to forecast the next day’s stock price.
- LSTM model architecture includes 4 layers:

  - **Layer 1**: units = 30, activation = ‘relu’, Dropout(0.1), input shape matches data size.
  - **Layer 2**: units = 40, activation = ‘relu’, Dropout(0.1).
  - **Layer 3**: units = 50, activation = ‘relu’, Dropout(0.1).
  - **Layer 4**: units = 60, activation = ‘relu’, Dropout(0.1).

The model is implemented on the **Python** platform.

## 3 Model Compilation and Training

Figure 3 indicates that the **model is compiled** with the following specific coefficients:  
- **optimizer** = ‘adam’  
- **loss** = ‘mean_squared_error’

Next, **fit the model** with the following coefficients:  
- **epochs** = 1000  
- **batch_size** = 32  

The model will loop 1000 times to correct the coefficient of fit. However, to reduce computational complexity and ensure feasibility in model building, the author uses **EarlyStopping** with the following coefficients:  
- **monitor** = ‘loss’  
- **patience** = 8  
- **restore_best_weights** = True  

After building the **LSTM model** with the training set data, the model will forecast stock prices for the test set observations, covering trading sessions from **January 1, 2021** until **April 1, 2021**. The forecasted stock prices will be compared with the **actual stock prices** of the respective trading sessions.

The **accuracy** of the model’s prediction is evaluated based on the formula:  

\[ A_j = \frac{\sum_{i=0}^n |P_{ij} - V_{ij}|}{n} \]  

Where:  
- **Aj**: The accuracy of the model’s forecast for share j  
- **Vij**: Actual closing price of share j at the ith trading session in the test set  
- **Pij**: Forecast result for price of stock j at the ith trading session  
- **n**: Number of sessions in the test set  

The prediction accuracy of the **LSTM model** will be compared with the **baseline value of 93%**. According to trading regulations on the **Ho Chi Minh City Stock Exchange, Vietnam**, the **maximum fluctuation range of stock prices** in one trading session is **7%**. Thus, if forecasting stock price by the simplest method (today’s price = yesterday’s price), the error rate is 7%, and the baseline accuracy is 93%. If the model’s accuracy is below 93%, the model is deemed inefficient.

---

## Table 1: List of Stock Codes Used in the Study

| No. | Code | Company’s Name                                               | Branch       | Listing Date |
|------|-------|--------------------------------------------------------------|--------------|--------------|
| 1    | VN-Index | VN-Index                                                     | N/A          | 20/07/2000   |
| 2    | BID    | Bank for Investment and Development of Vietnam               | Finance      | 24/01/2014   |
| 3    | BVH    | Bao Viet Group                                               | Finance      | 25/06/2009   |
| 4    | CTG    | Bank of Industry and Trade of Vietnam                        | Finance      | 16/07/2009   |
| 5    | FPT    | FPT Corporation                                             | Technology   | 13/12/2006   |
| 6    | GAS    | Vietnam Gas Corporation                                      | Utilities    | 21/05/2012   |
| 7    | HDB    | Ho Chi Minh City Development Commercial Bank                 | Finance      | 05/01/2018   |
| 8    | HPG    | Hoa Phat Group Joint Stock Company                           | Manufacturing| 15/11/2007   |
| 9    | KDH    | Khang Dien House Trading and Investment Joint Stock Company | Build        | 01/02/2010   |
| 10   | MBB    | Military Commercial Joint Stock Bank                         | Finance      | 01/11/2011   |
| 11   | MSN    | Masan Group Joint Stock Company                              | Manufacturing| 05/11/2009   |
| 12   | MWG    | Mobile World Investment Joint Stock Company                  | Retail       | 14/07/2014   |
| 13   | NVL    | Nova Real Estate Investment Group JSC                        | Build        | 28/12/2016   |
| 14   | PDR    | Phat Dat Real Estate Development Joint Stock Company         | Build        | 30/07/2010   |
| 15   | PLX    | Vietnam National Petroleum Corporation                        | Oil and Gas  | 21/04/2017   |
| 16   | PNJ    | Phu Nhuan Jewelry Joint Stock Company                        | Manufacturing| 23/03/2009   |
| 17   | POW    | PetroVietnam Power Corporation                               | Utilities    | 14/01/2019   |
| 18   | REE    | Refrigeration Mechanical and Electrical Joint Stock Company | Build        | 28/07/2000   |
| 19   | SBT    | Thanh Thanh Cong Joint Stock Company - Bien Hoa              | Manufacturing| 25/02/2008   |
| 20   | SSI    | SSI Securities JSC                                           | Finance      | 29/10/2007   |
| 21   | STB    | Saigon Thuong Tin Commercial Joint Stock Bank               | Finance      | 12/07/2006   |
| 22   | TCB    | Vietnam Technological and Commercial Joint Stock Bank       | Finance      | 04/06/2018   |
| 23   | TCH    | Hoang Huy Financial Services Investment Joint Stock Company | Build        | 05/10/2016   |
| 24   | TPB    | Tien Phong Commercial Joint Stock Bank                       | Finance      | 19/04/2018   |
| 25   | VCB    | Joint Stock Commercial Bank for Foreign Trade of Vietnam    | Finance      | 30/06/2009   |
| 26   | VHM    | Vinhomes Joint Stock Company                                 | Build        | 17/05/2018   |
| 27   | VIC    | Vingroup Company                                            | Build        | 19/09/2007   |
| 28   | VJC    | Vietjet Aviation Joint Stock Company                         | Carriage     | 28/02/2017   |
| 29   | VNM    | Vietnam Dairy Products Joint Stock Company                   | Manufacturing| 19/01/2006   |
| 30   | VPB    | Vietnam Prosperity Joint Stock Commercial Bank              | Finance      | 17/08/2017   |
| 31   | VRE    | Vincom Retail JSC                                           | Build        | 06/11/2017   |

*Note: More information on these stocks can be found in the Appendix.*

---

## Research Results and Discussions

When applying the **LSTM algorithm** and **technical analysis indicators** to forecast price trends on the **Vietnamese stock market**, the authors present results from the analysis according to the research process and method.

### Industry Group Classification of VN-30 Stocks

Figure 4 shows that most stocks in the **VN-30 group** belong mainly to:  
- **Finance**  
- **Construction**  
- **Manufacturing**

These industries have high **corporate capitalization**, explaining why many companies from these groups were selected for the study.

### LSTM Model Forecasting Performance

The **LSTM model predicts stock prices** for the test set observations, from **January 1, 2021, to April 1, 2021**, covering **78 trading sessions**.

- Forecast accuracy varies by stock ticker.
- Figure 5 shows the **forecast results for the VN-Index**, revealing that the forecast price closely follows the actual price trend.
- Differences between forecasted and actual prices are generally not significant.

### Model Accuracy Results

Figure 6 shows the **accuracy level of the LSTM model** for each stock in the research list:  
- The **red horizontal line** represents the baseline accuracy at **93%**.  
- The LSTM model achieves forecast accuracy **higher than 93% for most stocks**.  

Highest accuracy examples include:  
- **PNJ**: 97.7% (Fig. 7)  
- **MSN** and **TPB**: Approximately 97% (Figs. 8 and 9)  

Other stocks with lower forecast accuracy include:  
- **BID**, **BVH**, **CTG**, **GAS**, **HDB**, **HPG**, **KDH**, **MBB**, **MWG**, **PDR**, **REE**, **SBT**, **SSI**, **STB**, **TCB**, **VIC**, **VJC**, **VNM**, **VCB**, **VHM** (Figs. 10–29).

The comparison chart for **PNJ** shows very high similarity between forecast and actual prices, explaining the high prediction accuracy.

However, some stocks achieved lower accuracy, such as:  
- **NVL**: 78.9%  
- **TCH**: 86.8%  
- Approximately **89%** for **FPT**, **PLX**, **POW**, **VPB**, and **VRE** (Figs. 30–36).

### Analysis of Lowest Forecast Accuracy Cases

- The **NVL** stock shows stable prediction in the first 20 test observations but significant deviations afterward, resulting in lower overall prediction quality.
- Similar trends are observed for **TCH**, **FPT**, **PLX**, **POW**, **VPB**, and **VRE**.

### Overall Conclusions

- High predictive accuracy for most stocks demonstrates the **suitability of the LSTM model** for **analyzing and forecasting stock price movements**.
- Findings align with previous studies by **Sen and Chaudhuri (2016)**, **Sen (2017)**, and **Mehtab and Sen (2019)**.
- Combining **price history** and **technical analysis indicators** to build the LSTM model confirms the potential of **technical indicators** in forecasting stock prices.
- The study highlights the **compatibility and mutual support** between technical analysis and financial data analysis models on a **machine learning platform**, specifically the **LSTM algorithm**.

## LSTM Algorithm and Technical Analysis Indicators to Forecast Price Trends in Vietnam’s Stock Market

The **LSTM model** is used for analyzing and forecasting stock price trends in Vietnam’s stock market, with practical and academic relevance. Figures 21 to 35 compare the **forecast price** and **actual price** of various shares such as **PLX, POW, REE, SBT, SSI, STB, TCB, TCH, TPB, VIC, VJC, VNM, VPB, VRE**, and **VCB**. These comparisons show the effectiveness of the LSTM model in forecasting stock prices.

The model's **application level** and forecasting performance demonstrate high potential for practical use by **investors, financial institutions, and government market regulators**. Real and updated stock market data enhance the model’s robustness and applicability.

---

## Conclusions

This research aims to evaluate the application of the **LSTM algorithm** combined with **technical analysis indicators** to forecast price trends on the Vietnamese stock market. The study uses historical price data from the **VN-Index** and stocks of the **VN-30 group**.

- The forecast results of the **LSTM model** show a **good predictive performance** for most stock data analyzed.
- The **LSTM model** is highly suitable for time series data such as **stock price history** due to its structural and analytical characteristics.
- The study confirms the **appropriateness** of the LSTM algorithm for stock price analysis and forecasting.

However, there are other strong **machine learning algorithms** like **Random Forest** and **Support Vector Machine** with great potential for stock price analysis and forecasting, which future studies may explore.

A common trend is to **combine multiple machine learning algorithms** to create more complex models with potentially higher performance. This study only applies a single LSTM algorithm, suggesting future research could improve predictive performance by combining different methods.

Unstructured data types such as **text, audio, and images** represent a potential new area for financial analysis in Vietnam using machine learning, which is currently unexplored.

### Limitations

- The study uses data limited to the **Ho Chi Minh City stock market (VN-Index)**.
- Difficulties arise in forecasting during periods of **strong stock price fluctuations**, partly due to internal market issues such as:
  - Small share sizes not fully reflecting supply and demand dynamics.
  - Market manipulation and legal risks.
  
Future research should consider expanding the data sources to include other **Vietnamese stock exchanges** to improve model evaluation and forecast reliability.

---

## Data Availability

The datasets used and/or analyzed are available from the author upon reasonable request.  
All data can be accessed at: [https://zenodo.org/uploads/10418013](https://zenodo.org/uploads/10418013).

---

## References

- **Agrawal M**, Khan AU, Shukla PK (2019) Stock price prediction using technical indicators: A predictive model using optimal deep learning. *International Journal of Recent Technology and Engineering*, 8(2), 2297–2305.

- **Appel, G** (2005) *Technical analysis: power tools for active investors*. FT Press.

- **Biondo AE**, Pluchino A, Rapisarda A, Helbing D (2013) Are random trading strategies more successful than technical ones? *PloS One*, 8(7), e68344.

- **Borovkova S**, Tsiamas I (2019) An ensemble of LSTM neural networks for high-frequency stock market classification. *Journal of Forecasting*, 38(6), 600–619.

- **Brock W**, Lakonishok J, LeBaron B (1992) Simple technical trading rules and the stochastic properties of stock returns. *Journal of Finance*, 47(5), 1731–1764.

- **Brown SJ**, Goetzmann WN, Kumar A (1998) The Dow theory: William Peter Hamilton’s track record reconsidered. *Journal of Finance*, 53(4), 1311–1333.

- **Burton, N** (2018) *An Analysis of Burton G. Malkiel’s A Random Walk Down Wall Street* (1st edition). Routledge. ISBN 9781912128822. Available at: https://www.routledge.com/An-Analysis-of-Burton-G-Malkiels-A-Random-Walk-Down-Wall-Street/Burton/p/book/9781912128822 (accessed 30 Oct 2023).

- **Chan, MC**, Wong, CC, & Lam, CC (2000) Financial time series forecasting by neural network using conjugate gradient learning algorithm and multiple linear regression weight initialization. Available at: https://citeseerx.ist.psu.edu/document?repid=rep1&type=pdf&doi=5853eb9035a62449c39f213768a60603352bcf05 (accessed 30 Oct 2023).

- **Chen, K**, Zhou, Y, & Dai, F (2015) A LSTM-based method for stock returns prediction: A case study of China stock market. In *2015 IEEE International Conference on Big Data* (pp. 2823–2824). https://doi.org/10.1109/BigData.2015.7364089.

- **Di Persio L**, Honchar O (2016) Artificial neural networks approach to the forecast of stock market price movements. *International Journal of Economics and Management Systems*, 1, 158–162.

- **Ding, X**, Zhang, Y, Liu, T, & Duan, J (2015) Deep learning for event-driven stock prediction. *Proceedings of the Twenty-Fourth International Joint Conference on Artificial Intelligence* (IJCAI 2015), 2327–2333. Available at: https://www.ijcai.org/Proceedings/15/Papers/329.pdf (accessed 30 Oct 2023).

- **Elman JL** (1990) Finding structures in time. *Cognitive Science*, 14(2), 179–211.

- **El-Nagar AM**, Zaki AM, Soliman FAS, El-Bardini M (2022) Hybrid deep learning diagonal recurrent neural network controller for nonlinear systems. *Neural Computing and Applications*, 34(24), 22367–22386. https://doi.org/10.1007/s00521-022-07673-9.

- **Fama EF** (1970) Efficient Capital Markets: A Review of Theory and Empirical Work. *Journal of Finance*, 25(2), 383–417. https://doi.org/10.2307/2325486.

- **Fifield SG**, Power DM, Donald Sinclair C (2005) An analysis of trading strategies in eleven European stock markets. *European Journal of Finance*, 11(6), 531–548.

- **Fromlet, H** (2001) Behavioral finance-theory and practical application: Systematic analysis of departures from the homo oeconomicus paradigm are essential for realistic financial research and analysis. *Business Economics*, 63–69.

- **Graves A**, Liwicki M, Fernández S, Bertolami R, Bunke H, Schmidhuber J (2008) A novel connectionist system for unconstrained handwriting recognition. *IEEE Transactions on Pattern Analysis and Machine Intelligence*, 31(5), 855–868.

- **Graves, A** (2012) Supervised sequence labeling. In *Supervised sequence labeling with recurrent neural networks* (pp. 5–13). Springer.

- **Greff K**, Srivastava RK, Koutník J, Steunebrink BR, Schmidhuber J (2016) LSTM: A search space odyssey. *IEEE Transactions on Neural Networks and Learning Systems*, 28(10), 2222–2232.

- **Gülmez B** (2023) Stock price prediction with optimized deep LSTM network with artificial rabbits optimization algorithm. *Expert Systems with Applications*, 227, 120346. https://doi.org/10.1016/j.eswa.2023.120346.

- **Heaton JB**, Polson N, Witte J (2016) Deep Learning for Finance: Deep Portfolios. *Applied Stochastic Models in Business and Industry*, 33(1), 3–12.

- **Hegazy O**, Soliman OS, Salam MA (2014) A machine learning model for stock market prediction. *International Journal of Computer Science and Telecommunications*, 4(12), 17–23.

- **Hoang, M** (2023) Vietnam’s stock market capitalization is equivalent to 82% of GDP. Available at: https://tapchitaichinh.vn/von-hoa-thi-truong-chung-khoan-viet-nam-tuong-duong-82-gdp.html (accessed 30 Oct 2023).

- **Hochreiter S**, Schmidhuber J (1997) Long Short-Term Memory. *Neural Computation*, 9, 1735–1780. https://doi.org/10.1162/neco.1997.9.8.1735.

- **Jia, H** (2016) Investigation into the effectiveness of long short term memory networks for stock price prediction. Retrieved from https://ui.adsabs.harvard.edu/abs/2016arXiv160307893J/abstract (accessed 1 Nov 2023).

- **Kirkpatrick CD**, Dahlquist JR (2010) *Technical Analysis: The Complete Resource for Financial Market Technicians*. 2nd Ed., FT Press.

- **Lai, CY**, Chen, RC, & Caraka, RE (2019) Prediction Stock Price Based on Different Index Factors Using LSTM.

## References

- **2019 International Conference on Machine Learning and Cybernetics (ICMLC), 1-6**

- **Le TTH, Nguyen VC, Phan THN (2022)**  
  Foreign Direct Investment, Environmental Pollution and Economic Growth—An Insight from Non-Linear ARDL Co-Integration Approach. *Sustainability* **14**(13):8146.  
  [https://doi.org/10.3390/su14138146](https://doi.org/10.3390/su14138146)

- **Liu S, Graham SL, Schulz A, Kalloniatis M, Zangerl B, Cai W, Gao Y, Chua B, Arvind H, Grigg J, Chu D, Klistorner A, You Y (2018)**  
  A Deep Learning-Based Algorithm Identifies Glaucomatous Discs Using Monoscopic Fundus Photographs. *Ophthalmol Glaucoma* **1**(1):15–22.

- **Lo, AW, & MacKinlay, AC (1999)**  
  *A Non-Random Walk Down Wall Street*. Princeton University Press.  
  [http://www.jstor.org/stable/j.ctt7tccx](http://www.jstor.org/stable/j.ctt7tccx)

- **Mehtab S, Sen J (2020)**  
  Stock Price Prediction Using Convolutional Neural Networks on a Multivariate Time Series. Proceedings of the 3rd National Conference on Machine Learning and Artificial Intelligence, New Delhi, INDIA.  
  [https://ssrn.com/abstract=3665363](https://ssrn.com/abstract=3665363)

- **Mehtab, S, Sen, J (2019)**  
  A Robust Predictive Model for Stock Price Prediction Using Deep Learning and Natural Language Processing. Available at SSRN:  
  [https://ssrn.com/abstract=3502624](https://ssrn.com/abstract=3502624)

- **Nelson, DM, Pereira, AC, & de Oliveira, RA (2017)**  
  Stock market’s price movement prediction with LSTM neural networks. Paper presented at the 2017 International Joint Conference on Neural Networks (IJCNN).

- **Pahwa N, Khalfay N, Soni V, Vora D (2017)**  
  Stock prediction using machine learning: a review paper. *Int J Computer Appl* **163**(5):36–43.

- **Roman J, Jameel A (1996)**  
  Backpropagation and recurrent neural networks in financial analysis of multiple stock market returns. *Proc HICSS-29: 29th Hawaii Int Conf Syst Sci* **2**:454–460 vol.2.  
  [https://doi.org/10.1109/HICSS.1996.495431](https://doi.org/10.1109/HICSS.1996.495431)

- **Saad E, Prokhorov E, Wunsch D (1998)**  
  Comparative Study of Stock Trend Prediction Using Time Delay, Recurrent and Probabilistic Neural Networks. *IEEE Trans Neural Netw* **9**:1456–1470.  
  [https://doi.org/10.1109/72.728395](https://doi.org/10.1109/72.728395)

- **Sen J (2017)**  
  A Robust Analysis and Forecasting Framework for the Indian Mid Cap Sector Using Times Series Decomposition Approach. *J Insurance Financ Manag* **3**(4):1–32.

- **Sen J, Datta Chaudhuri D (2018)**  
  Understanding the sectors of Indian economy for portfolio choice. *Int J Bus Forecast Market Intel* **4**(2):178–222.  
  [https://doi.org/10.1504/IJBFMI.2018.090914](https://doi.org/10.1504/IJBFMI.2018.090914)

- **Sen J, Chaudhuri TD (2016)**  
  An alternative framework for time series decomposition and forecasting and its relevance for portfolio choice: a comparative study of the Indian consumer durable and small cap sectors. *J Econ Libr* **3**(2):303–326.

- **Trinh HH, Nguyen CP, Hao W, Wongchoti U (2021)**  
  Does stock liquidity affect bankruptcy risk? DID analysis from Vietnam. *Pac-Basin Financ J* **69**:101634.  
  [https://doi.org/10.1016/j.pacfin.2021.101634](https://doi.org/10.1016/j.pacfin.2021.101634)

- **Usmani S, Shamsi JA (2023)**  
  LSTM based stock prediction using weighted and categorized financial news. *PLoS ONE* **18**(3):e0282234.

- **White (1988)**  
  Economic prediction using neural networks: the case of IBM daily stock returns. *IEEE 1988 Int Conf Neural Netw* **2**:451–458.  
  [https://doi.org/10.1109/ICNN.1988.23959](https://doi.org/10.1109/ICNN.1988.23959)

- **Zhuge Q, Xu L, Zhang G (2017)**  
  LSTM Neural Network with Emotional Analysis for prediction of stock price. *Eng Lett* **25**(2):25–32.

---

## Author Contributions

- **Conceptualization:** TP, PTKA, PHT, NCV  
- **Methodology:** TP, PTKA, PHT, NCV  
- **Format analysis and investigation:** TP, PTKA, PHT, NCV  
- **Writing - review and editing:** TP, PTKA, PHT, NCV  
- **Resources:** TP  
- **Supervision:** NCV

---

## Competing Interests

The authors declare **no competing interests**.

---

## Ethical Approval

Ethical approval is **not required** by our universities. Ethical approval was therefore **not provided**.

---

## Informed Consent

This article does **not contain any studies with human participants** performed by any of the authors.

---

## Additional Information

Correspondence and requests for materials should be addressed to **Chien V. Nguyen**. Reprints and permission information is available at [http://www.nature.com/reprints](http://www.nature.com/reprints).

---

## Publisher’s Note

**Springer Nature** remains neutral with regard to jurisdictional claims in published maps and institutional affiliations.

---

## Open Access License

This article is licensed under a **Creative Commons Attribution 4.0 International License**, which permits use, sharing, adaptation, distribution, and reproduction in any medium or format, as long as you give appropriate credit to the original author(s) and the source, provide a link to the Creative Commons licence, and indicate if changes were made.

- The images or other third party material in this article are included in the article’s Creative Commons licence, unless otherwise indicated in a credit line to the material.  
- If material is not included in the article’s Creative Commons licence and your intended use is not permitted by statutory regulation or exceeds the permitted use, you will need to obtain permission directly from the copyright holder.  
- To view a copy of this licence, visit [http://creativecommons.org/licenses/by/4.0/](http://creativecommons.org/licenses/by/4.0/).

© The Author(s) **2024**

---

## Citation

**HUMANITIES AND SOCIAL SCIENCES COMMUNICATIONS** | https://doi.org/10.1057/s41599-024-02807-x  
**18 HUMANITIES AND SOCIAL SCIENCES COMMUNICATIONS** | (2024) 11:393  
[https://doi.org/10.1057/s41599-024-02807-x](https://doi.org/10.1057/s41599-024-02807-x)