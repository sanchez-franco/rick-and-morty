import React, { useEffect } from 'react';
import { Navigate, Outlet, useNavigate } from 'react-router-dom';
import { message } from 'antd';

const PrivateRoute: React.FC = () => {
  const navigate = useNavigate();
  const token = sessionStorage.getItem('token');
  const expiresAt = sessionStorage.getItem('expiresAt');

  useEffect(() => {
    if (expiresAt && Date.now() > parseInt(expiresAt)) {
      sessionStorage.removeItem('token');
      sessionStorage.removeItem('expiresAt');
      message.warning('Sesión expirada. Inicia sesión nuevamente.');
      navigate('/');
    }
  }, [expiresAt, navigate]);

  if (!token) return <Navigate to="/" replace />;

  return <Outlet />;
};

export default PrivateRoute;
