import React from 'react';
import { IonToolbar } from '@ionic/react';
import { useHistory } from 'react-router-dom';
import logo from '../assets/logo.png';
import Popover from './Popover';
import '../styles/Nav.css';

const Header: React.FC = () => {
  const history = useHistory();

  const handleLogoClick = () => {
    history.push('/');
  };

  return (
    <>
      <IonToolbar className="header-toolbar">
        <img 
          src={logo} 
          alt="Logo" 
          style={{ cursor: 'pointer', height: '40px' }}
          onClick={handleLogoClick} 
        />

        <Popover>
          <Popover.Trigger>
            <button>Show</button>
          </Popover.Trigger>
          <Popover.Content>
            <input type='text' />
            <Popover.Close>
              <button>Close</button>
            </Popover.Close>
          </Popover.Content>
        </Popover>
      </IonToolbar>
    </>
  );
};

export default Header;