import * as React from 'react';

const PopoverContext = React.createContext<(
  isShow: boolean,
  setIsShow: React.Dispatch<React.SetStateAction<boolean>>
)>({
  isShow: false,
  setIsShow: () => {
    throw new Error('PopoverContext should be used under provider');
  }
})

export default function Popover({ children }: {children: React.ReactNode}) {
  const [isShow, setIsShow] = useState(false);

  const contextValue = {
    isShow,
    setIsShow
  };
  return (
    <PopoverContext.Provider value={contextValue}>
      {children}
    </PopoverContext.Provider>
  );
}



function Trigger ( {children}: {children: React.ReactElement} ) {
  const onClick = () => {
    //TODO
  }
  const childrenToTriggerPopover = React.cloneElement( children, {onclick,});
  return childrenToTriggerPopover;
}

function Content ( {children}: {children: React.ReactNode} ) {
  return children;
}

function Close ( {children}: {children: React.ReactNode} ) {
  return children;
}

Popover.Trigger = Trigger;
Popover.Content = Content;
Popover.Close = Close;
