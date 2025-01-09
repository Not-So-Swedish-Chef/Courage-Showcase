import React from 'react';
import { IonFooter, IonToolbar, IonButtons, IonButton } from '@ionic/react';
import { useHistory } from 'react-router-dom';

const Footer: React.FC = () => {
  const history = useHistory();

  const handleContactClick = () => {
    history.push('/contact-us'); // Navigate to the Contact Us page
  };

  return (
    <IonFooter>
      <IonToolbar>
        <IonButtons slot="end">
          <IonButton onClick={handleContactClick}>
            Contact Us
          </IonButton>
        </IonButtons>
      </IonToolbar>
    </IonFooter>
  );
};

export default Footer;