import React, { Component } from 'react';
import { useParams } from 'react-router-dom'

export class Instrument extends Component<any, any> {
  static displayName = Instrument.name;

  constructor(props: any) {
    super(props);
    this.state = { currentInstrument: {}, currentCount: 0 };
    this.incrementCounter = this.incrementCounter.bind(this);
  }

  // componentDidMount(): void {
  // }
  incrementCounter() {
    this.setState((prevState: any) => {
      return { currentCount: prevState.currentCount + 1 };
    });
  }

  render() {
    const { symbol } = this.props.match.params;  // Unpacking and retrieve symbol
    return (
      <div>
        <h1>Counter</h1>
        <div>Id: {symbol}</div>
        <p>This is a simple example of a React component.</p>

        <p aria-live="polite">Current count: <strong>{this.state.currentCount}</strong></p>

        <button className="btn btn-primary" onClick={this.incrementCounter}>Increment</button>
      </div>
    );
  }
}
