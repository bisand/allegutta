import React, { Component } from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';

export class NavMenu extends Component<any, any> {
  static displayName = NavMenu.name;
  private _renderedScript = false;

  constructor(props: any) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = {
      collapsed: true
    };
  }

  toggleNavbar() {
    this.setState({
      collapsed: !this.state.collapsed
    });
  }

  componentDidMount() {
    if (this._renderedScript)
      return;
    this._renderedScript = true;

    const script = document.createElement("script");
    script.innerHTML = `
      function toggleDarkModeButtons() {
        if(darkmode.inDarkMode) {
          document.querySelector("#darkmode-sun").classList.remove("d-none");
          document.querySelector("#darkmode-moon").classList.add("d-none");
        } else {
          document.querySelector("#darkmode-sun").classList.add("d-none");
          document.querySelector("#darkmode-moon").classList.remove("d-none");
        }
      }
      setTimeout(()=>{
        toggleDarkModeButtons();
        document.querySelector("#darkmode-button").onclick = function(e){
          darkmode.toggleDarkMode();
          toggleDarkModeButtons();

        }
      }, 100);
    `;
    script.async = true;
    document.body.appendChild(script);
  }

  render() {
    return (
      <header>
        <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" container light>
          <NavbarBrand tag={Link} to="/">AlleGutta!</NavbarBrand>
          <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
          <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
            <ul className="navbar-nav flex-grow">
              <NavItem>
                <NavLink tag={Link} to="/">Hjem</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} to="/">Nyheter</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} to="/counter">Vedtekter</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} to="/portfolio">Portefølje</NavLink>
              </NavItem>
            </ul>
          </Collapse>
          <a id="darkmode-button" className="btn btn-outline-secondary">
            <i id="darkmode-moon" className="fa fa-moon-o fa-fw d-none d-light-inline" title="Switch to dark mode"></i>
            <i id="darkmode-sun" className="fa fa-sun-o fa-fw d-none d-dark-inline" title="Switch to light mode"></i>
          </a>
        </Navbar>
      </header>
    );
  }
}
