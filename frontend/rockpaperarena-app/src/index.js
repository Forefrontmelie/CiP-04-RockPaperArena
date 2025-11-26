import React from 'react';
import ReactDOM from 'react-dom/client';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import './index.css';
import App from './App';


// TODO: Skapa eget theme ...?
const theme = createTheme({
  palette: {
  },
  typography: {
  },
});



const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <ThemeProvider theme={theme}>
      <CssBaseline /> {/* Normalizes browser defaults */}
      <App />
    </ThemeProvider>
  </React.StrictMode>
);

