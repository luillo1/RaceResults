import React, { useRef } from "react";
import { Container, Table } from "semantic-ui-react";
import { useFetchRunnerQuery } from "./slices/runners/runners-api-slice";
import "./App.css";
import Navbar from "./components/navbar";
import Home from "./pages/home";
import { Routes, Route } from "react-router-dom";

function App() {
  /*
  const { data = [], isFetching } = useFetchRunnerQuery();

  const table = (
    <Table celled>
      <Table.Header>
        <Table.Row>
          <Table.HeaderCell>First Name</Table.HeaderCell>
          <Table.HeaderCell>Last Name</Table.HeaderCell>
          <Table.HeaderCell>Nicknames</Table.HeaderCell>
        </Table.Row>
      </Table.Header>

      <Table.Body>
        {data.map((runner) => (
          <Table.Row key={runner.firstName}>
            <Table.Cell>{runner.firstName}</Table.Cell>
            <Table.Cell>{runner.lastName}</Table.Cell>
            <Table.Cell>{runner.nicknames.join(", ")}</Table.Cell>
          </Table.Row>
        ))}
      </Table.Body>
    </Table>
  );
  */

  const appRef = useRef(null);
  return (
    <Container fluid ref={appRef}>
      <Navbar appRef={appRef} />
      <Container>
        <Routes>
          <Route path="/" element={<Home />}></Route>
        </Routes>
      </Container>
    </Container>
  );
}

export default App;
