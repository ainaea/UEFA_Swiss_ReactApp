import { Container } from 'reactstrap';
import NavMenu from './NavMenu';
import { Routes } from 'react-router';


function Layout({ children }) {
    return (
      <>
          <NavMenu />
          <Container>
              <Routes>
                  {children.map(c => c)}
              </Routes>
          </Container>
      </>
  );
}

export default Layout;