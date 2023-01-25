/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { Component } from 'react';
import { testData } from '../data/test-data';
import Chart from "react-apexcharts";
import axios from 'axios';


export class Instrument extends Component<any, any> {
  static displayName = Instrument.name;
  private _didMount: any;

  constructor(props: any) {
    super(props);
    this.state = {
      currentInstrument: {},
      currentCount: 0,
      options: {
        chart: {
          height: 350,
          type: "line"
        },
        title: {
          text: "CandleStick Chart",
          align: "left"
        },
        stroke: {
          width: [3, 1]
        },
        xaxis: {
          type: "datetime"
        }
      },
      series: [{}]
    };
    this.incrementCounter = this.incrementCounter.bind(this);
  }

  componentDidMount() {
    if (this._didMount)
      return;
    this._didMount = true;

    const { symbol } = this.props.match.params;  // Unpacking and retrieve symbol

    this.fetchData(symbol);
  }

  private transformData(data: any) {
    const items = data.timestamp.map((time: Date, index: number) => {
      return {
        x: time,
        y: [
          data.indicators.quote[0].open[index],
          data.indicators.quote[0].high[index],
          data.indicators.quote[0].low[index],
          data.indicators.quote[0].close[index]
        ]
      }
    });
    return items;
  }

  async fetchData(symbol: string) {
    try {
      const response = await axios.get(`api/instruments/${symbol}/chart/1d/1m`);
      const series = this.transformData(response.data[0]);
      this.setState({ series: [{ data: series }] });
    } catch (e) {
      console.log(e);
    }
    return {};
  }

  incrementCounter() {
    this.setState((prevState: any) => {
      return { currentCount: prevState.currentCount + 1 };
    });
  }

  private renderChart() {
    return (
      <Chart
        options={this.state.options}
        series={this.state.series}
        type="candlestick"
        width="1000"
      />
    );
  }

  render() {
    const { symbol } = this.props.match.params;  // Unpacking and retrieve symbol

    const chartView = this.renderChart();

    return (
      <div>
        <h1>Counter</h1>
        <div>Id: {symbol}</div>
        <p>This is a simple example of a React component.</p>

        <p aria-live="polite">Current count: <strong>{this.state.currentCount}</strong></p>
        <div style={{ height: 600 }}>
          {chartView}
        </div>
        <button className="btn btn-primary" onClick={this.incrementCounter}>Increment</button>
      </div>
    );
  }
}
