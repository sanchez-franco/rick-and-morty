import CustomTable from '../../components/Table/CustomTable';
import styles from './Dashboard.module.scss';
import { useEffect, useRef, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { fetchCharacters, toggleFavorite } from '../../store/actions';
import FavoriteCounter from '../../components/FavoriteCounter/FavoriteCounter';
import SearchBar from '../../components/SearchBar/SearchBar';
import type { Character } from '../../types/Character';
import { Button, Modal } from 'antd';
import type { AppState } from '../../store/types';
import { parseFilters } from '../../utils/filter';
import loadingGif from '../../assets/images/loading.gif';
import logo from '../../assets/images/rickandmorty.png';

const Dashboard = () => {
  const dispatch = useDispatch();
  const characters = useSelector((state: AppState) => state.characters);
  const loading = useSelector((state: AppState) => state.loading);
  const currentPage = useSelector((state: AppState) => state.currentPage);
  const info = useSelector((state: AppState) => state.info);
  const [selectedCharacters, setSelectedCharacters] = useState<Character[]>([]);
  const [modalCharacter, setModalCharacter] = useState<Character | null>(null);
  const webSocket = useRef<WebSocket | null>(null);

  const closeWebSocket = () => {
    webSocket.current?.close();
    webSocket.current = null;
  };
  
  useEffect(() => {
    const wsUrl = `wss://localhost:7114/ws/favorites?email=${sessionStorage.getItem('email')}`; 

    const ws = new WebSocket(wsUrl);

    ws.onopen = () => {
      console.log("WebSocket conectado");
    };

    ws.onmessage = (event) => {
      console.log("Mensaje recibido:");
      console.log("Mensaje recibido:", event.data);
    };

    ws.onclose = () => {
      console.log("WebSocket desconectado");
    };

    webSocket.current = ws;

    window.addEventListener('beforeunload', closeWebSocket);
 
    return () => {
      closeWebSocket();
      window.removeEventListener('beforeunload', closeWebSocket);
    };
  }, []);

  useEffect(() => {
    dispatch(fetchCharacters(1));
   
  
  }, [dispatch]);

  const handleToggleFavorite = (id: number) => {
    dispatch(toggleFavorite(id));
  };    

  const handleToggleFavoriteOfSelected = () => {
    selectedCharacters.forEach((char) => {
      dispatch(toggleFavorite(char.id));
    });
  };

  const handleShowDetailsOfSelected = () => {
    if (selectedCharacters.length === 1) {
      setModalCharacter(selectedCharacters[0]);
    }
  };

  const handlePageChange = (page: number) => {
    dispatch(fetchCharacters(page)); 
  };

  
  const handleFetchSearch = async (searchTerm: string) => {
      const filters = parseFilters(searchTerm);
      dispatch(fetchCharacters(1, filters))
  };

  const handleLogOut = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('expiresAt');
    sessionStorage.removeItem('token');
    sessionStorage.removeItem('email'); 
    window.location.href = '/';
  };


  return (
    <div className={styles.container}>
      <div className={styles.logOutContainer}>
        <Button type="primary" onClick={handleLogOut}>
          Log out
        </Button>
      </div>
      <div className={styles.titleContainer}>
        <img src={logo} className={styles.imgLogo} />
        <h1 className={styles.title}>Rick and Morty Characters</h1>
      </div>
      {loading ?
      <>
        <p className={styles.text}>Loading...</p>
        <img src={loadingGif} className={styles.image} />
      </>  : (
        <>
          <SearchBar
            selectedCount={selectedCharacters.length}
            onToggleFavorite={handleToggleFavoriteOfSelected}
            onShowDetails={handleShowDetailsOfSelected}
            onSearch={handleFetchSearch}
          />
          <CustomTable 
            data={characters}
            onToggleFavorite={handleToggleFavorite} 
            onSelectRows={setSelectedCharacters}
            currentPage={currentPage}
            totalItems={info?.count || 0}
            onPageChange={(page) => handlePageChange(page)}
            loading={loading}
          />
          <FavoriteCounter />

          <Modal
            open={!!modalCharacter}
            onCancel={() => setModalCharacter(null)}
            title={modalCharacter?.name}
            footer={null}
          >
            {modalCharacter && (
              <div>
                <img
                  src={modalCharacter.image}
                  alt={modalCharacter.name}
                  className={styles.imageCharacter}
                />
                <p><strong>Full Name:</strong> {modalCharacter.name}</p>
                <p><strong>Origin:</strong> {modalCharacter.origin?.name}</p>
                <p><strong>Episodes Count:</strong> {modalCharacter.episode?.length}</p>
                <p><strong>Species:</strong> {modalCharacter.species}</p>
                <p><strong>Status:</strong> {modalCharacter.status}</p>
              </div>
            )}
          </Modal>
        </>
      )}
    </div>
  );
};

export default Dashboard;
