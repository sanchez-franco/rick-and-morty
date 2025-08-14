import { BrowserRouter, Routes, Route } from 'react-router-dom';
import './App.css'
import Login from './pages/Login/Login'
import Dashboard from './pages/Dashboard/Dashboard';
import { Provider } from 'react-redux';
import { store } from './store';
import PrivateRoute from './components/PrivateRoute/PrivateRoute';
import SignUp from './pages/SignUp/SignUp';
import FavoritesWebSocketListener from './websocket/socket';

function App() {

  return (
    <Provider store={store}>
      <BrowserRouter>
        {/* <FavoritesWebSocketListener /> */}
        <Routes>
          <Route path="/" element={<Login />} />
          <Route path="/sign-up" element={<SignUp />} />
          <Route path="/dashboard" element={<PrivateRoute />}>
            <>
              <Route index element={<Dashboard />} />
            </>
          </Route>
        </Routes>
      </BrowserRouter>
    </Provider>
  )
}

export default App
