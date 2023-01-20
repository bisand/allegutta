import React, { Component } from 'react';
import { Route, Routes, useParams } from 'react-router-dom';
import AppRoutes from './AppRoutes';
import './App.css';
import { Layout } from './components/Layout';
import { Instrument } from './components/Instrument';

export default class App extends Component {
  static displayName = App.name;

  constructor(props: any) {
    super(props);
    this.state = { symbol: '' };
  }

  render() {
    return (
      <Layout>
        <Routes>
          {AppRoutes.map((route, index) => {
            const { element, ...rest } = route;
            return <Route key={index} {...rest} element={element} />;
          })}
        </Routes>
      </Layout>
    );
  }
}

