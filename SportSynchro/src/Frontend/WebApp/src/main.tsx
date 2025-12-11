import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import { RouterProvider } from "react-router-dom";
import { router } from "./router";
import { SportsProvider } from './contexts/SportsContext';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <SportsProvider>
      <RouterProvider router={router} />
    </SportsProvider>
  </StrictMode>,
)
