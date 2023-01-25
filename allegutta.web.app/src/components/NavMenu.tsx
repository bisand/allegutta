/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { Component } from 'react';
import { Collapse, Nav, Navbar, NavbarBrand, NavbarText, NavbarToggler, NavItem, NavLink } from 'reactstrap';
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

  closeNavbar() {
    if (this.state.collapsed !== true) {
      this.toggleNavbar();
    }
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
        <Navbar className="navbar navbar-expand-lg" container light>
          <NavbarBrand tag={Link} to="/" className='' >
            <img
              alt="logo"
              src="logo192.png"
              className="align-bottom me-2"
              style={{
                height: 32,
                width: 32
              }}
            />
            AlleGutta!</NavbarBrand>

          <NavbarToggler onClick={this.toggleNavbar} className="mr-2" data-toggle="collapse" />
          <Collapse isOpen={!this.state.collapsed} navbar>
            <Nav className="ms-auto" navbar>
              <NavItem className="ms-auto">
                <NavLink onClick={() => this.closeNavbar()} tag={Link} to="/" >Hjem</NavLink>
              </NavItem>
              <NavItem className="ms-auto">
                <NavLink onClick={() => this.closeNavbar()} tag={Link} to="/news" >Nyheter</NavLink>
              </NavItem>
              <NavItem className="ms-auto">
                <NavLink onClick={() => this.closeNavbar()} tag={Link} to="/vedtekter" >Vedtekter</NavLink>
              </NavItem>
              <NavItem className="ms-auto">
                <NavLink onClick={() => this.closeNavbar()} tag={Link} to="/portfolio" >Portefølje</NavLink>
              </NavItem>
            </Nav>
            <NavbarText className="d-flex justify-content-end">
              <button type='button' className="btn btn-link" onClick={() => window.location.reload()} title="Oppdatere side"><i className="bi bi-arrow-clockwise"></i></button>
              <a id="darkmode-button" className="btn btn-outline-secondary">
                <i id="darkmode-moon" className="fa fa-moon-o fa-fw d-none d-light-inline" title="Switch to dark mode"></i>
                <i id="darkmode-sun" className="fa fa-sun-o fa-fw d-none d-dark-inline" title="Switch to light mode"></i>
              </a>
            </NavbarText>
          </Collapse>
        </Navbar>
      </header>
    );
  }
}
