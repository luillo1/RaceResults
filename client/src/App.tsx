import React, { useRef } from "react";
import { Container } from "semantic-ui-react";
import "./App.css";
import Navbar from "./components/navbar";
import Home from "./pages/home";
import { Routes, Route } from "react-router-dom";

function App() {
  console.log(import.meta.env);

  const appRef = useRef(null);

  return (
    <div ref={appRef}>
      <div>
        <Navbar appRef={appRef} />
        <br />
        <Container>
          <Routes>
            <Route path="/" element={<Home />}></Route>
          </Routes>
        </Container>
      </div>
    </div>
  );
}

export default App;
