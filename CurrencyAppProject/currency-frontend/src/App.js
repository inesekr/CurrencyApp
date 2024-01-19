import "./App.css";
import React, { useState, useEffect } from "react";
import axios from "axios";

const CurrencyMappings = {
  USD: { Symbol: "$" },
  EUR: { Symbol: "€" },
  GBP: { Symbol: "£" },
  JPY: { Symbol: "¥" },
  CHF: { Symbol: "CHF" },
};

function App() {
  const [conversionData, setConversionData] = useState({});
  const [fromCurrency, setFromCurrency] = useState("EUR");
  const [toCurrency, setToCurrency] = useState("USD");
  const [amount, setAmount] = useState(1);

  const formatAmount = (value) => {
    // Removing existing commas and parsing the value as a number
    const numericValue = parseFloat(value.replace(/,/g, ""));

    // If the value ends with a dot, don't add a comma
    if (value.endsWith(".")) {
      return (
        numericValue.toLocaleString(undefined, {
          useGrouping: false,
          minimumFractionDigits: 0,
          maximumFractionDigits: 2,
        }) + "."
      );
    }

    const formattedValue = numericValue.toLocaleString(undefined, {
      useGrouping: true,
      minimumFractionDigits: 0,
      maximumFractionDigits: 2,
    });

    return formattedValue;
  };

  const handleAmountChange = (e) => {
    const formattedValue = formatAmount(e.target.value);
    setAmount(formattedValue);
  };

  const formatNumber = (value) => {
    const numericValue = parseFloat(value);
    return new Intl.NumberFormat(undefined, {
      minimumFractionDigits: 0,
      maximumFractionDigits: 2,
    }).format(numericValue);
  };

  const renderFormattedResult = (value) => {
    const numericValue = parseFloat(value);
    const formattedValue = new Intl.NumberFormat(undefined, {
      minimumFractionDigits: 0,
      maximumFractionDigits: 4,
    }).format(numericValue);
    const [integerPart, decimalPart] = formattedValue.split(".");

    return (
      <>
        <span>{integerPart}</span>
        <span>.</span>
        <span>{formatDecimalPart(decimalPart)}</span>
      </>
    );
  };

  const formatDecimalPart = (decimalPart) => {
    const firstTwoDigits = decimalPart.slice(0, 2);
    const remainingDigits = decimalPart.slice(2);

    return (
      <>
        <span>{firstTwoDigits}</span>
        <span className="lighter">{remainingDigits}</span>
      </>
    );
  };

  const handleConvert = async () => {
    try {
      const response = await axios.get(
        `http://localhost:5039/Currency?fromCurrencyCode=${fromCurrency}&toCurrencyCode=${toCurrency}&amount=${amount}`
      );

      setConversionData(response.data);
    } catch (error) {
      console.error("Error converting currency:", error);
    }
  };

  const handleReverseCurrencies = () => {
    // Swap fromCurrency and toCurrency
    setFromCurrency(toCurrency);
    setToCurrency(fromCurrency);
  };

  return (
    <div className="App">
      <h1>Currency Converter</h1>
      <div className="input-group">
        <div className="input-field">
          <label className="label-left">Amount:</label>

          <div className="amount-input">
            <span className="input-group-text">
              {CurrencyMappings[fromCurrency].Symbol}
            </span>
            <input
              type="text"
              value={amount}
              onChange={handleAmountChange}
              className="form-control form-control-lg"
            />
          </div>
        </div>
        <div className="input-field">
          <label className="label-left">From:</label>
          <div className="currency-input">
            <select
              value={fromCurrency}
              onChange={(e) => setFromCurrency(e.target.value)}
              className="form-select form-select-lg"
            >
              <option value="" disabled>
                Select Currency
              </option>
              <option value="USD">USD - United States Dollar</option>
              <option value="EUR">EUR - Euro</option>
              <option value="GBP">GBP - British Pounds</option>
              <option value="JPY">JPY - Japanese Jen</option>
              <option value="CHF">CHF - Swiss Franc</option>
            </select>
          </div>
        </div>

        <div className="reverse">
          <button
            onClick={handleReverseCurrencies}
            className="btn btn-secondary"
          >
            ↔️ Reverse
          </button>
        </div>

        <div className="input-field">
          <label className="label-left">To:</label>
          <div className="currency-input">
            <select
              value={toCurrency}
              onChange={(e) => setToCurrency(e.target.value)}
              className="form-select form-select-lg"
            >
              <option value="" disabled>
                Select Currency
              </option>
              <option value="USD">USD - United States Dollar</option>
              <option value="EUR">EUR - Euro</option>
              <option value="GBP">GBP - British Pounds</option>
              <option value="JPY">JPY - Japanese Jen</option>
              <option value="CHF">CHF - Swiss Franc</option>
            </select>
          </div>
        </div>
      </div>

      <div className="d-flex justify-content-end">
        <button onClick={handleConvert} className="btn btn-primary">
          Convert
        </button>
      </div>

      {Object.keys(conversionData).length > 0 && (
        <div className="result">
          <p>
            {formatNumber(conversionData.amount)} {conversionData.fromCurrency}{" "}
            =
          </p>
          <h3>
            {renderFormattedResult(conversionData.result)}{" "}
            {conversionData.toCurrency}
          </h3>
          <p>
            1 {conversionData.fromCurrency} = {conversionData.exchangeRate}{" "}
            {conversionData.toCurrency}
          </p>
        </div>
      )}
    </div>
  );
}

export default App;
