//import React, { useState } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import { useState } from 'react';


function NavMenu() {
    const [collapsed, setCollapsed] = useState(true);
    function toggleNavbar() {
        setCollapsed(!collapsed);
    }
  return (
      <header>
          <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
              <Container>
                  <NavbarBrand tag={Link} to="/">UEFA Swiss React App</NavbarBrand>
                  <NavbarToggler onClick={toggleNavbar} className="mr-2" />
                  <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!collapsed} navbar>
                      <ul className="navbar-nav flex-grow">
                          <NavItem>
                              <NavLink tag={Link} className="text-dark" to="/">Countries</NavLink>
                          </NavItem>
                          <NavItem>
                              <NavLink tag={Link} className="text-dark" to="/counter">Clubs</NavLink>
                          </NavItem>
                          <NavItem>
                              <NavLink tag={Link} className="text-dark" to="/rank-items">Scenarios</NavLink>
                          </NavItem>
                          <NavItem>
                              <NavLink tag={Link} className="text-dark" to="/rank-items">Simulations</NavLink>
                          </NavItem>
                      </ul>
                  </Collapse>
              </Container>
          </Navbar>
      </header>
  );
}

export default NavMenu;