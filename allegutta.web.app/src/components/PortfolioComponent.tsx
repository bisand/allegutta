import React, { Component } from 'react';
import axios from 'axios'
import { format } from 'date-fns'
import { HubConnectionBuilder } from '@microsoft/signalr';

export class PortfolioComponent extends Component<any, any> {
  static displayName = PortfolioComponent.name;
  private _hubConnection: any;
  private _didMount = false;
  private _sortProperty: string;
  private _sortOrder: any = {};
  private _dataSummary: any = {};

  constructor(props: any) {
    super(props);
    this.state = { portfolio: {}, loading: true, portfolioUpdated: new Date(), search: '' };

    window.onbeforeunload = (event) => {
      console.log(event);
      this._hubConnection.stop();
    };
    this._hubConnection = new HubConnectionBuilder()
      .withUrl('/hubs/portfolio')
      .withAutomaticReconnect()
      .build();
    if (this._hubConnection) {
      this._hubConnection.serverTimeoutInMilliseconds = 100000; // 100 second
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
    console.log(e);
    const searchBox: any = document.getElementById("search-in-table");
    const searchText: string = searchBox.value;
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
        <div className="d-flex justify-content-around flex-wrap">
          <div className="flex-item-3">
            <p className='fs-4 text-secondary mb-0'>Egenkapital</p>
            <p className='fs-3'>
              {portfolio.equity.toLocaleString('nb-NO', { maximumFractionDigits: 0 })},-
            </p>
          </div>
          <div className="flex-item-3">
            <p className='fs-4 text-secondary mb-0'>Markedsverdi</p>
            <p className='fs-3'>
              {portfolio.marketValue.toLocaleString('nb-NO', { maximumFractionDigits: 0 })},-
            </p>
          </div>
          <div className="flex-item-3 d-sm-block d-md-inline">
            <p className='fs-4 text-secondary mb-0'>ATH</p>
            <p className='fs-3'>
              {portfolio.ath.toLocaleString('nb-NO', { maximumFractionDigits: 0 })},-
            </p>
          </div>
          <div className="flex-item-3">
            <p className='fs-4 text-secondary mb-0'>Cash</p>
            <p className='fs-3'>
              {portfolio.cash.toLocaleString('nb-NO', { maximumFractionDigits: 0 })},-
            </p>
          </div>
          <div className="flex-item-3 d-sm-block d-md-inline">
            <p className='fs-4 text-secondary mb-0'>Endring i dag</p>
            <p className={'fs-3 ' + (portfolio.changeTodayPercent >= 0 ? "text-success" : "text-danger")}>
              <span className='text-nowrap'>
                <i className={"bi " + (portfolio.changeTodayPercent >= 0 ? "bi-graph-up-arrow" : "bi-graph-down-arrow")}>&nbsp;</i>
                {portfolio.changeTodayPercent.toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} %
              </span>
              <span className='d-none d-lg-inline'> | </span><br className='d-sm-inline d-lg-none' />
              <span className='text-nowrap'>{portfolio.changeTodayTotal.toLocaleString('nb-NO', { maximumFractionDigits: 0 })},-</span>
            </p>
          </div>
          <div className="flex-item-3 d-sm-block d-md-inline">
            <p className='fs-4 text-secondary mb-0'>Endring total</p>
            <p className={'fs-3 ' + (portfolio.changeTotalPercent >= 0 ? "text-success" : "text-danger")}>
              <span className='text-nowrap'>
                <i className={"bi " + (portfolio.changeTotalPercent >= 0 ? "bi-graph-up-arrow" : "bi-graph-down-arrow")}>&nbsp;</i>
                {portfolio.changeTotalPercent.toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} %
              </span>
              <span className='d-none d-lg-inline'> | </span><br className='d-sm-inline d-lg-none' />
              <span className='text-nowrap'>{portfolio.changeTotal.toLocaleString('nb-NO', { maximumFractionDigits: 0 })},-</span>
            </p>
          </div>
        </div>
      </div>
    );
  }

