import React from 'react';
import { useNavigate } from 'react-router-dom';
import './SignIn.css';
import { useState } from 'react';
import { gql,useLazyQuery,useQuery } from '@apollo/client';

function SignIn() {
    const [message, setMessage] = useState(null);
        const [err, setErr] = useState(null);
    const navigate = useNavigate();
    const [inputs, setInputs] = useState({
        email: "",
        password: "",
    });
    
    const CHECK_AUTHENTICATION = gql`
        query CheckAuthCustomerAsync($email: String!, $password: String!) {
            checkAuthCustomer(email: $email, password: $password){
                id
                email
            }
        }`;
    const [checkAuthCustomer] = useLazyQuery(CHECK_AUTHENTICATION);
    const handleSubmit = (e) => {
        e.preventDefault();
        checkAuthCustomer({variables: {email: inputs.email, password: inputs.password}})
        .then (response =>{
            setMessage("Sign in successful! Redirecting...");
            console.log('User signed in:', response.data.checkAuthCustomer);
            setTimeout(() => navigate('/'), 2000);
            localStorage.setItem('user', JSON.stringify(response.data.checkAuthCustomer));
        })
        .catch (error =>{
            console.error('Error signing in:', error);
            const backendMessage = error.graphQLErrors?.[0]?.message;
            setErr(backendMessage || error.message || "Failed to sign in. Please try again.");
            setMessage(null);
        });
    };
    


    return (
        <div className="login-container">
            <div className="login-card">
                <div className="login-header">
                    <h2 className="login-title">Welcome Back! 🍋</h2>
                    <p className="login-subtitle">Sign in to your account</p>
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
                        Sign In
                    </button>
                    {message && <div className="success-message">{message}</div>}
                    {err && <div className="error-message">{err}</div>}
                    <div className="signup-prompt">
                        <span>Don't have an account? </span>
                        <button 
                            type="button"
                            onClick={() => navigate('/SignUp')} 
                            className="btn-link"
                        >
                            Sign up here
                        </button>
                    </div>
                </form>
            </div>
        </div>
    )
}

export default SignIn;