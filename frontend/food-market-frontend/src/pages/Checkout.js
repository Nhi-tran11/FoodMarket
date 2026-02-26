import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import './Checkout.css';
import { gql, useQuery, useMutation, useLazyQuery } from '@apollo/client';
import { loadStripe } from '@stripe/stripe-js';
import { Elements, CardElement, useStripe, useElements } from '@stripe/react-stripe-js';

const toCurrencyAmount = (value) => Number(value.toFixed(2));


const COUNTRY_NAME_TO_CODE = {
  'new zealand': 'NZ',
  'australia': 'AU',
  'united states': 'US',
  'usa': 'US',
  'united kingdom': 'GB',
  'uk': 'GB',
  'great britain': 'GB',
  'canada': 'CA'
};

const toIsoCountryCode = (countryValue) => {
  const trimmedValue = countryValue?.trim();
  if (!trimmedValue) return null;

  const upperValue = trimmedValue.toUpperCase();
  if (/^[A-Z]{2}$/.test(upperValue)) {
    return upperValue;
  }

  return COUNTRY_NAME_TO_CODE[trimmedValue.toLowerCase()] || null;
};

const SHIPPING_DETAILS = gql`
  query GetShippingDetailByDefault($customerId: Int!) {
    shippingDetailByDefault(customerId: $customerId) {
      id
      fullName
      phoneNumber
      address
      city
      state
      zipCode
      country
      isDefault
    }
  }
`;

const CREATE_SHIPPING_DETAIL = gql`
  mutation CreateShippingDetail($customerId: Int!, $fullName: String!, $phoneNumber: String!, $address: String!, $city: String!, $state: String!, $zipCode: String!, $country: String!, $isDefault: Boolean!) {
    createShippingDetail(customerId: $customerId, fullName: $fullName, phoneNumber: $phoneNumber, address: $address, city: $city, state: $state, zipCode: $zipCode, country: $country, isDefault: $isDefault) {
      id
      fullName
      phoneNumber
      address
      city
      state
      zipCode
      country
      isDefault
    }
  }
`;

const CREATE_PAYMENT_INTENT = gql`
  mutation CreatePaymentIntent($amount: Decimal!, $currency: String!, $orderId: Int!) {
    createPaymentIntent(amount: $amount, currency: $currency, orderId: $orderId) {
      clientSecret
    }
  }
`;

const CREATE_ORDER = gql`
  mutation CreateOrder(
    $customerId: Int!
    $shippingDetailId: Int!
    $items: [OrderItemInputTypeInput!]!
    $subtotal: Decimal!
    $shipping: Decimal!
    $discountCode: String
  ) {
    createOrder(
      customerId: $customerId
      shippingDetailId: $shippingDetailId
      items: $items
      subtotal: $subtotal
      shipping: $shipping
      discountCode: $discountCode
    ) {
      id
      total
      status
    }
  }
`;
const GET_DISCOUTNTS = gql`
  query referalCodeByCode($code: String!
  $userId: Int!) {
    referalCodeByCode(code: $code, userId: $userId) {
      discountPercentage
    }
  }
`;

const GET_STRIPE_KEY = gql`
  query GetStripePublishableKey {
    stripePublishableKey
  }
`;
const UPDATE_ORDER_STATUS = gql`
    mutation UpdateOrderStatus($orderId: Int!, $status: String!) {
        updateOrderStatus(orderId: $orderId, status: $status) {
            id
            status
        }
    }
`;

