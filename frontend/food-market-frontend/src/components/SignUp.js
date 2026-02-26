import React from 'react';
import { useNavigate } from 'react-router-dom';
import './SignIn.css';
import { useState } from 'react';
import { gql,useMutation } from '@apollo/client';

function SignUp() {
   
    const [err, setErr] = useState(null);
    const navigate = useNavigate();
    const [inputs, setInputs] = useState({
        email: "",
        password: "",
    });
    const [message, setMessage] = useState(null);
  const CREATE_NEW_USER = gql`
  mutation CreateCustomer($email: String!, $password: String!) {
    createCustomer(email: $email, password: $password) {
      id
      email
    }
  }
`;

// Use the mutation hook
const [createCustomer] = useMutation(CREATE_NEW_USER);
    const handleSubmit = (e) => {
        e.preventDefault();
        createCustomer({variables: {email: inputs.email, password: inputs.password}})
        .then(response => {
            setMessage("Account created successfully! Please sign in.");
            console.log('User created:', response.data.createCustomer);
            setTimeout(() => navigate('/SignIn'), 2000);
        })
        .catch(error => {
            console.error('Error creating user:', error);
            const backendMessage = error.graphQLErrors?.[0]?.message;
            
            setErr(backendMessage || error.message || "Failed to create account. Please try again.");
            setMessage(null);
        });
    };
    return (
        <div className="login-container">
            <div className="login-card">
                <div className="login-header">
                    <h2 className="login-title">Create Account! 🍋</h2>
                    <p className="login-subtitle">Sign up for a new account</p>
                </div>
                
                <form className="login-form" action="/action_page.php">
                    <div className="form-group">
                        <label htmlFor="email" className="form-label">Email</label>
                        <input 
                            type="email" 
                            className="form-input" 
                            id="email" 
                            placeholder="Enter your email" 
                            name="email" 
                            onChange ={e => setInputs({...inputs, email:e.target.value})}
                        />
                    </div>
                    
                    <div className="form-group">
                        <label htmlFor="pwd" className="form-label">Password</label>
                        <input 
                            type="password" 
                            className="form-input" 
                            id="pwd" 
                            placeholder="Enter your password" 
                            name="pswd" 
                            onChange ={e=> setInputs({...inputs, password:e.target.value})}
                        />
                    </div>
                    
                    <div className="form-check">
                        <input 
                            className="form-checkbox" 
                            type="checkbox" 
                            id="remember"
                            name="remember" 
                        />
                        <label htmlFor="remember" className="checkbox-label">
                            Remember me
                        </label>
                    </div>
                    
                    <button type="submit" onClick={handleSubmit} className="btn-primary">
                        Sign Up
                    </button>
                    {message && <div className="success-message">{message}</div>}
                    {err && <div className="error-message">{err}</div>}
                    <div className="signup-prompt">
                        <span>Already have an account? </span>
                        <button 
                            type="button"
                            onClick={() => navigate('/SignIn')} 
                            className="btn-link"
                        >
                            Sign in here
                        </button>
                    </div>
                </form>
            </div>
        </div>
    )
}

export default SignUp;