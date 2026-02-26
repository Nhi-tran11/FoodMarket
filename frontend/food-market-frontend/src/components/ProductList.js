import React, { use, useState } from 'react';
import ProductCard from './ProductCard';
import { products, categories } from '../data/products';
import './ProductList.css';
import { gql,useLazyQuery } from '@apollo/client';
import { useEffect } from 'react';
const ProductList = () => {
  const [selectedCategory, setSelectedCategory] = useState('All');
  const [searchTerm, setSearchTerm] = useState('');
    const [userRole, setUserRole] = useState(null);
  const [user, setUser] = useState(() => {
    const savedUser = localStorage.getItem('user');
    return savedUser ? JSON.parse(savedUser) : null;
  });
  console.log('Current user:', user);
      const CHECK_AUTHENTICATION = gql`
          query CustomerByEmailAsync($email: String!) {
              customerByEmail(email: $email) {
                  role
              }
          }`;
     const [checkAuthCustomer, { data }] = useLazyQuery(CHECK_AUTHENTICATION, {
    onCompleted: (data) => {
      setUserRole(data?.customerByEmail?.role);
    }
  });
     // Fetch user role when component mounts
  useEffect(() => {
    if (user?.email) {
      checkAuthCustomer({ variables: { email: user.email } });
    }
  }, [user?.email, checkAuthCustomer]);

  //Fetch products from backend (to be implemented)
  const fetchProducts = gql`
    query{
      allProducts{
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
        promotionStartDate
        promotionEndDate
        isOnPromotion
        currentPrice
      }
    }`
  const [getProducts, {loading, error, data:productData}] = useLazyQuery(fetchProducts);
  useEffect(() => {
    getProducts();
  }, [getProducts]);

  if (loading) return <p>Loading products...</p>;
  if (error) return <p>Error loading products: {error.message}</p>;

    const filteredProducts = (productData?.allProducts||[]).filter(product => {
    const matchesCategory = selectedCategory === 'All' || product.category === selectedCategory;
    const matchesSearch = product.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         product.description.toLowerCase().includes(searchTerm.toLowerCase());
    return matchesCategory && matchesSearch;
  });
  const categories = ['All', ...new Set(filteredProducts.map(product => product.category))];
  console.log('Fetched products from backend:', productData);
  return (
    <div className="product-list-container">
      <div className="filters-section">
        <div>
          {userRole === 'Admin' ? (
            <>
              <h2>Welcome Admin </h2>
              <button className="add-product-btn" onClick={() => window.location.href = '/add-product'}>
                + Add New Product
              </button>             
            </>
          ) : null}
        </div>
        <div className="search-bar">
          <input
            type="text"
            placeholder="Search products..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="search-input"
          />
        </div>
        <div className="category-filters">
          {categories.map(category => (
            <button
              key={category}
              onClick={() => setSelectedCategory(category)}
              className={`category-btn ${selectedCategory === category ? 'active' : ''}`}
            >
              {category}
            </button>
          ))}
        </div>
      </div>
      
      <div className="products-grid">
        {filteredProducts.length > 0 ? (
          filteredProducts.map(product => (
            <ProductCard key={product.id} product={product} userRole={userRole} />
          ))
        ) : (
          <div className="no-products">
            <p>No products found matching your criteria.</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default ProductList;
