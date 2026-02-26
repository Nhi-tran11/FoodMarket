import React from 'react';
import { Link } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import './ProductCard.css';
import { gql, useMutation } from '@apollo/client';
import { LocalState } from '@apollo/client/core/LocalState';

const ProductCard = ({ product, userRole }) => {
  const { addToCart } = useCart();
  
  const handleAddToCart = (e) => {
    e.preventDefault();
    addToCart(product);
    
  };
  const DELETE_PRODUCT = gql`
    mutation DeleteProduct($productId: Int!) {
      deleteProduct(productId: $productId) {
        id
        name
      }
    }
  `;
  const [deleteProduct] = useMutation(DELETE_PRODUCT, {
    onCompleted: () => {
      alert('Product deleted successfully!');
      window.location.reload(); // Refresh the page to show updated product list
    },
    onError: (error) => {
      console.error('Error deleting product:', error);
      alert(`Failed to delete product: ${error.message}`);
    }
  });
  
  const handleDeleteProduct = async (e) => {
    e.preventDefault();
    if (window.confirm(`Are you sure you want to delete "${product.name}"?`)) {
      try {
        await deleteProduct({ variables: { productId: product.id } });
      } catch (error) {
        console.error('Delete product error:', error);
      }
    }
  };

  return (
    <div className="product-card">
      {userRole === 'Admin' && (
        <button 
        onClick={handleDeleteProduct}
        className="admin-badge">Delete</button>
      )}
      <Link to={`/product/${product.id}`} className="product-link">
        <div className="product-image-container">
          <img src={product.image} alt={product.name} className="product-image" />
          {!product.inStock && <div className="out-of-stock-badge">Out of Stock</div>}
        </div>
        <div className="product-info">
          <span className="product-category">{product.category}</span>
          <h3 className="product-name">{product.name}</h3>
          <p className="product-description">{product.description.substring(0, 80)}...</p>
          <div className="product-footer">
            <div className="product-price-container">
              {product.isOnPromotion && product.promotionPrice ? (
                <>
                  <span className="product-price" style={{ textDecoration: 'line-through', fontSize: '0.9rem', color: '#999' }}>
                    ${product.price.toFixed(2)}
                  </span>
                  <span className="product-promotion-price" style={{ color: '#e74c3c', fontWeight: 'bold', fontSize: '1.2rem' }}>
                    ${product.promotionPrice.toFixed(2)}
                  </span>
                  <span className="promotion-badge" style={{ background: '#e74c3c', color: 'white', padding: '2px 8px', borderRadius: '4px', fontSize: '0.75rem', marginLeft: '8px' }}>
                    SALE
                  </span>
                </>
              ) : (
                <span className="product-price">${product.price.toFixed(2)}</span>
              )}
              <span className="product-unit">{product.unit}</span>
            </div>
            <button
              onClick={() => { localStorage.setItem('selectedProduct', JSON.stringify(product)); window.location.href = '/add-promotion'; }}
              className="add-promotion-btn"
            >
              🎁 Add Promotion
            </button>
          </div>
        </div>
      </Link>
      <button 
        onClick={handleAddToCart} 
        className="add-to-cart-btn"
        disabled={!product.inStock}
      >
        {product.inStock ? '🛒 Add to Cart' : 'Out of Stock'}
      </button>
    </div>
  );
};

export default ProductCard;
