/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { Component } from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';

export class Layout extends Component<any, any> {
  static displayName = Layout.name;

  render() {
    return (
      <div>
        <Router>
          <NavMenu />
          <Container tag="main" className="mb-5">
            {this.props.children}
          </Container>
        </Router>
      </div>
    );
  }
}
