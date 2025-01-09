import React from 'react';
import { IonContent, IonFooter, IonHeader, IonPage, IonTitle, IonToolbar } from '@ionic/react';
import '../styles/GlobalStyles.css';
import Header from '../components/Header';
import Footer from '../components/Footer';



const ContactUs: React.FC = () => (
  <IonPage>
    <IonHeader>
      <Header />
      <IonToolbar>
        <IonTitle>Contact Us</IonTitle>
      </IonToolbar>
    </IonHeader>
    
    <IonContent>
      <div style={{ padding: '16px' }}>
        <h2>Get in Touch</h2>
        <p>If you have any questions, feel free to reach out!</p>
        {/* You can add a contact form here */}
      </div>
    </IonContent>

    <IonFooter>
      <Footer />
    </IonFooter>
  </IonPage>
);

export default ContactUs;