/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { Component } from 'react';
import { ResponsiveLine } from '@nivo/line'


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

  private renderChart(data: any) {
    return (
      <ResponsiveLine
        data={data}
        margin={{ top: 50, right: 110, bottom: 50, left: 60 }}
        xScale={{ type: 'point' }}
        yScale={{
          type: 'linear',
          min: 'auto',
          max: 'auto',
          stacked: true,
          reverse: false
        }}
        yFormat=" >-.2f"
        axisTop={null}
        axisRight={null}
        axisBottom={{
          tickSize: 5,
          tickPadding: 5,
          tickRotation: 0,
          legend: 'transportation',
          legendOffset: 36,
          legendPosition: 'middle'
        }}
        axisLeft={{
          tickSize: 5,
          tickPadding: 5,
          tickRotation: 0,
          legend: 'count',
          legendOffset: -40,
          legendPosition: 'middle'
        }}
        pointSize={10}
        pointColor={{ theme: 'background' }}
        pointBorderWidth={2}
        pointBorderColor={{ from: 'serieColor' }}
        pointLabelYOffset={-12}
        useMesh={true}
        legends={[
          {
            anchor: 'bottom-right',
            direction: 'column',
            justify: false,
            translateX: 100,
            translateY: 0,
            itemsSpacing: 0,
            itemDirection: 'left-to-right',
            itemWidth: 80,
            itemHeight: 20,
            itemOpacity: 0.75,
            symbolSize: 12,
            symbolShape: 'circle',
            symbolBorderColor: 'rgba(0, 0, 0, .5)',
            effects: [
              {
                on: 'hover',
                style: {
                  itemBackground: 'rgba(0, 0, 0, .03)',
                  itemOpacity: 1
                }
              }
            ]
          }
        ]}
      />
    );
  }

  render() {
    const { symbol } = this.props.match.params;  // Unpacking and retrieve symbol
    const data = [
      {
        "id": "japan",
        "color": "hsl(286, 70%, 50%)",
        "data": [
          {
            "x": "plane",
            "y": 151
          },
          {
            "x": "helicopter",
            "y": 279
          },
          {
            "x": "boat",
            "y": 231
          },
          {
            "x": "train",
            "y": 63
          },
          {
            "x": "subway",
            "y": 227
          },
          {
            "x": "bus",
            "y": 24
          },
          {
            "x": "car",
            "y": 118
          },
          {
            "x": "moto",
            "y": 204
          },
          {
            "x": "bicycle",
            "y": 276
          },
          {
            "x": "horse",
            "y": 209
          },
          {
            "x": "skateboard",
            "y": 228
          },
          {
            "x": "others",
            "y": 127
          }
        ]
      },
      {
        "id": "france",
        "color": "hsl(220, 70%, 50%)",
        "data": [
          {
            "x": "plane",
            "y": 29
          },
          {
            "x": "helicopter",
            "y": 215
          },
          {
            "x": "boat",
            "y": 51
          },
          {
            "x": "train",
            "y": 230
          },
          {
            "x": "subway",
            "y": 30
          },
          {
            "x": "bus",
            "y": 184
          },
          {
            "x": "car",
            "y": 222
          },
          {
            "x": "moto",
            "y": 56
          },
          {
            "x": "bicycle",
            "y": 31
          },
          {
            "x": "horse",
            "y": 279
          },
          {
            "x": "skateboard",
            "y": 157
          },
          {
            "x": "others",
            "y": 201
          }
        ]
      },
      {
        "id": "us",
        "color": "hsl(78, 70%, 50%)",
        "data": [
          {
            "x": "plane",
            "y": 100
          },
          {
            "x": "helicopter",
            "y": 8
          },
          {
            "x": "boat",
            "y": 100
          },
          {
            "x": "train",
            "y": 34
          },
          {
            "x": "subway",
            "y": 248
          },
          {
            "x": "bus",
            "y": 197
          },
          {
            "x": "car",
            "y": 111
          },
          {
            "x": "moto",
            "y": 63
          },
          {
            "x": "bicycle",
            "y": 115
          },
          {
            "x": "horse",
            "y": 186
          },
          {
            "x": "skateboard",
            "y": 123
          },
          {
            "x": "others",
            "y": 111
          }
        ]
      },
      {
        "id": "germany",
        "color": "hsl(249, 70%, 50%)",
        "data": [
          {
            "x": "plane",
            "y": 268
          },
          {
            "x": "helicopter",
            "y": 22
          },
          {
            "x": "boat",
            "y": 268
          },
          {
            "x": "train",
            "y": 46
          },
          {
            "x": "subway",
            "y": 217
          },
          {
            "x": "bus",
            "y": 82
          },
          {
            "x": "car",
            "y": 272
          },
          {
            "x": "moto",
            "y": 289
          },
          {
            "x": "bicycle",
            "y": 74
          },
          {
            "x": "horse",
            "y": 80
          },
          {
            "x": "skateboard",
            "y": 144
          },
          {
            "x": "others",
            "y": 143
          }
        ]
      },
      {
        "id": "norway",
        "color": "hsl(330, 70%, 50%)",
        "data": [
          {
            "x": "plane",
            "y": 85
          },
          {
            "x": "helicopter",
            "y": 123
          },
          {
            "x": "boat",
            "y": 193
          },
          {
            "x": "train",
            "y": 274
          },
          {
            "x": "subway",
            "y": 163
          },
          {
            "x": "bus",
            "y": 48
          },
          {
            "x": "car",
            "y": 258
          },
          {
            "x": "moto",
            "y": 124
          },
          {
            "x": "bicycle",
            "y": 59
          },
          {
            "x": "horse",
            "y": 77
          },
          {
            "x": "skateboard",
            "y": 140
          },
          {
            "x": "others",
            "y": 173
          }
        ]
      }
    ]

    return (
      <div>
        <h1>Counter</h1>
        <div>Id: {symbol}</div>
        <p>This is a simple example of a React component.</p>

        <p aria-live="polite">Current count: <strong>{this.state.currentCount}</strong></p>

        <button className="btn btn-primary" onClick={this.incrementCounter}>Increment</button>
        {this.renderChart(data)}
      </div>
    );
  }
}
