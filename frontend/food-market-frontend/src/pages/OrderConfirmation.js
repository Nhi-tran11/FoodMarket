import React from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import './OrderConfirmation.css';

const OrderConfirmation = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const { orderNumber, total } = location.state || {};

  if (!orderNumber) {
    navigate('/');
    return null;
  }

  return (
    <div className="order-confirmation-container">
      <div className="confirmation-card">
        <div className="success-icon">✓</div>
        <h1>Order Confirmed!</h1>
        <p className="confirmation-message">
          Thank you for your purchase. Your order has been successfully placed.
        </p>
        
        <div className="order-details">
          <div className="detail-row">
            <span className="detail-label">Order Number:</span>
            <span className="detail-value">{orderNumber}</span>
          </div>
          <div className="detail-row">
            <span className="detail-label">Total Amount:</span>
            <span className="detail-value">${total?.toFixed(2)}</span>
          </div>
          <div className="detail-row">
            <span className="detail-label">Estimated Delivery:</span>
            <span className="detail-value">3-5 Business Days</span>
          </div>
        </div>
        
        <p className="email-notification">
          A confirmation email has been sent to your email address.
        </p>
        
        <div className="confirmation-actions">
          <button onClick={() => navigate('/')} className="btn-primary">
            Continue Shopping
          </button>
        </div>
      </div>
    </div>
  );
};

export default OrderConfirmation;
