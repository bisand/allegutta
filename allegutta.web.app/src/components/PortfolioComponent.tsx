import React, { Component } from 'react';
import axios from 'axios'
import { format } from 'date-fns'
import { HubConnectionBuilder } from '@microsoft/signalr';

export class PortfolioComponent extends Component<any, any> {
  static displayName = PortfolioComponent.name;
  private _hubConnection: any;

  constructor(props: any) {
    super(props);
    this.state = { portfolio: {}, loading: true, portfolioUpdated: new Date(), search: '' };

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
      this.setState({ portfolio: portfolio, loading: false, portfolioUpdated: new Date(), search: '' });
      console.log('Portfolio updated.');
    });
  }

  private submitHandler(e: any) {
    e.preventDefault();
  }

  private setSearchValue(e: any) {
    console.log(e);
    this.setState({search:''})
  }

  private renderPortfolioSummary(portfolio: any) {
    return (
      <div className="card mb-3">
        <div className="card-header">
          Porteføljens verdi
        </div>
        <div className="card-body">
        </div>
      </div>
    );
  }

  private renderPortfolioControls(portfolio: any) {
    return (
      <div className="card mb-3">
        <div className="card-body">
          <form onSubmit={this.submitHandler}>
            <input className='form-control' placeholder="Search" list="datalistOptions" id="search-in-table" onKeyUp={this.setSearchValue} />
            <datalist id="datalistOptions">
              {portfolio.positions.map((item: any, key: any) =>
                <option key={key} value={item.name} />
              )}
            </datalist>
          </form>
        </div>
      </div>
    );
  }

  private renderPositionsTable(portfolio: any) {
    return (
      <div className="card mb-3">
        <div className="card-header">
          Beholdning
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
              {portfolio?.positions?.filter((x: any) => x.name.toLowerCase().includes(this.state.search.toLowerCase())).map((position: any) =>
                <tr key={position.id}>
                  <td>{position.name}</td>
                  <td>{position.symbol}</td>
                  <td className='text-end'>{position.shares.toLocaleString('nb-NO', { maximumFractionDigits: 0 })}</td>
                  <td className='text-end'>{position.avgPrice.toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</td>
                  <td className='text-end'>{position.costValue.toLocaleString('nb-NO', { maximumFractionDigits: 0 })}</td>
                  <td className={'text-end ' + (position.changeTodayPercent >= 0 ? 'text-success' : 'text-danger')}>{position.changeTodayPercent.toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</td>
                  <td className='text-end'>{position.lastPrice.toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</td>
                  <td className='text-end'>{position.currentValue.toLocaleString('nb-NO', { maximumFractionDigits: 0 })}</td>
                  <td className={'text-end ' + (position.return >= 0 ? 'text-success' : 'text-danger')}>{position.return.toLocaleString('nb-NO', { maximumFractionDigits: 0 })}</td>
                  <td className={'text-end ' + (position.returnPercent >= 0 ? 'text-success' : 'text-danger')}>{position.returnPercent.toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</td>
                </tr>
              )}
            </tbody>
            <tfoot>
              <tr>
                <th colSpan={10}>Portfolio updated {format(this.state.portfolioUpdated, 'yyyy-MM-dd HH:mm:ss')}</th>
              </tr>
            </tfoot>
          </table>
        </div>
      </div>
    );
  }

  render() {
    let summary = this.state.loading
      ? <div className="spinner-border" role="status">
        <span className="visually-hidden">Loading...</span>
      </div>
      : this.renderPortfolioSummary(this.state.portfolio);
    let positions = this.state.loading
      ? <div className="spinner-border" role="status">
        <span className="visually-hidden">Loading...</span>
      </div>
      : this.renderPositionsTable(this.state.portfolio);
    let controls = this.state.loading
      ? <div className="spinner-border" role="status">
        <span className="visually-hidden">Loading...</span>
      </div>
      : this.renderPortfolioControls(this.state.portfolio)
    return (
      <div>
        <h1 id="tableLabel">Portefølje - AlleGutta</h1>
        <p>This component demonstrates fetching data from the server.</p>
        {summary}
        {controls}
        {positions}
      </div>
    );
  }

  async fetchWeather() {
    try {
      this.setState({ ...this.state, loading: true });
      const response = await axios.get('api/portfolio/allegutta');
      this.setState({ portfolio: response.data, loading: false });
    } catch (e) {
      console.log(e);
      this.setState({ ...this.state, loading: false });
    }
  }
  async populateWeatherData() {
    const response = await fetch('api/weatherforecast');
    const data = await response.json();
    this.setState({ portfolio: data, loading: false });
  }
}
