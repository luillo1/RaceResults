import React from "react";
import { Table, Segment, Dimmer, Loader, Placeholder } from "semantic-ui-react";
import { useFetchRunnerQuery } from "../slices/runners/runners-api-slice";

function Home() {
  const { data = [], isFetching } = useFetchRunnerQuery();

  const placeHolder = (
    <Segment>
      <Dimmer active>
        <Loader content="Loading" />
      </Dimmer>

      <Placeholder fluid>
        <Placeholder.Header image>
          <Placeholder.Line />
          <Placeholder.Line />
        </Placeholder.Header>
        <Placeholder.Paragraph>
          <Placeholder.Line />
          <Placeholder.Line />
          <Placeholder.Line />
        </Placeholder.Paragraph>
      </Placeholder>
    </Segment>
  );

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
    <div>
      <h2>Sample Data</h2>
      {isFetching ? placeHolder : table}
    </div>
  );
}

export default Home;
