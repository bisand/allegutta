import React from 'react';
import { Counter } from "./components/Counter";
import { PortfolioComponent } from "./components/PortfolioComponent";
import { Home } from "./components/Home";
import { Instrument } from './components/Instrument';
import { Vedtekter } from './components/Vedtekter';
import { News } from './components/News';
import { useParams } from 'react-router-dom';

const InstrumentWrapper = (props: any) => {
  const params = useParams();
  console.log('WRAPPER PARAMS: ', params);
  return (
    <Instrument {...{ ...props, match: { params } }} />
  );
};

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
    element: <InstrumentWrapper />
  },
  {
    path: '/portfolio',
    element: <PortfolioComponent />
  }
];

export default AppRoutes;
