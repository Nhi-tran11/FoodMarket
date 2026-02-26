import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { CartProvider } from './context/CartContext';
import Header from './components/Header';
import Footer from './components/Footer';
import Home from './pages/Home';
import ProductDetails from './components/ProductDetails';
import Cart from './components/Cart';
import Checkout from './pages/Checkout';
import OrderConfirmation from './pages/OrderConfirmation';
import './App.css';
import SignIn from './components/SignIn';
import SignUp from './components/SignUp';
import ProductList from './components/ProductList';
import AddProduct from './components/AddProduct';
import AddPromotion from './components/AddPromotion';
import PromotionProductList from './components/PromotionProductList';
import InviteFriend from './pages/InviteFriend';
function App() {
  return (
    <CartProvider>
      <Router>
        <div className="App">
          <Header />
          <main className="main-content">
            <Routes>
              <Route path="/" element={<Home />} />
              <Route path="/product/:id" element={<ProductDetails />} />
              <Route path="/cart" element={<Cart />} />
              <Route path="/checkout" element={<Checkout />} />
              <Route path="/order" element={<OrderConfirmation />} />
              <Route path="/SignIn" element={<SignIn />} />
              <Route path="/SignUp" element={<SignUp />} />
              <Route path="/FoodList" element={<ProductList />} />
              <Route path="/add-product" element={<AddProduct />} />
              <Route path="/add-promotion" element={<AddPromotion />} />
              <Route path="/promotion-products" element={<PromotionProductList />} />
              <Route path="/invite-friend" element={<InviteFriend />} />
            </Routes>
          </main>
          <Footer />
        </div>
      </Router>
    </CartProvider>
  );
}

export default App;
