import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { gql, useQuery } from '@apollo/client';
import { useCart } from '../context/CartContext';
import './ProductDetails.css';

const GET_PRODUCT = gql`
  query GetProduct($id: Int!) {
    productById(id: $id) {
      id
      name
      description
      price
      category
      image
      inStock
      stockQuantity
      unit
      promotionPrice
      isOnPromotion
    }
  }
`;

const ProductDetails = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { addToCart } = useCart();
  const [quantity, setQuantity] = useState(1);
  
  const { loading, error, data } = useQuery(GET_PRODUCT, {
    variables: { id: parseInt(id) }
  });
  
  if (loading) return <div className="product-details-container"><p>Loading product...</p></div>;
  if (error) return <div className="product-details-container"><p>Error loading product: {error.message}</p></div>;
  
  const product = data?.productById;

  if (!product) {
    return (
      <div className="product-details-container">
        <div className="product-not-found">
          <h2>Product not found</h2>
          <button onClick={() => navigate('/')} className="btn-primary">
            Back to Products
          </button>
        </div>
      </div>
    );
  }

  const handleAddToCart = () => {
    for (let i = 0; i < quantity; i++) {
      addToCart(product);
    }
    navigate('/cart');
  };

  const handleQuantityChange = (change) => {
    const newQuantity = quantity + change;
    if (newQuantity >= 1 && newQuantity <= 99) {
      setQuantity(newQuantity);
    }
  };

  return (
    <div className="product-details-container">
      <button onClick={() => navigate('/')} className="back-btn">
        ← Back to Products
      </button>
      
      <div className="product-details-content">
        <div className="product-details-image">
          <img src={product.image} alt={product.name} />
        </div>
        
        <div className="product-details-info">
          <span className="product-details-category">{product.category}</span>
          <h1 className="product-details-name">{product.name}</h1>
          <div className="product-details-price">
            <span className="price">${product.isOnPromotion && product.promotionPrice ? product.promotionPrice.toFixed(2) : product.price.toFixed(2)}</span>
            {product.isOnPromotion && product.promotionPrice && (
              <span className="product-details-price">${product.promotionPrice.toFixed(2)}</span>
            )}
            <span className="unit">{product.unit}</span>
          </div>
          
          <div className={`stock-status ${product.inStock ? 'in-stock' : 'out-of-stock'}`}>
            {product.inStock ? '✓ In Stock' : '✗ Out of Stock'}
          </div>
          
          <p className="product-details-description">{product.description}</p>
          
          <div className="product-actions">
            <div className="quantity-selector">
              <button onClick={() => handleQuantityChange(-1)} className="quantity-btn">−</button>
              <input 
                type="number" 
                value={quantity} 
                onChange={(e) => setQuantity(Math.max(1, Math.min(99, parseInt(e.target.value) || 1)))}
                className="quantity-input"
                min="1"
                max="99"
              />
              <button onClick={() => handleQuantityChange(1)} className="quantity-btn">+</button>
            </div>
            
            <button 
              onClick={handleAddToCart}
              disabled={!product.inStock}
              className="add-to-cart-btn-large"
            >
              {product.inStock ? '🛒 Add to Cart' : 'Out of Stock'}
            </button>
          </div>
          
          <div className="total-price">
            {product.isOnPromotion && product.promotionPrice ? (
              <span className="total-price">${(product.promotionPrice * quantity).toFixed(2)}</span>
            ) : (
              <span className="total-price">${(product.price * quantity).toFixed(2)}</span>
            )}
            Total: ${(product.isOnPromotion && product.promotionPrice ? product.promotionPrice * quantity : product.price * quantity).toFixed(2)}
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProductDetails;
