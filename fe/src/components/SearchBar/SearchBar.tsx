import React, { useState } from 'react';
import { Dropdown, Button, Menu, Input } from 'antd';
import { DownOutlined } from '@ant-design/icons';
import styles from './SearchBar.module.scss';
import { fetchCharacters } from '../../store/actions';
import { useDispatch } from 'react-redux';

interface Props {
  selectedCount: number;
  onToggleFavorite: () => void;
  onShowDetails: () => void;
  onSearch: (searchTerm: string) => void;
}

const SearchBar: React.FC<Props> = ({ selectedCount, onToggleFavorite, onShowDetails, onSearch }) => {
  const [searchTerm, setSearchTerm] = useState('');
    const dispatch = useDispatch();

   const handleSearch = () => {
    onSearch(searchTerm.trim());
  }
  
  const handleClear = () => {
    dispatch(fetchCharacters(1))
  }

  const menu = (
    <Menu className={styles.menuCustom}>
      <Menu.Item key="favorite" onClick={onToggleFavorite}
        className={styles.menuItemCustom}>
        Toggle Favorite ({selectedCount} selected)
      </Menu.Item>
      {selectedCount==1 &&
      <Menu.Item
        className={styles.menuItemCustom}
        key="details"
        onClick={onShowDetails}
        disabled={selectedCount !== 1}
      >
        View Details
      </Menu.Item>
      }
      
    </Menu>
  );

  return (
    <div className={styles.searchContainer}>
      <Input
        placeholder='Search by name, status or gender (e.g. "rick alive male")'
        value={searchTerm}
        onChange={e => setSearchTerm(e.target.value)}
        allowClear
        style={{ width: 400 }}
      />
      <Button type="primary" onClick={handleSearch}>
        Search
      </Button>
      <Button type="primary" onClick={handleClear}>
        Clear
      </Button>
      <Dropdown overlay={menu} disabled={selectedCount === 0}>
        <Button>
          Actions <DownOutlined />
        </Button>
      </Dropdown>
    </div>
  );
};

export default SearchBar;