const CheckoutForm = () => {
  const stripe = useStripe();
  const elements = useElements();
  const navigate = useNavigate();
  const { cartItems, getCartTotal, clearCart } = useCart();
  const [processingPayment, setProcessingPayment] = useState(false);
  const [user] = useState(() => {
    const savedUser = localStorage.getItem('user');
    return savedUser ? JSON.parse(savedUser) : null;
  });
  const [discountCode, setDiscountCode] = useState('');
  const [discountError, setDiscountError] = useState('');
  const [appliedDiscount, setAppliedDiscount] = useState(null); // { percentage, amount }

  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: user?.email || '',
    phone: '',
    address: '',
    city: '',
    state: '',
    zipCode: '',
    country: '',
    isDefaultAddress: false
  });

  useQuery(SHIPPING_DETAILS, {
    variables: { customerId: parseInt(user?.id, 10) },
    skip: !user?.id,
    onCompleted: (data) => {
      if (data?.shippingDetailByDefault) {
        const detail = data.shippingDetailByDefault;
        setFormData(prev => ({
          ...prev,
          firstName: detail.fullName.split(' ')[0] || '',
          lastName: detail.fullName.split(' ')[1] || '',
          phone: detail.phoneNumber,
          address: detail.address,
          city: detail.city,
          state: detail.state,
          zipCode: detail.zipCode,
          country: detail.country,
        }));
      }
    }
  });

  const [createShippingDetail] = useMutation(CREATE_SHIPPING_DETAIL);
  const [createPaymentIntent] = useMutation(CREATE_PAYMENT_INTENT);
  const [createOrder] = useMutation(CREATE_ORDER);
  const [updateOrderStatus] = useMutation(UPDATE_ORDER_STATUS);
  const [fetchDiscount, { loading: discountLoading }] = useLazyQuery(GET_DISCOUTNTS);
  useEffect(() => {
    if (user?.email && !formData.email) {
      setFormData(prev => ({
        ...prev,
        email: user.email
      }));
    }
  }, [user?.email, formData.email]);

  const [errors, setErrors] = useState({});

  const handleChange = (e) => {
    const { name, value } = e.target;

    if (name === 'email' && user?.email) {
      return;
    }

    setFormData(prev => ({
      ...prev,
      [name]: value
    }));

    if (errors[name]) {
      setErrors(prev => ({
        ...prev,
        [name]: ''
      }));
    }
  };

  const validateForm = () => {
    const newErrors = {};

    if (!formData.firstName.trim()) newErrors.firstName = 'First name is required';
    if (!formData.lastName.trim()) newErrors.lastName = 'Last name is required';

    if (!formData.email.trim()) {
      newErrors.email = 'Email is required';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'Email is invalid';
    } else if (user?.email && formData.email !== user.email) {
      newErrors.email = 'Email must match your account email';
    }

    if (!formData.phone.trim()) newErrors.phone = 'Phone is required';
    if (!formData.address.trim()) newErrors.address = 'Address is required';
    if (!formData.city.trim()) newErrors.city = 'City is required';
    if (!formData.state.trim()) newErrors.state = 'State is required';
    if (!formData.zipCode.trim()) newErrors.zipCode = 'ZIP code is required';
    if (!formData.country.trim()) newErrors.country = 'Country is required';

    return newErrors;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    const newErrors = validateForm();
    if (Object.keys(newErrors).length > 0) {
      setErrors(newErrors);
      return;
    }

    if (!stripe || !elements) {
      alert('Payment system is still loading. Please wait a moment.');
      return;
    }

    setProcessingPayment(true);

    try {
      const { data: shippingData } = await createShippingDetail({
        variables: {
          customerId: parseInt(user?.id, 10),
          fullName: `${formData.firstName} ${formData.lastName}`,
          phoneNumber: formData.phone,
          address: formData.address,
          city: formData.city,
          state: formData.state,
          zipCode: formData.zipCode,
          country: formData.country,
          isDefault: formData.isDefaultAddress
        }
      });

      const shippingDetailId = shippingData.createShippingDetail.id;

      // Use component-level totals which already account for discount
      const orderItems = cartItems.map(item => ({
        productId: item.id,
        quantity: item.quantity,
        price: item.price
      }));

      const { data: orderData } = await createOrder({
        variables: {
          customerId: parseInt(user?.id, 10),
          shippingDetailId,
          items: orderItems,
          subtotal,
          shipping,
          discountCode: discountCode.trim() || null
        }
      });

      const orderId = orderData.createOrder.id;

      const { data: paymentData } = await createPaymentIntent({
        variables: {
          amount: total,
          currency: 'nzd',
          orderId
        }
      });

      const clientSecret = paymentData.createPaymentIntent.clientSecret;
      const cardElement = elements.getElement(CardElement);
      const stripeCountryCode = toIsoCountryCode(formData.country);

      if (!cardElement) {
        throw new Error('Card details are not ready. Please check card input and try again.');
      }

      if (!stripeCountryCode) {
        throw new Error('Please use a 2-letter country code for payment (for example: NZ, US, GB).');
      }

      const { error: stripeError, paymentIntent } = await stripe.confirmCardPayment(
        clientSecret,
        {
          payment_method: {
            card: cardElement,
            billing_details: {
              name: `${formData.firstName} ${formData.lastName}`,
              email: formData.email,
              address: {
                line1: formData.address,
                city: formData.city,
                state: formData.state,
                postal_code: formData.zipCode,
                country: stripeCountryCode
              }
            }
          }
        }
      );

      if (stripeError) {
        throw new Error(stripeError.message);
      }

      if (paymentIntent.status === 'succeeded') {

            await updateOrderStatus({
                variables: {
                    orderId,
                    status: 'PAID'
                }
            });
        clearCart();
        navigate('/order', {
          state: {
            orderNumber: paymentIntent.id.substring(3, 10).toUpperCase(),
            total,
            paymentIntentId: paymentIntent.id
          }
        });
      }
    } catch (error) {
      console.error('Payment error:', error);
      const backendMessage = error.graphQLErrors?.[0]?.message || error.networkError?.result?.errors?.[0]?.message;
      alert(backendMessage || error.message || 'Payment failed. Please try again.');
      setProcessingPayment(false);
    }
  };

  if (cartItems.length === 0) {
    return (
      <div className="checkout-container">
        <div className="empty-checkout">
          <h2>Your cart is empty</h2>
          <button onClick={() => navigate('/')} className="btn-primary">
            Go Shopping
          </button>
        </div>
      </div>
    );
  }

  const subtotal = toCurrencyAmount(getCartTotal());
  const shipping = toCurrencyAmount(5.00);
  const discountAmount = appliedDiscount
    ? toCurrencyAmount(subtotal * (appliedDiscount.percentage / 100))
    : 0;
  const discountedSubtotal = toCurrencyAmount(subtotal - discountAmount);
  const tax = toCurrencyAmount(discountedSubtotal * 0.1);
  const total = toCurrencyAmount(discountedSubtotal + shipping + tax);

  const applyDiscountCode = async () => {
    if (!discountCode.trim()) {
      setDiscountError('Please enter a discount code');
      return;
    }
    try {
      const { data, error } = await fetchDiscount({
        variables: {
          code: discountCode.trim(),
          userId: parseInt(user?.id, 10) 
        }
      });
      if (error || !data?.referalCodeByCode) {
        setAppliedDiscount(null);
        setDiscountError('Invalid or expired discount code');
        return;
      }
      const percentage = data.referalCodeByCode.discountPercentage ?? 0;
      setAppliedDiscount({ percentage });
      setDiscountError('');
    } catch (err) {
      setAppliedDiscount(null);
      setDiscountError(err.graphQLErrors?.[0]?.message || 'Invalid or expired discount code');
    }
  };

  return (
    <div className="checkout-container">
      <h1 className="checkout-title">Checkout</h1>

      <div className="checkout-content">
        <form onSubmit={handleSubmit} className="checkout-form" noValidate>
          <section className="form-section">
            <h2>Shipping Information</h2>

            <div className="form-row">
              <div className="form-group">
                <label>First Name *</label>
                <input
                  type="text"
                  name="firstName"
                  value={formData.firstName}
                  onChange={handleChange}
                  className={errors.firstName ? 'error' : ''}
                />
                {errors.firstName && <span className="error-message">{errors.firstName}</span>}
              </div>

              <div className="form-group">
                <label>Last Name *</label>
                <input
                  type="text"
                  name="lastName"
                  value={formData.lastName}
                  onChange={handleChange}
                  className={errors.lastName ? 'error' : ''}
                />
                {errors.lastName && <span className="error-message">{errors.lastName}</span>}
              </div>
            </div>

            <div className="form-row">
              <div className="form-group">
                <label>Email *</label>
                <input
                  type="email"
                  name="email"
                  value={formData.email}
                  onChange={handleChange}
                  className={errors.email ? 'error' : ''}
                  readOnly={!!user?.email}
                  style={user?.email ? { backgroundColor: '#f5f5f5', cursor: 'not-allowed' } : {}}
                />
                {user?.email && (
                  <span className="info-message" style={{ color: '#27ae60', fontSize: '0.85rem' }}>
                    ✓ Email from your account
                  </span>
                )}
                {errors.email && <span className="error-message">{errors.email}</span>}
              </div>

              <div className="form-group">
                <label>Phone *</label>
                <input
                  type="tel"
                  name="phone"
                  value={formData.phone}
                  onChange={handleChange}
                  className={errors.phone ? 'error' : ''}
                />
                {errors.phone && <span className="error-message">{errors.phone}</span>}
              </div>
            </div>

            <div className="form-group">
              <label>Address *</label>
              <input
                type="text"
                name="address"
                value={formData.address}
                onChange={handleChange}
                className={errors.address ? 'error' : ''}
              />
              {errors.address && <span className="error-message">{errors.address}</span>}
            </div>

            <div className="form-row">
              <div className="form-group">
                <label>City *</label>
                <input
                  type="text"
                  name="city"
                  value={formData.city}
                  onChange={handleChange}
                  className={errors.city ? 'error' : ''}
                />
                {errors.city && <span className="error-message">{errors.city}</span>}
              </div>

              <div className="form-group">
                <label>State *</label>
                <input
                  type="text"
                  name="state"
                  value={formData.state}
                  onChange={handleChange}
                  className={errors.state ? 'error' : ''}
                />
                {errors.state && <span className="error-message">{errors.state}</span>}
              </div>

              <div className="form-group">
                <label>ZIP Code *</label>
                <input
                  type="text"
                  name="zipCode"
                  value={formData.zipCode}
                  onChange={handleChange}
                  className={errors.zipCode ? 'error' : ''}
                />
                {errors.zipCode && <span className="error-message">{errors.zipCode}</span>}
              </div>

              <div className="form-group">
                <label>Country *</label>
                <input
                  type="text"
                  name="country"
                  value={formData.country}
                  onChange={handleChange}
                  className={errors.country ? 'error' : ''}
                />
                {errors.country && <span className="error-message">{errors.country}</span>}
              </div>
            </div>

            <div className="form-group checkbox-group" style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
              <input
                type="checkbox"
                id="isDefaultAddress"
                name="isDefaultAddress"
                checked={formData.isDefaultAddress}
                onChange={(e) => setFormData((prev) => ({ ...prev, isDefaultAddress: e.target.checked }))}
                style={{ width: 'auto', cursor: 'pointer' }}
              />
              <label htmlFor="isDefaultAddress" style={{ margin: 0, cursor: 'pointer', fontWeight: 'normal' }}>
                Save this as my default shipping address
              </label>
            </div>
          </section>

          <section className="form-section">
            <h2>Payment Information</h2>

            <div className="form-group">
              <label>Card Details *</label>
              <div className="card-element-container">
                <CardElement
                  options={{
                    style: {
                      base: {
                        fontSize: '16px',
                        color: '#424770',
                        '::placeholder': {
                          color: '#aab7c4',
                        }
                      },
                      invalid: {
                        color: '#e74c3c',
                      },
                    },
                  }}
                />
              </div>
              <p className="card-info" style={{ fontSize: '0.85rem', color: '#666', marginTop: '0.5rem' }}>
                💳 Test card: 4242 4242 4242 4242 | Any future date | Any CVC
              </p>
            </div>
          </section>

          <button type="submit" className="place-order-btn" disabled={!stripe || processingPayment}>
            {processingPayment ? 'Processing Payment...' : `Place Order - $${total.toFixed(2)}`}
          </button>
        </form>

        <div className="order-summary-checkout">
          <h2>Order Summary</h2>

          <div className="order-items">
            {cartItems.map((item) => (
              <div key={item.id} className="order-item">
                <img src={item.image} alt={item.name} />
                <div className="order-item-info">
                  <p className="order-item-name">{item.name}</p>
                  <p className="order-item-quantity">Qty: {item.quantity}</p>
                </div>
                <p className="order-item-price">${(item.price * item.quantity).toFixed(2)}</p>
              </div>
            ))}
          </div>

          <div className="order-totals">
            <div className="total-row">
              <span>Subtotal</span>
              <span>${subtotal.toFixed(2)}</span>
            </div>
            <div className="total-row">
              <span>Shipping</span>
              <span>${shipping.toFixed(2)}</span>
            </div>
            <div className="total-row">
              <span>Tax</span>
              <span>${tax.toFixed(2)}</span>
            </div>

            <div className="discount-section" style={{ marginTop: '1rem', paddingTop: '1rem', borderTop: '1px solid #ddd' }}>
              <label htmlFor="discountCode">🎟️ Discount Code</label>
              <div style={{ display: 'flex', gap: '0.5rem', marginTop: '0.5rem' }}>
                <input
                  type="text"
                  id="discountCode"
                  value={discountCode}
                  onChange={(e) => {
                    setDiscountCode(e.target.value);
                    if (discountError) setDiscountError('');
                  }}
                  placeholder="Enter discount code"
                  style={{ flex: 1, padding: '0.5rem' }}
                />
                <button
                  type="button"
                  onClick={applyDiscountCode}
                  className="btn-apply-discount"
                  style={{ padding: '0.5rem 1rem', backgroundColor: '#3498db', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer' }}
                >
                  Apply{discountLoading ? '...' : ''}
                </button>
              </div>
              {discountError && <span className="error-message">{discountError}</span>}
              {appliedDiscount && (
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginTop: '0.5rem', color: '#27ae60', fontWeight: '500' }}>
                  <span>✅ {appliedDiscount.percentage}% discount applied!</span>
                  <span>-${discountAmount.toFixed(2)}</span>
                  <button
                    type="button"
                    onClick={() => { setAppliedDiscount(null); setDiscountCode(''); }}
                    style={{ background: 'none', border: 'none', color: '#e74c3c', cursor: 'pointer', fontSize: '0.85rem' }}
                  >
                    Remove
                  </button>
                </div>
              )}
            </div>

            <div className="total-divider"></div>
            <div className="total-row final">
              <span>Total</span>
              <span>${total.toFixed(2)}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

const Checkout = () => {
  const [stripePromise, setStripePromise] = useState(null);
  const { data: stripeKeyData, loading, error } = useQuery(GET_STRIPE_KEY);

  useEffect(() => {
    if (stripeKeyData?.stripePublishableKey) {
      setStripePromise(loadStripe(stripeKeyData.stripePublishableKey));
    }
  }, [stripeKeyData]);

  if (error) {
    return (
      <div className="checkout-container">
        <div className="order-processing">
          <h2>Error Loading Payment System</h2>
          <p style={{ color: 'red' }}>{error.message}</p>
          <p>Please check if backend is running on http://localhost:5000</p>
        </div>
      </div>
    );
  }

  if (loading || !stripePromise) {
    return (
      <div className="checkout-container">
        <div className="order-processing">
          <div className="spinner"></div>
          <h2>Loading Payment System...</h2>
          <p>Fetching Stripe configuration...</p>
        </div>
      </div>
    );
  }

  return (
    <Elements stripe={stripePromise}>
      <CheckoutForm />
    </Elements>
  );
};

export default Checkout;
