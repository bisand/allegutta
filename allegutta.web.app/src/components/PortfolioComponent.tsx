import React, { Component } from 'react';
import axios from 'axios'
import { HubConnectionBuilder } from '@microsoft/signalr';

export class PortfolioComponent extends Component<any, any> {
  static displayName = PortfolioComponent.name;
  private _hubConnection: any;

  constructor(props: any) {
    super(props);
    this.state = { portfolioPositions: [], loading: true, portfolioUpdated: Date.now() };

    window.onbeforeunload = (event) => {
      this._hubConnection.stop();
    };
    this._hubConnection = new HubConnectionBuilder()
      .withUrl('/hubs/portfolio')
      .withAutomaticReconnect()
      .build();
    this._hubConnection.serverTimeoutInMilliseconds = 100000; // 100 second
    if (this._hubConnection) {
      this._hubConnection.start()
        .then(() => {
          console.log('Connected!');
        })
        .catch((e: any) => console.log('Connection failed: ', e));
    }
  }

  componentDidMount() {
    this.fetchWeather();

    this._hubConnection.on('PortfolioUpdated', (portfolio: any) => {
      this.setState({ portfolioPositions: portfolio.positions, loading: false, portfolioUpdated: Date.now() });
      console.log('Portfolio updated.');
    });
  }

  private renderForecastsTable(portfolioPositions: any) {
    return (
      <div className="card">
        <div className="card-header">
          Portfolio
        </div>
        <div className="card-body">
          <table className="table table-striped table-hover" aria-labelledby="tableLabel">
            <thead>
              <tr>
                <th>Navn</th>
                <th>Ticker</th>
                <th className='text-end'>Antall</th>
                <th className='text-end'>GAV</th>
                <th className='text-end'>Kost</th>
                <th className='text-end'>I dag %</th>
                <th className='text-end'>Siste</th>
                <th className='text-end'>Verdi</th>
                <th className='text-end'>Avkastning</th>
                <th className='text-end'>Avkastning %</th>
              </tr>
            </thead>
            <tbody>
              {portfolioPositions.map((position: any) =>
                <tr key={position.id}>
                  <td>{position.name}</td>
                  <td>{position.symbol}</td>
                  <td className='text-end'>{position.shares.toLocaleString('nb-NO', { maximumFractionDigits: 0 })}</td>
                  <td className='text-end'>{position.avgPrice.toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</td>
                  <td className='text-end'>{position.costValue.toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</td>
                  <td className='text-end'>{position.changeTodayPercent.toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</td>
                  <td className='text-end'>{position.lastPrice.toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</td>
                  <td className='text-end'>{position.currentValue.toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</td>
                  <td className='text-end'>{position.return.toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</td>
                  <td className='text-end'>{position.returnPercent.toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</td>
                </tr>
              )}
            </tbody>
            <tfoot>
              <tr>
                <th>Portfolio updated {this.state.portfolioUpdated.toLocaleString('nb-NO')}</th>
              </tr>
            </tfoot>
          </table>
        </div>
      </div>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : this.renderForecastsTable(this.state.portfolioPositions);

    return (
      <div>
        <h1 id="tableLabel">Portefølje - AlleGutta</h1>
        <p>This component demonstrates fetching data from the server.</p>
        {contents}
      </div>
    );
  }

  async fetchWeather() {
    try {
      this.setState({ ...this.state, loading: true });
      const response = await axios.get('api/portfolio/allegutta');
      this.setState({ portfolioPositions: response.data.positions, loading: false });
    } catch (e) {
      console.log(e);
      this.setState({ ...this.state, loading: false });
    }
  }
  async populateWeatherData() {
    const response = await fetch('api/weatherforecast');
    const data = await response.json();
    this.setState({ portfolioPositions: data, loading: false });
  }
}
