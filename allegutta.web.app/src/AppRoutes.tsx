import { Counter } from "./components/Counter";
import { PortfolioComponent } from "./components/PortfolioComponent";
import { Home } from "./components/Home";
import { Instrument } from './components/Instrument';
import { Vedtekter } from './components/Vedtekter';
import { News } from './components/News';

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
    path: '/vedtekter',
    element: <Vedtekter />
  },
  {
    path: '/news',
    element: <News />
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
