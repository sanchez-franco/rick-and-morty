import React, { useState, useCallback } from 'react';
import { Form, Input, Button, message } from 'antd';
import styles from './SignUp.module.scss';
import users from '../../utils/users.json';
import type { User } from '../../types/User';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';

const SignUp: React.FC = () => {
  const [form] = Form.useForm();
  const [isFormValid, setIsFormValid] = useState(false);
  const navigate = useNavigate();

   const handleFinish = async (values: User): Promise<void> => {
    try {
    const apiUrl = import.meta.env.VITE_REACT_APP_API_URL || '';
      const response = await axios.post(
        `${apiUrl}/SignUp`,
        values
      );
      if (response.data?.token) {
        localStorage.setItem('token', response.data.token);
      }
      message.success('Account created successfully!');
      navigate('/');
    } catch (error: any) {
      if (error.response && error.response.data && error.response.data.message) {
        message.error(error.response.data.message);
      } else {
        message.error('Error creating account');
      }
    }
  };

  const validateForm = useCallback((): void => {
    const values = form.getFieldsValue();
    const hasEmail = values.email && values.email.trim() !== '';
    const hasPassword = values.password && values.password.trim() !== '';
    const isEmailValid = values.email && /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(values.email);
        
    setIsFormValid(hasEmail && hasPassword && isEmailValid);
  }, [form]);

  return (
    <div className={styles.loginContainer}>
      <div className={styles.loginForm}>
         <h1 className={styles.title}>Sign Up</h1>
        <Form
          form={form}
          layout="vertical"
          onFinish={handleFinish}
          onFieldsChange={validateForm}
          autoComplete="off"
        >
          <Form.Item
            label="Email"
            name="email"
            rules={[
              { required: true, message: 'Email is required' },
              { type: 'email', message: 'Invalid email format' }
            ]}
          >
            <Input 
              className={styles.input}
              placeholder="Enter your email"
              autoComplete="email"
            />
          </Form.Item>

          <Form.Item
            label="Password"
            name="password"
            rules={[
              { required: true, message: 'Password is required' }
            ]}
          >
            <Input.Password 
              className={styles.input}
              placeholder="Enter your password"
              autoComplete="current-password"
            />
          </Form.Item>

          <Form.Item>
            <Button 
              type="primary" 
              htmlType="submit" 
              disabled={!isFormValid} 
              block
            >
              Sign Up
            </Button>
          </Form.Item>
        </Form>

        <a href='/' className={styles.text}>Already have an account? Login</a>
      </div>
    </div>
  );
};

export default SignUp;