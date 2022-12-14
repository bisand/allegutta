import React, { Component } from 'react';
import axios from 'axios'
import { format } from 'date-fns'
import { HubConnectionBuilder } from '@microsoft/signalr';
import { Link, NavLink } from 'react-router-dom';

export class PortfolioComponent extends Component<any, any> {
  static displayName = PortfolioComponent.name;
  private _hubConnection: any;
  private _didMount = false;
  private _sortProperty: string;
  private _sortOrder: any = {};

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
    this._sortProperty = "name";
    this._sortOrder[this._sortProperty] = "asc";
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
            <p className='fs-4 text-secondary'>Egenkapital</p>
            <p className='fs-3'>
              {portfolio.equity.toLocaleString('nb-NO', { maximumFractionDigits: 0 })},-
            </p>
          </div>
          <div className="col">
            <p className='fs-4 text-secondary'>Markedsverdi</p>
            <p className='fs-3'>
              {portfolio.marketValue.toLocaleString('nb-NO', { maximumFractionDigits: 0 })},-
            </p>
          </div>
          <div className="col">
            <p className='fs-4 text-secondary'>Cash</p>
            <p className='fs-3'>
              {portfolio.cash.toLocaleString('nb-NO', { maximumFractionDigits: 0 })},-
            </p>
          </div>
        </div>
        <div className="row">
          <div className="col">
            <p className='fs-4 text-secondary'>ATH</p>
            <p className='fs-3'>
              {portfolio.ath.toLocaleString('nb-NO', { maximumFractionDigits: 0 })},-
            </p>
          </div>
          <div className="col">
            <p className='fs-4 text-secondary'>Endring total</p>
            <p className={'fs-3 ' + (portfolio.changeTotalPercent >= 0 ? "text-success" : "text-danger")}>
              <i className={"bi " + (portfolio.changeTotalPercent >= 0 ? "bi-graph-up-arrow" : "bi-graph-down-arrow")}>&nbsp;</i>
              {portfolio.changeTotalPercent.toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} % | {portfolio.changeTotal.toLocaleString('nb-NO', { maximumFractionDigits: 0 })},-
            </p>
          </div>
          <div className="col">
            <p className='fs-4 text-secondary'>Endring i dag</p>
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

  private sorter = (a: any, b: any): number => {
    let fa = a[this._sortProperty];
    let fb = b[this._sortProperty];
    if (typeof a[this._sortProperty] === 'string') {
      fa = a[this._sortProperty].toLowerCase();
      fb = b[this._sortProperty].toLowerCase();
    }

    if (fa < fb && this._sortOrder[this._sortProperty] === "asc") {
      return -1;
    } else if (fa < fb && this._sortOrder[this._sortProperty] === "desc") {
      return 1;
    }
    if (fa > fb && this._sortOrder[this._sortProperty] === "asc") {
      return 1;
    } else if (fa > fb && this._sortOrder[this._sortProperty] === "desc") {
      return -1;
    }
    return 0;
  };