  private renderPortfolioControls(portfolio: any) {
    return (
      <div className="card mb-3">
        <div className="card-body">
          <form onSubmit={this.submitHandler}>
            <input className='form-control' placeholder="Search" list="datalistOptions" id="search-in-table" onKeyUp={this.setSearchValue} autoComplete="off" />
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
    this._dataSummary = {};
    return (
      <table className="table table-striped table-hover" aria-labelledby="tableLabel" id="portfolio-positions-table">
        <thead className=''>
          <tr style={{ cursor: "pointer" }}>
            <th className='d-none d-xl-table-cell text-nowrap' onClick={e => this.sortClick(e, "name")}>
              Navn
              <span className="ms-1" style={{ display: "inline-block" }}>
                <i className={"bi " + (this._sortOrder[this._sortProperty] === "asc" ? "bi-sort-down-alt " : "bi-sort-up ") + (this._sortProperty !== "name" ? "d-none" : "d-inline")}>&nbsp;</i>
                <i className={"bi bi-chevron-expand text-secondary " + (this._sortProperty === "name" ? "d-none" : "d-inline")}>&nbsp;</i>
              </span>
            </th>
            <th className='text-nowrap' onClick={e => this.sortClick(e, "symbol")}>
              Ticker
              <span className="ms-1" style={{ display: "inline-block" }}>
                <i className={"bi " + (this._sortOrder[this._sortProperty] === "asc" ? "bi-sort-down-alt " : "bi-sort-up ") + (this._sortProperty !== "symbol" ? "d-none" : "d-inline")}>&nbsp;</i>
                <i className={"bi bi-chevron-expand text-secondary " + (this._sortProperty === "symbol" ? "d-none" : "d-inline")}>&nbsp;</i>
              </span>
            </th>
            <th className='d-none d-md-table-cell text-end text-nowrap' onClick={e => this.sortClick(e, "shares")}>
              Antall
              <span className="ms-1" style={{ display: "inline-block" }}>
                <i className={"bi " + (this._sortOrder[this._sortProperty] === "asc" ? "bi-sort-down-alt " : "bi-sort-up ") + (this._sortProperty !== "shares" ? "d-none" : "d-inline")}>&nbsp;</i>
                <i className={"bi bi-chevron-expand text-secondary " + (this._sortProperty === "shares" ? "d-none" : "d-inline")}>&nbsp;</i>
              </span>
            </th>
            <th className='d-none d-lg-table-cell text-end text-nowrap' onClick={e => this.sortClick(e, "avgPrice")}>
              GAV
              <span className="ms-1" style={{ display: "inline-block" }}>
                <i className={"bi " + (this._sortOrder[this._sortProperty] === "asc" ? "bi-sort-down-alt " : "bi-sort-up ") + (this._sortProperty !== "avgPrice" ? "d-none" : "d-inline")}>&nbsp;</i>
                <i className={"bi bi-chevron-expand text-secondary " + (this._sortProperty === "avgPrice" ? "d-none" : "d-inline")}>&nbsp;</i>
              </span>
            </th>
            <th className='d-none d-lg-table-cell text-end text-nowrap' onClick={e => this.sortClick(e, "costValue")}>
              Kost
              <span className="ms-1" style={{ display: "inline-block" }}>
                <i className={"bi " + (this._sortOrder[this._sortProperty] === "asc" ? "bi-sort-down-alt " : "bi-sort-up ") + (this._sortProperty !== "costValue" ? "d-none" : "d-inline")}>&nbsp;</i>
                <i className={"bi bi-chevron-expand text-secondary " + (this._sortProperty === "costValue" ? "d-none" : "d-inline")}>&nbsp;</i>
              </span>
            </th>
            <th className='text-end text-nowrap' onClick={e => this.sortClick(e, "changeTodayPercent")}>
              Dag %
              <span className="ms-1" style={{ display: "inline-block" }}>
                <i className={"bi " + (this._sortOrder[this._sortProperty] === "asc" ? "bi-sort-down-alt " : "bi-sort-up ") + (this._sortProperty !== "changeTodayPercent" ? "d-none" : "d-inline")}>&nbsp;</i>
                <i className={"bi bi-chevron-expand text-secondary " + (this._sortProperty === "changeTodayPercent" ? "d-none" : "d-inline")}>&nbsp;</i>
              </span>
            </th>
            <th className='d-none d-xs-table-cell text-end text-nowrap' onClick={e => this.sortClick(e, "lastPrice")}>
              Siste
              <span className="ms-1" style={{ display: "inline-block" }}>
                <i className={"bi " + (this._sortOrder[this._sortProperty] === "asc" ? "bi-sort-down-alt " : "bi-sort-up ") + (this._sortProperty !== "lastPrice" ? "d-none" : "d-inline")}>&nbsp;</i>
                <i className={"bi bi-chevron-expand text-secondary " + (this._sortProperty === "lastPrice" ? "d-none" : "d-inline")}>&nbsp;</i>
              </span>
            </th>
            <th className='d-none d-lg-table-cell text-end text-nowrap' onClick={e => this.sortClick(e, "currentValue")}>
              Verdi
              <span className="ms-1" style={{ display: "inline-block" }}>
                <i className={"bi " + (this._sortOrder[this._sortProperty] === "asc" ? "bi-sort-down-alt " : "bi-sort-up ") + (this._sortProperty !== "currentValue" ? "d-none" : "d-inline")}>&nbsp;</i>
                <i className={"bi bi-chevron-expand text-secondary " + (this._sortProperty === "currentValue" ? "d-none" : "d-inline")}>&nbsp;</i>
              </span>
            </th>
            <th className='d-none d-md-table-cell text-end text-nowrap' onClick={e => this.sortClick(e, "returnValue")}>
              <span className='d-none d-lg-inline'>Avkastning</span>
              <span className='d-lg-none'>Avk.</span>
              <span className="ms-1" style={{ display: "inline-block" }}>
                <i className={"bi " + (this._sortOrder[this._sortProperty] === "asc" ? "bi-sort-down-alt " : "bi-sort-up ") + (this._sortProperty !== "returnValue" ? "d-none" : "d-inline")}>&nbsp;</i>
                <i className={"bi bi-chevron-expand text-secondary " + (this._sortProperty === "returnValue" ? "d-none" : "d-inline")}>&nbsp;</i>
              </span>
            </th>
            <th className='text-end text-nowrap' onClick={e => this.sortClick(e, "returnPercent")}>
              <span className='d-none d-lg-inline'>Avkastning %</span>
              <span className='d-sm-inline d-lg-none'>Avk.%</span>
              <span className="ms-1" style={{ display: "inline-block" }}>
                <i className={"bi " + (this._sortOrder[this._sortProperty] === "asc" ? "bi-sort-down-alt " : "bi-sort-up ") + (this._sortProperty !== "returnPercent" ? "d-none" : "d-inline")}>&nbsp;</i>
                <i className={"bi bi-chevron-expand text-secondary " + (this._sortProperty === "returnPercent" ? "d-none" : "d-inline")}>&nbsp;</i>
              </span>
            </th>
          </tr>
        </thead>
        <tbody>
          {portfolio?.positions?.sort(this.sorter).filter((x: any) => x.name.toLowerCase().includes(this.state.search.toLowerCase())).map((position: any) =>
            <tr key={position.id} style={{ cursor: "default" }}>
              <td className='d-none d-xl-table-cell text-nowrap'><a href={'instrument/' + position.symbol} style={{ textDecoration: "unset", color: "unset" }}>{position.name}</a></td>
              <td title={position.name}><a href={'instrument/' + position.symbol} style={{ textDecoration: "unset", color: "unset" }}>{position.symbol}</a></td>
              <td className='d-none d-md-table-cell text-end'>{this.sumAndPresent(position, "shares", 0)}</td>
              <td className='d-none d-lg-table-cell text-end'>{this.sumAndPresent(position, "avgPrice", 2)}</td>
              <td className='d-none d-lg-table-cell text-end'>{this.sumAndPresent(position, "costValue", 2)}</td>
              <td className={'text-nowrap text-end ' + (position.changeTodayPercent >= 0 ? 'text-success' : 'text-danger')}>{position.changeTodayPercent.toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} %</td>
              <td className='d-none d-xs-table-cell text-end'>{position.lastPrice.toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</td>
              <td className='d-none d-lg-table-cell text-end'>{this.sumAndPresent(position, "currentValue", 0)},-</td>
              <td className={'d-none d-md-table-cell text-end ' + (position.returnValue >= 0 ? 'text-success' : 'text-danger')}>{this.sumAndPresent(position, "returnValue", 0)},-</td>
              <td className={'text-nowrap text-end ' + (position.returnPercent >= 0 ? 'text-success' : 'text-danger')}>{this.sumAndPresent(position, "returnPercent", 2)} %</td>
            </tr>
          )}
        </tbody>
        <tfoot>
          <tr>
            <th colSpan={2} className="d-none d-xl-table-cell fw-lighter fst-italic">Portfolio updated {format(this.state.portfolioUpdated, 'yyyy-MM-dd HH:mm:ss')}</th>
            <th className={'d-xl-none'}></th>
            <th className={'d-none d-md-table-cell text-end '}>{this.presentSum("shares", 0)}</th>
            <th className={'d-none d-lg-table-cell text-end '}></th>
            <th className={'d-none d-lg-table-cell text-end '}>{this.presentSum("costValue", 0)},-</th>
            <th></th>
            <th className='d-none d-xs-table-cell'></th>
            <th className={'d-none d-lg-table-cell text-end ' + (portfolio?.changeTotalPercent ?? 0 >= 0 ? "text-success" : "text-danger")}>
              {this.presentSum("currentValue", 0)},-
            </th>
            <th className={'d-none d-md-table-cell text-end ' + (this._dataSummary["returnValue"] >= 0 ? "text-success" : "text-danger")}>
              <i className={"bi " + (this._dataSummary["returnValue"] >= 0 ? "bi-graph-up-arrow" : "bi-graph-down-arrow")}>&nbsp;</i>
              {this.presentSum("returnValue", 0)},-
            </th>
            <th className={'text-end text-nowrap ' + (this._dataSummary["returnValue"] >= 0 ? "text-success" : "text-danger")}>
              <i className={"bi " + (this._dataSummary["returnValue"] >= 0 ? "bi-graph-up-arrow" : "bi-graph-down-arrow")}>&nbsp;</i>
              {((this._dataSummary["returnValue"] ?? 0) * 100 / (this._dataSummary["costValue"] ?? 1)).toLocaleString('nb-NO', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} %
            </th>
          </tr>
        </tfoot>
      </table>
    );
  }

  sumAndPresent(data: any, property: string, decimals: number): React.ReactNode {
    if (!this._dataSummary)
      this._dataSummary = {};
    this._dataSummary[property] = (this._dataSummary[property] ?? 0) + (data[property] ?? 0);
    this._dataSummary[property + "_count"] = (this._dataSummary[property + "_count"] ?? 0) + 1;
    return data[property].toLocaleString('nb-NO', { minimumFractionDigits: decimals, maximumFractionDigits: decimals });
  }

  presentSum(property: string, decimals: number, average = false, resetValues = false): React.ReactNode {
    const tmpSum = this._dataSummary[property] / (average && this._dataSummary[property + "_count"] && this._dataSummary[property + "_count"] > 0 ? this._dataSummary[property + "_count"] : 1);
    const result = tmpSum.toLocaleString('nb-NO', { minimumFractionDigits: decimals, maximumFractionDigits: decimals });
    if (resetValues) {
      this._dataSummary[property] = 0;
      this._dataSummary[property + "_count"] = 0;
    }
    return result;
  }

  private sortClick = (event: React.MouseEvent<HTMLTableCellElement>, sortProperty: string) => {
    event.preventDefault();

    // const cell: HTMLTableCellElement = event.currentTarget;
    if (sortProperty === this._sortProperty)
      this._sortOrder[sortProperty] = (this._sortOrder[sortProperty] ?? "desc") === "desc" ? "asc" : "desc";
    else
      this._sortOrder[sortProperty] = "asc";
    this._sortProperty = sortProperty;
    this.forceUpdate();
  };

  render() {
    const templateLoading = <div className="spinner-border" role="status"><span className="visually-hidden">Loading...</span></div>

    const summary = this.state.loading ? templateLoading : this.renderPanel(this.renderPortfolioSummary(this.state.portfolio), "Porteføljens verdi");
    const controls = this.renderPortfolioControls(this.state.portfolio);
    const positions = this.state.loading ? templateLoading : this.renderPanel(this.renderPositionsTable(this.state.portfolio), "Beholdning");

    return (
      <div>
        {summary}
        {controls}
        {positions}
        <div className="ms-3 d-xl-none fw-lighter fst-italic">Portfolio updated {format(this.state.portfolioUpdated, 'yyyy-MM-dd HH:mm:ss')}</div>
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
