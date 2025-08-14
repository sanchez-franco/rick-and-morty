import React from 'react';
import { useSelector } from 'react-redux';
import type { RootState } from '../../store';
import styles from './FavoriteCounter.module.scss'

const FavoriteCounter: React.FC = () => {
  const favoritesCount = useSelector((state: RootState) =>
    state.characters.filter((c) => c.isFavorite).length
  );

  return <div className={styles.text}>Favorites: {favoritesCount}</div>;
};

export default FavoriteCounter;
