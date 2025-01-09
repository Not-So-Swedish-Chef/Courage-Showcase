import { IonContent, IonFooter, IonHeader, IonPage, IonTitle, IonToolbar } from '@ionic/react';
import '../styles/GlobalStyles.css';
import Header from '../components/Header';
import Footer from '../components/Footer';

const Home: React.FC = () => {
  return (
    <IonPage>
      <IonHeader>
        <Header />
      </IonHeader>
      
      <IonContent fullscreen>
        <div className="content">
          <h1>
            Welcome to the Courage Culture Club!
          </h1>
          
          <p>
                A Culture of Courage makes people feel safe to take risks and provides a compelling reason to do so.
          </p>
          <p>
                Aristotle said that Courage is the first of all virtues because it is the only one that guarantees all others. The Merriam-Webster dictionary defines courage as: mental or moral strength to venture, persevere, and withstand danger, fear, or difficulty. Those born with, or who have acquired, a disability or special need, are usually automatically set apart from the majority of people in their community because many of us hold an acquired opinion that it is very probable that these people will always be dependent and incapable of being contributing members of society. In fact, for many of us, those with dis-abilities are invisible to us as we aren’t comfortable being around them and we fail to recognize their abilities.
          </p>
          <p>
            Many of us fail to recognize these abilities because we have not had the opportunity to connect with a community of very special people. I know I didn't until I was faced with the reality of raising a child with complex needs. I had no comprehension how difficult it is for them to fit into our world - snow and ice on sidewalks, lack of ramps and elevators in and around buildings and subways, recreation and community programming that expects those with disabilities and special needs to be able to access with no allowances or equipment designed specifically for them ....
          </p>
          <p>
            As a mom of a young adult with significant special needs who does require significant care, I, too, have often underestimated exactly what my daughter is capable of doing. For those who take the time to participate in her life and the lives of others like her, these stereotypes are broken regularly when we believe in the potential these special people have and they are afforded opportunities that are designed for them to participate easily. Our culture and community, for the most part, leave so many youth and adults with special needs hidden away, especially after they finish their schooling. There is a lack of day program spaces, employment, housing, funding, and community involvement opportunities for this sector of Canada’s population.
          </p>
          <p>
            Researchers have found that we are wired to defend what we know, to resist information that contradicts it and to seek out like-minds and evidence that complies with our beliefs. What if those with disabilities and special needs forged new ground and showed their communities just how adept they can be? Scary for them – and for those communities? Undoubtably! Courageous for all? Absolutely!
          </p>
          <p>
            As Dr. Margie Warrell wrote, "it is easy to focus on what is directly ahead; to live from a place of probability. It takes courage to shift your sights beyond the immediate horizon and to dream about what could be possible now, 5, 10 or 25 years from now."<sup>1</sup> Unless we can move beyond probabilities to possibilities, nothing will change. Cultivating a “courage culture” requires leaders and all community members to park their thoughts, ideas, and egos and be proactive in seeking information and beliefs that differ from their own.
          </p>
          <p>
            Courage is a habit, and like all habits it can be learned. The ‘mere exposure’ effect means that the more often someone is exposed to a situation that makes him or her uncomfortable, the less scary it will become. Encouraging people to exit their comfort zone grows their confidence to step further outside it more often. So too will rewarding courageous behavior; doing so makes a strong implicit declaration that you see smart risks as vital to long-term success, even when they may not pay off in the short run.
          </p>
          <p>
            Creating a culture of courage starts with you and I. Sticking with the status quo is not an option as it will grow increasingly expensive.
          </p>
          <p>
            It's time to change the world!
          </p>
          <p>
            - Lisa xo
          </p>
          <p>
            <sup>1</sup> https://www.forbes.com/sites/margiewarrell/2014/08/31/building-brave-people/?sh=4d3e1ecdb484
          </p>
        </div>
      </IonContent>

      <IonFooter>
        <Footer />
      </IonFooter>
    </IonPage>
  );
};

export default Home;