  private renderPositionsTable(portfolio: any) {
    return (
      <table className="table table-striped table-hover" aria-labelledby="tableLabel" id="portfolio-positions-table">
        <thead>
          <tr style={{ cursor: "pointer" }}>
            <th onClick={e => this.sortClick(e, "name")}>
              Navn&nbsp;
              <span style={{ width: 21, display: "inline-block" }}>
                <i className={"bi " + (this._sortOrder[this._sortProperty] === "asc" ? "bi-sort-alpha-down " : "bi-sort-alpha-up-alt ") + (this._sortProperty !== "name" ? "d-none" : "d-inline")}>&nbsp;</i>
              </span>
            </th>
            <th onClick={e => this.sortClick(e, "symbol")}>
              Ticker&nbsp;
              <span style={{ width: 21, display: "inline-block" }}>
                <i className={"bi " + (this._sortOrder[this._sortProperty] === "asc" ? "bi-sort-alpha-down " : "bi-sort-alpha-up-alt ") + (this._sortProperty !== "symbol" ? "d-none" : "d-inline")}>&nbsp;</i>
              </span>
            </th>
            <th className='text-end' onClick={e => this.sortClick(e, "shares")}>
              <span style={{ width: 21, display: "inline-block" }}>
                <i className={"bi " + (this._sortOrder[this._sortProperty] === "asc" ? "bi-sort-numeric-down " : "bi-sort-numeric-up-alt ") + (this._sortProperty !== "shares" ? "d-none" : "d-inline")}>&nbsp;</i>
              </span>
              Antall
            </th>
            <th className='text-end' onClick={e => this.sortClick(e, "avgPrice")}>
              <span style={{ width: 21, display: "inline-block" }}>
                <i className={"bi " + (this._sortOrder[this._sortProperty] === "asc" ? "bi-sort-numeric-down " : "bi-sort-numeric-up-alt ") + (this._sortProperty !== "avgPrice" ? "d-none" : "d-inline")}>&nbsp;</i>
              </span>
              GAV
            </th>
            <th className='text-end' onClick={e => this.sortClick(e, "costValue")}>
              <span style={{ width: 21, display: "inline-block" }}>
                <i className={"bi " + (this._sortOrder[this._sortProperty] === "asc" ? "bi-sort-numeric-down " : "bi-sort-numeric-up-alt ") + (this._sortProperty !== "costValue" ? "d-none" : "d-inline")}>&nbsp;</i>
              </span>
              Kost
            </th>
            <th className='text-end' onClick={e => this.sortClick(e, "changeTodayPercent")}>
              <span style={{ width: 21, display: "inline-block" }}>
                <i className={"bi " + (this._sortOrder[this._sortProperty] === "asc" ? "bi-sort-numeric-down " : "bi-sort-numeric-up-alt ") + (this._sortProperty !== "changeTodayPercent" ? "d-none" : "d-inline")}>&nbsp;</i>
              </span>
              I dag %
            </th>
            <th className='text-end' onClick={e => this.sortClick(e, "lastPrice")}>
              <span style={{ width: 21, display: "inline-block" }}>
                <i className={"bi " + (this._sortOrder[this._sortProperty] === "asc" ? "bi-sort-numeric-down " : "bi-sort-numeric-up-alt ") + (this._sortProperty !== "lastPrice" ? "d-none" : "d-inline")}>&nbsp;</i>
              </span>
              Siste
            </th>
            <th className='text-end' onClick={e => this.sortClick(e, "currentValue")}>
              <span style={{ width: 21, display: "inline-block" }}>
                <i className={"bi " + (this._sortOrder[this._sortProperty] === "asc" ? "bi-sort-numeric-down " : "bi-sort-numeric-up-alt ") + (this._sortProperty !== "currentValue" ? "d-none" : "d-inline")}>&nbsp;</i>
              </span>
              Verdi
            </th>
            <th className='text-end' onClick={e => this.sortClick(e, "return")}>
              <span style={{ width: 21, display: "inline-block" }}>
                <i className={"bi " + (this._sortOrder[this._sortProperty] === "asc" ? "bi-sort-numeric-down " : "bi-sort-numeric-up-alt ") + (this._sortProperty !== "return" ? "d-none" : "d-inline")}>&nbsp;</i>
              </span>
              Avkastning
            </th>
            <th className='text-end' onClick={e => this.sortClick(e, "returnPercent")}>
              <span style={{ width: 21, display: "inline-block" }}>
                <i className={"bi " + (this._sortOrder[this._sortProperty] === "asc" ? "bi-sort-numeric-down " : "bi-sort-numeric-up-alt ") + (this._sortProperty !== "returnPercent" ? "d-none" : "d-inline")}>&nbsp;</i>
              </span>
              Avkastning %
            </th>
          </tr>
        </thead>
        <tbody>
          {portfolio?.positions?.sort(this.sorter).filter((x: any) => x.name.toLowerCase().includes(this.state.search.toLowerCase())).map((position: any) =>
            <tr key={position.id} style={{ cursor: "default" }}>
              <td><a href={'instrument/' + position.symbol} style={{ textDecoration: "unset", color: "unset" }}>{position.name}</a></td>
              <td><a href={'instrument/' + position.symbol} style={{ textDecoration: "unset", color: "unset" }}>{position.symbol}</a></td>
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

  private sortClick = (event: React.MouseEvent<HTMLTableCellElement>, sortProperty: string) => {
    event.preventDefault();

    const cell: HTMLTableCellElement = event.currentTarget;
    this._sortProperty = sortProperty;
    this._sortOrder[this._sortProperty] = (this._sortOrder[this._sortProperty] ?? "desc") === "desc" ? "asc" : "desc";
    this.forceUpdate();
  };

  render() {
    let templateLoading = <div className="spinner-border" role="status"><span className="visually-hidden">Loading...</span></div>

    let summary = this.state.loading ? templateLoading : this.renderPanel(this.renderPortfolioSummary(this.state.portfolio), "Porteføljens verdi");
    let controls = this.renderPortfolioControls(this.state.portfolio);
    let positions = this.state.loading ? templateLoading : this.renderPanel(this.renderPositionsTable(this.state.portfolio), "Beholdning");

    return (
      <div>
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
