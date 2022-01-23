# RaceResults Frontend

This is the frontend webpage for https://raceresults.run.

## Technologies

- Language: [TypeScript](https://www.typescriptlang.org/)
- Framework: [React](https://reactjs.org/)
- Webpack: [ViteJS](https://vitejs.dev/)
- Data store/state management: [React-Redux](https://react-redux.js.org/)
- Package management: [npm](https://www.npmjs.com/)
- UI framework: [Semantic UI](https://react.semantic-ui.com/)

## Getting started

While in this document's containing directory, run `npm install`. Afterwards, you can start the development server by
running `npm run dev`.

## Backend connection

While running in development mode, all calls to a backend will be forwarded to the proxy server
`https://localhost:5001/`. This can be configured by editing `vite.config.ts`. This proxy is the default
URL when running the backend API (which can be started by running `dotnet run` from the `/api/src/API` directory).

## Useful documentation
- [Semantic React docs](https://react.semantic-ui.com/) for reference when consturcting pages/components
- [React Redux RTK Query docs](https://redux-toolkit.js.org/rtk-query/overview) for understanding how to interact with the backup

## Folder structure
