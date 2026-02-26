import React, { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import ProductList from '../components/ProductList';
import './Home.css';
import { gql, useQuery } from '@apollo/client';

const PROMOTION_PRODUCTS = gql`
  query {
    promotionProducts {
      id
      name
      description
      image
      price
      promotionPrice
      promotionStartDate
      promotionEndDate
    }
  }
`;

const Home = () => {
  const navigate = useNavigate();
  const [currentSlide, setCurrentSlide] = useState(0);
  const [friendName, setFriendName] = useState('');
  const [friendEmail, setFriendEmail] = useState('');
  const [inviteSuccessMessage, setInviteSuccessMessage] = useState('');
  const inviteSectionRef = useRef(null);
  const { loading, error, data } = useQuery(PROMOTION_PRODUCTS);

  // Default slides as fallback
  const defaultSlides = [
    {
      title: "PROMOTION",
      subtitle: "Weekly Specials",
      description: "Fresh, healthy, and sustainable meals delivered to your door.",
      badge: "SALE",
      image: "https://images.unsplash.com/photo-1550547660-d9450f859349?w=600&h=600&fit=crop",
      image2: "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=600&h=600&fit=crop"
    },
    {
      title: "GET DISCOUNT BY INVITING FRIENDS",
      subtitle: "Share the love of good food",
      description: "Invite your friends and get exclusive discounts on your next order once they make their first purchase.",
      badge: "REFERRAL",
      image: "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=600&h=600&fit=crop",
      image2: "https://images.unsplash.com/photo-1490645935967-10de6ba17061?w=600&h=600&fit=crop"
    }
  ];

  // Convert promotion products to slides format
  const promotionSlides = data?.promotionProducts?.map(product => {
    const discountPercent = product.promotionPrice && product.price
      ? Math.round(((product.price - product.promotionPrice) / product.price) * 100)
      : 0;
    
    return {
      id: product.id,
      title: "ON SALE NOW",
      subtitle: `Start from ${product.promotionStartDate ? new Date(product.promotionStartDate).toLocaleDateString() : "Limited Time Offer"}${product.promotionEndDate ? ` - Valid until ${new Date(product.promotionEndDate).toLocaleDateString()}` : ""}`,
      description: product.description,
      badge: discountPercent > 0 ? `${discountPercent}% OFF` : "PROMOTION",
      image: product.image,
      image2: product.image
    };
  }) || [];

  // Always keep referral slide visible
  const referralSlide = defaultSlides[1];

  // Use promotion slides when available and append referral slide
  const slides = promotionSlides.length > 0 ? [...promotionSlides, referralSlide] : defaultSlides;

  useEffect(() => {
    const timer = setInterval(() => {
      setCurrentSlide((prev) => (prev + 1) % slides.length);
    }, 5000);
    return () => clearInterval(timer);
  }, [slides.length]);

  const goToSlide = (index) => {
    setCurrentSlide(index);
  };

  const nextSlide = () => {
    setCurrentSlide((prev) => (prev + 1) % slides.length);
  };

  const prevSlide = () => {
    setCurrentSlide((prev) => (prev - 1 + slides.length) % slides.length);
  };

  const scrollToInviteSection = () => {
    inviteSectionRef.current?.scrollIntoView({ behavior: 'smooth', block: 'start' });
  };

  const handleInviteSubmit = (event) => {
    event.preventDefault();
    setInviteSuccessMessage(`Invitation sent to ${friendName.trim()} (${friendEmail.trim()})`);
    setFriendName('');
    setFriendEmail('');
  };

  if (loading) return <div className="home"><p>Loading promotions...</p></div>;
  if (error) console.error('Error loading promotions:', error);

  return (
    <div className="home">
      <section className="hero-slider">
        <div className="slider-container">
          {slides.map((slide, index) => (
            <div
              key={index}
              className={`slide ${index === currentSlide ? 'active' : ''}`}
            >
              <div className="slide-content">
                <div className="slide-left">
                  <div className="slide-image-circle">
                    <img src={slide.image} alt="Food" />
                  </div>
                </div>

                <div className="slide-center">
                  
                  <h1 className="slide-title">{slide.title}</h1>
                  <h2 className="slide-subtitle">{slide.subtitle}</h2>
                  <p className="slide-description">{slide.description}</p>
                  
                  <div className="slide-actions">
                    <button 
                      className="order-btn" 
                      onClick={() => {
                        if (slide.id) {
                          navigate(`/product/${slide.id}`);
                          return;
                        }

                        if (slide.badge === 'REFERRAL') {
                          navigate('/invite-friend');
                          return;
                        }

                        navigate('/FoodList');
                      }}
                    >
                      {slide.id ? 'VIEW PRODUCT' : slide.badge === 'REFERRAL' ? 'INVITE FRIEND' : 'ORDER NOW'}
                    </button>
                    <div className="slide-arrows">
                      <button className="arrow-btn" onClick={prevSlide}>‹</button>
                      <button className="arrow-btn" onClick={nextSlide}>›</button>
                    </div>
                  </div>
                </div>

                <div className="slide-right">
                  <div className="discount-badge">
                    <span className="discount-text">{slide.badge}</span>
                  </div>
                  <div className="slide-image-dish">
                    <img src={slide.image2} alt="Dish" />
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>

        <div className="slider-dots">
          {slides.map((_, index) => (
            <button
              key={index}
              className={`dot ${index === currentSlide ? 'active' : ''}`}
              onClick={() => goToSlide(index)}
              aria-label={`Go to slide ${index + 1}`}
            />
          ))}
        </div>
      </section>
      
      <section className="products-section">
        <ProductList />
      </section>
    </div>
  );
};

export default Home;