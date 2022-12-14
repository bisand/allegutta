import { Counter } from "./components/Counter";
import { PortfolioComponent } from "./components/PortfolioComponent";
import { Home } from "./components/Home";
import { Instrument } from './components/Instrument';

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/instrument/:symbol',
    element: <Instrument />
  },
  {
    path: '/portfolio',
    element: <PortfolioComponent />
  }
];

export default AppRoutes;
