/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { Component } from 'react';
import axios from 'axios';
import ReactApexChart from 'react-apexcharts';


export class Instrument extends Component<any, any> {
  static displayName = Instrument.name;
  private _didMount: any;
  private _lineYMin = 0;
  private _lineYMax = 0;
  private _timer?: NodeJS.Timeout;

  constructor(props: any) {
    super(props);
    this.state = {
      currentInstrument: {},
      currentCount: 0,
      simpleData: [],
      series: [],
      options: {
        colors: ['#5e7f5782', '#257cda'],
        theme: {
          mode: this.getCurrentTheme(),
        },
        stroke: {
          width: [2, 2],
          curve: 'smooth',
        },
        fill: {
        },
        tooltip: {
          followCursor: true,
          x: {
            show: true,
            format: 'yyyy-MM-dd HH:mm',
            formatter: undefined,
          },
          y: {
            formatter: undefined,
            title: {
              formatter: (seriesName: string) => seriesName,
            },
          },
        },
        xaxis: {
          type: 'datetime',
          labels: {
            format: 'HH:mm',
          },
          title: {
            text: 'Tid',
            offsetY: 75,
          },
        },
        yaxis: [
          {
            labels: {
              formatter: function (value: number) {
                return value >= 10000 ? value >= 1000000 ? value / 1000000 + " M" : value / 1000 + " k" : value;
              }
            },
            title: {
              text: 'Volum',
            }
          },
          {
            opposite: true,
            decimalsInFloat: 3,
            title: {
              text: 'Pris',
            },
            min: (min: number) => {
              return this._lineYMin > 0 ? this._lineYMin - (Math.abs(this._lineYMax - this._lineYMin) * 5) / 100 : min;
            },
            max: (max: number) => {
              return this._lineYMax > 0 ? this._lineYMax + (Math.abs(this._lineYMax - this._lineYMin) * 2) / 100 : max;
            }
          },
        ]
      },
    };

    this.incrementCounter = this.incrementCounter.bind(this);
  }

  public componentDidMount() {
    if (this._didMount)
      return;
    this._didMount = true;

    const { symbol } = this.props.match.params;  // Unpacking and retrieve symbol

    this.updateChartOptions();

    this.fetchData(symbol);
  }

  private updateChartOptions() {
    this._timer = setTimeout(() => {
      const theme = this.getCurrentTheme();
      this.setState({
        options: {
          ...this.state.options,
          theme: {
            ...this.state.options.theme,
            mode: theme,
          }
        }
      });
    }, 100);
  }

  private getCurrentTheme(): string {
    const theme = document.getElementsByTagName('html')[0].getAttribute('data-bs-color-scheme');
    return theme ?? 'dark';
  }

  private getCandleStickData(data: any, chartType: string) {
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

  private round(value: number, decimals = 1) {
    const calc = Number(String('1').padEnd(decimals + 1, '0'));
    return Math.round((value + Number.EPSILON) * calc) / calc;
  }

  private getSimpleData(data: any) {
    // const diff_minutes = (dt2: Date, dt1: Date) => {
    //   return Math.abs(((dt2.getTime() - dt1.getTime()) / 1000) / 60);
    // };
    const items = data?.timestamp?.map((time: Date, index: number) => {
      return {
        x: time,
        o: (data.indicators.quote[0].open[index]),
        h: (data.indicators.quote[0].high[index]),
        l: (data.indicators.quote[0].low[index]),
        c: (data.indicators.quote[0].close[index]),
        v: (data.indicators.quote[0].volume[index])
      }
    }).reduce((acc: any, current: any, i: number, arr: any[]) => {
      if (i > 0 && current && current.v && !current.c) {
        current.o = arr[i - 1].o;
        current.h = arr[i - 1].h;
        current.l = arr[i - 1].l;
        current.c = arr[i - 1].c;
      }

      if (current.v)
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
      const simpleData = this.getSimpleData(response.data[0]);
      this._lineYMin = simpleData ? Math.min(...simpleData.map((x: any) => x.c)) : 0;
      this._lineYMax = simpleData ? Math.max(...simpleData.map((x: any) => x.c)) : 0;
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
    const dataSeries = [
      {
        name: 'Volum',
        type: 'column',
        data: this.state.simpleData?.map((x: any) => {
          return { x: x.x, y: x.v };
        })
      },
      {
        name: 'Pris',
        type: 'line',
        data: this.state.simpleData?.map((x: any) => {
          return { x: x.x, y: x.c };
        })
      },
    ];

    return (
      <ReactApexChart options={this.state.options} series={dataSeries} type="line" />
    );
  }

  render() {
    const { symbol } = this.props.match.params;  // Unpacking and retrieve symbol

    const chartView = this.renderChart();

    return (
      <div>
        <h1>{symbol}</h1>
        <div>
          {chartView}
        </div>
      </div>
    );
  }
}
