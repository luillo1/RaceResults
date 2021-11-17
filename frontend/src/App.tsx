import React from "react";
import { useAppDispatch, useAppSelector } from "./app/hooks";
import { useFetchRunnerQuery } from "./features/runners/runners-api-slice";
import logo from "./logo.svg";
import "./App.css";

function App() {
  const { data = [], isFetching } = useFetchRunnerQuery("foo");

  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>Hello Vite + React!</p>

        <p>
          <a
            className="App-link"
            href="https://reactjs.org"
            target="_blank"
            rel="noopener noreferrer"
          >
            Learn React
          </a>
          {" | "}
          <a
            className="App-link"
            href="https://vitejs.dev/guide/features.html"
            target="_blank"
            rel="noopener noreferrer"
          >
            Vite Docs
          </a>
        </p>
      </header>
    </div>
  );
}

export default App;
