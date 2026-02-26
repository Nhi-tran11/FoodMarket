import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import './Cart.css';
import { useLazyQuery, useMutation } from '@apollo/client';
import { gql } from '@apollo/client';

const GET_PENDING_ORDERS_QUERY = gql`
  query orderPendingByUserId($userId: Int!) {
    orderPendingByUserId(userId: $userId) {
      id
      status
      discountAmount
      subtotal
      total 
      tax
      orderItems {
        id
        productId
        quantity
        price
        subtotal
        product {
          id
          name
          price
          image
          unit
        }
      }
    }
  }
`;
const DELETE_PENDING_ORDER_MUTATION = gql`
  mutation deletePendingOrder($orderId: Int!) {
    deletePendingOrder(orderId: $orderId)
  }
`;

const Cart = () => {
  const navigate = useNavigate();
  const { cartItems, removeFromCart, updateQuantity, getCartTotal, clearCart } = useCart();
  const user = JSON.parse(localStorage.getItem('user'));
  const [orderCleared, setOrderCleared] = useState(false);
  const [getPendingOrders, { loading, error, data }] = useLazyQuery(GET_PENDING_ORDERS_QUERY, {
    variables: { userId: parseInt(user?.id, 10) },
    fetchPolicy: 'no-cache',
  });

  const [deletePendingOrder] = useMutation(DELETE_PENDING_ORDER_MUTATION, {
    onCompleted: () => {
      setOrderCleared(true);
      clearCart();
    },
    onError: (err) => {
      console.error('Failed to delete pending order:', err);
      alert('Failed to clear cart. Please try again.');
    },
  });


  useEffect(() => {
    if (user?.id) {
      getPendingOrders().catch(() => {});
    }
  }, [user?.id]);
  const displayItems = orderCleared ? [] : (data?.orderPendingByUserId?.orderItems?.map(oi => ({
    ...oi.product,
    quantity: oi.quantity,
    price: oi.price
  })) || cartItems);

  if (loading) {
    return (
      <div className="cart-container">
        <div className="empty-cart"><p>Loading cart...</p></div>
      </div>
    );
  }

  if (error) {
    console.error('Cart query error:', error);
  }

  if (displayItems.length === 0) {
    return (
      <div className="cart-container">
        <div className="empty-cart">
          <h2>Your Cart is Empty</h2>
          <p>Add some delicious items to get started!</p>
          <button onClick={() => navigate('/')} className="btn-primary">
            Continue Shopping
          </button>
        </div>
      </div>
    );
  }

  const pendingOrder = data?.orderPendingByUserId;
  const discountAmount = parseFloat(pendingOrder?.discountAmount) || 0;
  const subtotal = parseFloat(pendingOrder?.subtotal) || getCartTotal();
  const shipping = 5.00;
  const tax = pendingOrder ? parseFloat(pendingOrder.tax) || (subtotal - discountAmount) * 0.1 : (subtotal - discountAmount) * 0.1;
  const total = pendingOrder ? parseFloat(pendingOrder.total) || (subtotal - discountAmount + shipping + tax) : subtotal - discountAmount + shipping + tax;

  return (
    <div className="cart-container">
      <h1 className="cart-title">Shopping Cart</h1>
      
      <div className="cart-content">
        <div className="cart-items">
          {displayItems.map(item => (
            <div key={item.id} className="cart-item">
              <img src={item.image} alt={item.name} className="cart-item-image" />
              
              <div className="cart-item-details">
                <h3 className="cart-item-name">{item.name}</h3>
                <p className="cart-item-unit">{item.unit}</p>
                <p className="cart-item-price">${item.price.toFixed(2)}</p>
              </div>
              
              <div className="cart-item-actions">
                <div className="quantity-controls">
                  <button 
                    onClick={() => updateQuantity(item.id, item.quantity - 1)}
                    className="quantity-btn"
                  >
                    −
                  </button>
                  <span className="quantity">{item.quantity}</span>
                  <button 
                    onClick={() => updateQuantity(item.id, item.quantity + 1)}
                    className="quantity-btn"
                  >
                    +
                  </button>
                </div>
                
                <div className="cart-item-total">
                  ${(item.price * item.quantity).toFixed(2)}
                </div>
                
                <button 
                  onClick={() => removeFromCart(item.id)}
                  className="remove-btn"
                  title="Remove from cart"
                >
                  🗑️
                </button>
              </div>
            </div>
          ))}
        </div>
        
        <div className="cart-summary">
          <h2>Order Summary</h2>
          
          <div className="summary-row">
            <span>Subtotal</span>
            <span>${subtotal.toFixed(2)}</span>
          </div>

          {discountAmount > 0 && (
            <div className="summary-row" style={{ color: '#27ae60' }}>
              <span>🎟️ Discount</span>
              <span>-${discountAmount.toFixed(2)}</span>
            </div>
          )}
          
          <div className="summary-row">
            <span>Shipping</span>
            <span>${shipping.toFixed(2)}</span>
          </div>
          
          <div className="summary-row">
            <span>Tax (10%)</span>
            <span>${tax.toFixed(2)}</span>
          </div>
          
          <div className="summary-divider"></div>
          
          <div className="summary-row total">
            <span>Total</span>
            <span>${total.toFixed(2)}</span>
          </div>
          
          <button 
            onClick={() => navigate('/checkout')}
            className="checkout-btn"
          >
            Proceed to Checkout
          </button>
          
          <button 
            onClick={() => navigate('/')}
            className="continue-shopping-btn"
          >
            Continue Shopping
          </button>
          
          <button 
            onClick={() => {
              if (pendingOrder?.id) {
                deletePendingOrder({ variables: { orderId: pendingOrder.id } });
              } else {
                clearCart();
              }
            }}
            className="clear-cart-btn"
          >
            Clear Cart
          </button>
        </div>
      </div>
    </div>
  );
};

export default Cart;
