import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './ProductDetails.css';
import { useMutation } from '@apollo/client';
import { gql } from '@apollo/client';

const ADD_PROMOTION = gql`
    mutation AddPromotionToProduct($productId: Int!, $promotionPrice: Float!, $promotionStartDate: DateTime!, $promotionEndDate: DateTime!) {
        addPromotionToProduct(productId: $productId, promotionPrice: $promotionPrice, promotionStartDate: $promotionStartDate, promotionEndDate: $promotionEndDate)
        {
            id
            promotionPrice
            promotionStartDate
            promotionEndDate
        }
        }`

const AddPromotion = () => {
    const navigate = useNavigate();
    const [quantity, setQuantity] = useState(1);
    const [errors] = useState({});

    let product = null;
    try {
        const raw = localStorage.getItem('selectedProduct');
        product = raw ? JSON.parse(raw) : null;
    } catch {
        product = null;
    }

    const [formData, setFormData] = useState({
        promotionPrice: '',
        promotionStartDate: '',
        promotionEndDate: '',
    });

    const handleChange = (e) => {
        setFormData((prev) => ({
            ...prev,
            [e.target.name]: e.target.value,
        }));
    };
    const  [addPromotion] = useMutation(ADD_PROMOTION);
    const handleAddToPromotion=async () =>{
        if(!formData.promotionPrice || !formData.promotionStartDate || !formData.promotionEndDate){
            alert('Please fill in all promotion details');
            return;
        }
        
        // Convert date strings to ISO DateTime format with timezone
        const startDateTime = new Date(formData.promotionStartDate + 'T00:00:00Z').toISOString();
        const endDateTime = new Date(formData.promotionEndDate + 'T23:59:59Z').toISOString();
        
        const response = await addPromotion({
            variables:{
                productId: parseInt(product.id),
                promotionPrice: parseFloat(formData.promotionPrice),
                promotionStartDate: startDateTime,
                promotionEndDate: endDateTime,
            }
        });
        console.log('Promotion added:', response.data);
        alert('Promotion added successfully!');
        localStorage.removeItem('selectedProduct');
        navigate('/FoodList');
            }


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
                        <span className="price">${Number(product.price).toFixed(2)}</span>
                        <span className="unit">{product.unit}</span>
                    </div>

                    <div className={`stock-status ${product.inStock ? 'in-stock' : 'out-of-stock'}`}>
                        {product.inStock ? '✓ In Stock' : '✗ Out of Stock'}
                    </div>

                    <div className="product-details-info">
                        <label>Promotion Price *</label>
                        <input
                            type="number"
                            name="promotionPrice"
                            value={formData.promotionPrice}
                            onChange={handleChange}
                            className={errors.promotionPrice ? 'error' : ''}
                            placeholder="0.00"
                            step="0.01"
                            min="0"
                        />
                        {errors.promotionPrice && <span className="error-message">{errors.promotionPrice}</span>}
                    </div>

                    <div className="product-details-info">
                        <label>Promotion Start Date *</label>
                        <input
                            type="date"
                            name="promotionStartDate"
                            value={formData.promotionStartDate}
                            onChange={handleChange}
                            className={errors.promotionStartDate ? 'error' : ''}
                        />
                        {errors.promotionStartDate && (
                            <span className="error-message">{errors.promotionStartDate}</span>
                        )}
                    </div>

                    <div className="product-details-info">
                        <label>Promotion End Date *</label>
                        <input
                            type="date"
                            name="promotionEndDate"
                            value={formData.promotionEndDate}
                            onChange={handleChange}
                            className={errors.promotionEndDate ? 'error' : ''}
                        />
                        {errors.promotionEndDate && (
                            <span className="error-message">{errors.promotionEndDate}</span>
                        )}
                    </div>

                    <p className="product-details-description">{product.description}</p>

                    <div className="product-actions">

                       <button
                            onClick={handleAddToPromotion}
                            disabled={!product.inStock}
                            className="add-to-cart-btn-large"
                        >
                            Add to Promotion
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default AddPromotion;
