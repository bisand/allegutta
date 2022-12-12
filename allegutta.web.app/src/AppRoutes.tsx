import { Counter } from "./components/Counter";
import { PortfolioComponent } from "./components/PortfolioComponent";
import { Home } from "./components/Home";

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
    path: '/portfolio',
    element: <PortfolioComponent />
  }
];

export default AppRoutes;