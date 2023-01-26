/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { Component } from 'react';
import axios from 'axios';
import { LineChart, Line, CartesianGrid, XAxis, YAxis, Tooltip, ResponsiveContainer } from 'recharts';


export class Instrument extends Component<any, any> {
  static displayName = Instrument.name;
  private _didMount: any;

  constructor(props: any) {
    super(props);
    this.state = {
      currentInstrument: {},
      currentCount: 0,
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

  private getValueData(data: any, chartType: string) {
    // const diff_minutes = (dt2: Date, dt1: Date) => {
    //   return Math.abs(((dt2.getTime() - dt1.getTime()) / 1000) / 60);
    // };
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
    if (chartType === 'stepline') {
      return items.map((item: any) => {
        item.y = item.y[3];
        return item;
      });
    }
    return items;
  }

  private getSimpleData(data: any) {
    // const diff_minutes = (dt2: Date, dt1: Date) => {
    //   return Math.abs(((dt2.getTime() - dt1.getTime()) / 1000) / 60);
    // };
    const items = data.timestamp.map((time: Date, index: number) => {
      return {
        x: time,
        o: Math.round((data.indicators.quote[0].open[index] + Number.EPSILON) * 100) / 100,
        h: Math.round((data.indicators.quote[0].high[index] + Number.EPSILON) * 100) / 100,
        l: Math.round((data.indicators.quote[0].low[index] + Number.EPSILON) * 100) / 100,
        c: Math.round((data.indicators.quote[0].close[index] + Number.EPSILON) * 100) / 100,
        v: Math.round((data.indicators.quote[0].volume[index] + Number.EPSILON) * 100) / 100
      }
    }).reduce((acc: any, current: any, i: number, arr: any[]) => {
      if (i > 0 && !current?.c) {
        current.o = arr[i - 1].o;
        current.h = arr[i - 1].h;
        current.l = arr[i - 1].l;
        current.c = arr[i - 1].c;
      }

      acc.push(current);
      return acc;
    }, []);
    return items;
  }

  private getVolumeData(data: any) {
    // const diff_minutes = (dt2: Date, dt1: Date) => {
    //   return Math.abs(((dt2.getTime() - dt1.getTime()) / 1000) / 60);
    // };
    const items = data.timestamp.map((time: Date, index: number) => {
      return {
        time,
        y: data.indicators.quote[0].volume[index]
      }
    });
    return items;
  }

  async fetchData(symbol: string) {
    try {
      const response = await axios.get(`api/instruments/${symbol}/chart/1d/2m`);
      // const valueData = this.getValueData(response.data[0], 'stepline');
      // const volumeData = this.getVolumeData(response.data[0]);
      const simpleData = this.getSimpleData(response.data[0]);
      this.setState({ simpleData });
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
      <ResponsiveContainer width="100%" height="100%">
        <LineChart data={this.state.simpleData} margin={{ top: 5, right: 20, bottom: 5, left: 0 }}>
          <Line type="stepAfter" dataKey="c" stroke="#8884d8" dot={false} activeDot={{ r: 2 }} connectNulls={true} />
          <CartesianGrid stroke="#ccc" strokeDasharray="5 5" />
          <XAxis dataKey="x" />
          <YAxis type="number" domain={['dataMin - (dataMin*1.9)', 'dataMax + (dataMax*1.1)']} />
          <Tooltip />
        </LineChart>
      </ResponsiveContainer>
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
