import React from "react";
import { Loader, Segment, Table } from "semantic-ui-react";
import { useFetchRunnerQuery } from "./slices/runners/runners-api-slice";
import logo from "./logo.svg";
import "./App.css";

function App() {
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

  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>Hello Vite + React! </p>

        <Segment inverted>
          {isFetching ? <Loader active inverted content="Loading" /> : table}
        </Segment>
      </header>
    </div>
  );
}

export default App;
