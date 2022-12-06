import React, { Component } from 'react';
import axios from 'axios'
export class FetchData extends Component<any, any> {
  static displayName = FetchData.name;

  constructor(props: any) {
    super(props);
    this.state = { forecasts: [], loading: true };
  }

  componentDidMount() {
    this.fetchWeather();
  }

  static renderForecastsTable(forecasts: any) {
    return (
      <div className="card">
        <div className="card-header">
          Portfolio
        </div>
        <div className="card-body">
          <table className="table table-striped table-hover" aria-labelledby="tableLabel">
            <thead>
              <tr>
                <th>Id</th>
                <th>Ticker</th>
                <th>Navn</th>
                <th className='text-end'>Antall</th>
                <th className='text-end'>Innkjøpspris</th>
              </tr>
            </thead>
            <tbody>
              {forecasts.map((forecast: any) =>
                <tr key={forecast.id}>
                  <td>{forecast.id}</td>
                  <td>{forecast.symbol}</td>
                  <td>{forecast.name}</td>
                  <td className='text-end'>{forecast.shares}</td>
                  <td className='text-end'>{forecast.avgPrice.toFixed(2)}</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : FetchData.renderForecastsTable(this.state.forecasts);

    return (
      <div>
        <h1 id="tableLabel">Weather forecast</h1>
        <p>This component demonstrates fetching data from the server.</p>
        {contents}
      </div>
    );
  }

  async fetchWeather() {
    try {
      this.setState({ ...this.state, loading: true });
      const response = await axios.get('portfolio/allegutta');
      this.setState({ forecasts: response.data.positions, loading: false });
    } catch (e) {
      console.log(e);
      this.setState({ ...this.state, loading: false });
    }
  }
  async populateWeatherData() {
    const response = await fetch('weatherforecast');
    const data = await response.json();
    this.setState({ forecasts: data, loading: false });
  }
}
