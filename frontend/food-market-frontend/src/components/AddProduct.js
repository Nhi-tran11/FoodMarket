// /Users/xuannhitran/Documents/FoodMarketApp/frontend/food-market-frontend/src/components/AddProduct.js
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './AddProduct.css';
import { useMutation } from '@apollo/client';
import { gql } from '@apollo/client';

const AddProduct = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    price: '',
    category: 'Fruits',
    unit: 'per lb',
    stockQuantity: '',
    inStock: true
  });

  const [selectedFile, setSelectedFile] = useState(null);
  const [imagePreview, setImagePreview] = useState('');
  const [uploadProgress, setUploadProgress] = useState(false);
  const [errors, setErrors] = useState({});

  const categories = ['Fruits', 'Vegetables', 'Dairy', 'Meat', 'Bakery', 'Beverages', 'Snacks', 'Frozen Foods', 'Pantry', 'Seafood'];
  const units = ['per lb', 'per kg', 'per dozen', 'per unit', 'per pack'];

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));
    // Clear error for this field
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  const handleFileChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      // Validate file type
      const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
      if (!allowedTypes.includes(file.type)) {
        setErrors(prev => ({ ...prev, image: 'Please select a valid image file (JPG, PNG, GIF, WEBP)' }));
        return;
      }

      // Validate file size (max 5MB)
      if (file.size > 5 * 1024 * 1024) {
        setErrors(prev => ({ ...prev, image: 'File size must be less than 5MB' }));
        return;
      }

      setSelectedFile(file);
      
      // Create preview
      const reader = new FileReader();
      reader.onloadend = () => {
        setImagePreview(reader.result);
      };
      reader.readAsDataURL(file);

      // Clear error
      if (errors.image) {
        setErrors(prev => ({ ...prev, image: '' }));
      }
    }
  };

  const uploadImageToBackend = async () => {
    if (!selectedFile) {
      throw new Error('No file selected');
    }

    setUploadProgress(true);
    const formDataUpload = new FormData();
    formDataUpload.append('file', selectedFile);

    try {
      const response = await fetch('http://localhost:5000/api/FileUpload/upload', {
        method: 'POST',
        body: formDataUpload,
      });

      const data = await response.json();
      
      if (!response.ok) {
        throw new Error(data.error || 'Image upload failed');
      }

      setUploadProgress(false);
      return data.path; // Returns relative path like "/images/products/guid.jpg"
    } catch (error) {
      setUploadProgress(false);
      console.error('Error uploading image:', error);
      throw error;
    }
  };
  
  const validateForm = () => {
    const newErrors = {};
    if (!formData.name.trim()) newErrors.name = 'Product name is required';
    if (!formData.description.trim()) newErrors.description = 'Description is required';
    if (!formData.price || parseFloat(formData.price) <= 0) {
      newErrors.price = 'Valid price is required';
    }
    if (!formData.stockQuantity || parseInt(formData.stockQuantity) < 0) {
      newErrors.stockQuantity = 'Valid stock quantity is required';
    }
    if (!selectedFile) {
      newErrors.image = 'Product image is required';
    }
    
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const ADD_PRODUCT = gql`
    mutation CreateProduct($name: String!, $description: String!, $price: Float!, $category: String!, $image: String!, $inStock: Boolean!, $stockQuantity: Int!, $unit: String!) {
      createProduct(name: $name, description: $description, price: $price, category: $category, image: $image, inStock: $inStock, stockQuantity: $stockQuantity, unit: $unit) {
        id
        name
        image
      }
    }
  `;

  const [addProduct] = useMutation(ADD_PRODUCT);

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!validateForm()) {
      return;
    }

    try {
      // Step 1: Upload image to backend (which saves to frontend's public folder)
      const imagePath = await uploadImageToBackend();
      
      if (!imagePath) {
        alert('Failed to upload image');
        return;
      }

      console.log('Image saved with path:', imagePath);

      // Step 2: Add product to database with the relative image path
      const response = await addProduct({
        variables: {
          name: formData.name,
          description: formData.description,
          price: parseFloat(formData.price),
          category: formData.category,
          image: imagePath, // Relative path like "/images/products/guid.jpg"
          inStock: formData.inStock,
          stockQuantity: parseInt(formData.stockQuantity),
          unit: formData.unit
        }
      });

      console.log('Product added:', response.data.createProduct);
      alert('Product added successfully!');
      navigate('/FoodList');
    } catch (error) {
      console.error('Error adding product:', error);
      alert('Failed to add product: ' + error.message);
    }
  };

  const handleCancel = () => {
    navigate('/FoodList');
  };

  return (
    <div className="add-product-container">
      <div className="add-product-header">
        <button onClick={handleCancel} className="back-btn">
          ← Back to Products
        </button>
        <h1>Add New Product</h1>
      </div>

      <form onSubmit={handleSubmit} className="add-product-form">
        <div className="form-section">
          <h2>Product Information</h2>
          
          <div className="form-group">
            <label>Product Name *</label>
            <input
              type="text"
              name="name"
              value={formData.name}
              onChange={handleChange}
              className={errors.name ? 'error' : ''}
              placeholder="e.g., Fresh Organic Apples"
            />
            {errors.name && <span className="error-message">{errors.name}</span>}
          </div>

          <div className="form-group">
            <label>Description *</label>
            <textarea
              name="description"
              value={formData.description}
              onChange={handleChange}
              className={errors.description ? 'error' : ''}
              placeholder="Describe your product..."
              rows="4"
            />
            {errors.description && <span className="error-message">{errors.description}</span>}
          </div>

          <div className="form-row">
            <div className="form-group">
              <label>Category *</label>
              <select
                name="category"
                value={formData.category}
                onChange={handleChange}
              >
                {categories.map(cat => (
                  <option key={cat} value={cat}>{cat}</option>
                ))}
              </select>
            </div>

            <div className="form-group">
              <label>Price *</label>
              <input
                type="number"
                name="price"
                value={formData.price}
                onChange={handleChange}
                className={errors.price ? 'error' : ''}
                placeholder="0.00"
                step="0.01"
                min="0"
              />
              {errors.price && <span className="error-message">{errors.price}</span>}
            </div>

            <div className="form-group">
              <label>Stock Quantity *</label>
              <input
                type="number"
                name="stockQuantity"
                value={formData.stockQuantity}
                onChange={handleChange}
                className={errors.stockQuantity ? 'error' : ''}
                placeholder="0"
                min="0"
              />
              {errors.stockQuantity && <span className="error-message">{errors.stockQuantity}</span>}
            </div>

            <div className="form-group">
              <label>Unit *</label>
              <select
                name="unit"
                value={formData.unit}
                onChange={handleChange}
              >
                {units.map(unit => (
                  <option key={unit} value={unit}>{unit}</option>
                ))}
              </select>
            </div>
          </div>

          <div className="form-group">
            <label>Product Image *</label>
            <input
              type="file"
              accept="image/*"
              onChange={handleFileChange}
              className={errors.image ? 'error' : ''}
            />
            {errors.image && <span className="error-message">{errors.image}</span>}
            {imagePreview && (
              <div className="image-preview">
                <img src={imagePreview} alt="Preview" />
              </div>
            )}
            {uploadProgress && <p className="upload-progress">Uploading image...</p>}
          </div>

          <div className="form-group checkbox-group">
            <label>
              <input
                type="checkbox"
                name="inStock"
                checked={formData.inStock}
                onChange={handleChange}
              />
              <span>In Stock</span>
            </label>
          </div>
        </div>

        <div className="form-actions">
          <button type="button" onClick={handleCancel} className="btn-cancel">
            Cancel
          </button>
          <button type="submit" className="btn-submit" disabled={uploadProgress}>
            {uploadProgress ? 'Uploading...' : 'Add Product'}
          </button>
        </div>
      </form>
    </div>
  );
};

export default AddProduct;