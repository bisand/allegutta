import React, { Component } from 'react';
import axios from 'axios'
import { format } from 'date-fns'
import { HubConnectionBuilder } from '@microsoft/signalr';

export class PortfolioComponent extends Component<any, any> {
  static displayName = PortfolioComponent.name;
  private _hubConnection: any;
  private _didMount = false;

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
    if (this._didMount)
      return;
    this._didMount = true;

    this.fetchWeather();
    this._hubConnection.on('PortfolioUpdated', (portfolio: any) => {
      this.setState({ portfolio: portfolio, loading: false, portfolioUpdated: new Date() });
      console.log('Portfolio updated.');
    });
  }

  private submitHandler = (e: any) => {
    e.preventDefault();
  }

  private setSearchValue = (e: any) => {
    let searchBox: any = document.getElementById("search-in-table");
    let searchText: string = searchBox.value;
    this.setState({ search: searchText });
  }

  private renderPanel(contentBody: any, title: string) {
    return (
      <div className="card mb-3">
        <div className="card-header">
          {title}
        </div>
        <div className="card-body">
          {contentBody}
        </div>
      </div>
    );
  }

  private renderPortfolioSummary(portfolio: any) {
    return (
      <div className="container text-center">
        <div className="row mb-3">
          <div className="col">
            <p className='fs-4'>Egenkapital</p>
            <p className='fs-3'>
              {portfolio.equity.toLocaleString('nb-NO', { maximumFractionDigits: 0 })},-
            </p>
          </div>
          <div className="col">
            <p className='fs-4'>Markedsverdi</p>
            <p className='fs-3'>
              {portfolio.marketValue.toLocaleString('nb-NO', { maximumFractionDigits: 0 })},-
            </p>
          </div>
          <div className="col">
            <p className='fs-4'>Cash</p>
            <p className='fs-3'>
              {portfolio.cash.toLocaleString('nb-NO', { maximumFractionDigits: 0 })},-
            </p>
          </div>
        </div>
        <div className="row">
          <div className="col">
            <p className='fs-4'>ATH</p>
            <p className='fs-3'>
              {portfolio.ath.toLocaleString('nb-NO', { maximumFractionDigits: 0 })},-
            </p>
          </div>
          <div className="col">
            <p className='fs-4'>Endring total</p>
            <p className={'fs-3 ' + (portfolio.changeTotalPercent >= 0 ? "text-success" : "text-danger")}>
            <i className={"bi " + (portfolio.changeTotalPercent >= 0 ? "bi-graph-up-arrow" : "bi-graph-down-arrow")}>&nbsp;</i>
              {portfolio.changeTotalPercent.toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} % | {portfolio.changeTotal.toLocaleString('nb-NO', { maximumFractionDigits: 0 })},-
            </p>
          </div>
          <div className="col">
            <p className='fs-4'>Endring i dag</p>
            <p className={'fs-3 ' + (portfolio.changeTodayPercent >= 0 ? "text-success" : "text-danger")}>
              <i className={"bi " + (portfolio.changeTodayPercent >= 0 ? "bi-graph-up-arrow" : "bi-graph-down-arrow")}>&nbsp;</i>
              {portfolio.changeTodayPercent.toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} % | {portfolio.changeTodayTotal.toLocaleString('nb-NO', { maximumFractionDigits: 0 })},-
            </p>
          </div>
        </div>
      </div>);
  }

  private renderPortfolioControls(portfolio: any) {
    return (
      <div className="card mb-3">
        <div className="card-body">
          <form onSubmit={this.submitHandler}>
            <input className='form-control' placeholder="Search" list="datalistOptions" id="search-in-table" onKeyUp={this.setSearchValue} />
            <datalist id="datalistOptions">
              {portfolio?.positions?.map((item: any, key: any) =>
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
              <td className={'text-end ' + (position.changeTodayPercent >= 0 ? 'text-success' : 'text-danger')}>{position.changeTodayPercent.toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} %</td>
              <td className='text-end'>{position.lastPrice.toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</td>
              <td className='text-end'>{position.currentValue.toLocaleString('nb-NO', { maximumFractionDigits: 0 })}</td>
              <td className={'text-end ' + (position.return >= 0 ? 'text-success' : 'text-danger')}>{position.return.toLocaleString('nb-NO', { maximumFractionDigits: 0 })}</td>
              <td className={'text-end ' + (position.returnPercent >= 0 ? 'text-success' : 'text-danger')}>{position.returnPercent.toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} %</td>
            </tr>
          )}
        </tbody>
        <tfoot>
          <tr>
            <th colSpan={10}>Portfolio updated {format(this.state.portfolioUpdated, 'yyyy-MM-dd HH:mm:ss')}</th>
          </tr>
        </tfoot>
      </table>
    );
  }

  render() {
    let templateLoading = <p><div className="spinner-border" role="status"><span className="visually-hidden">Loading...</span></div></p>

    let summary = this.state.loading ? templateLoading : this.renderPanel(this.renderPortfolioSummary(this.state.portfolio), "Porteføljens verdi");
    let controls = this.renderPortfolioControls(this.state.portfolio);
    let positions = this.state.loading ? templateLoading : this.renderPanel(this.renderPositionsTable(this.state.portfolio), "Beholdning");

    return (
      <div>
        {/* <h1 id="tableLabel">Portefølje - AlleGutta</h1>
        <p>This component demonstrates fetching data from the server.</p> */}
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
